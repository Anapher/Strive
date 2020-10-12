import Redis from 'ioredis';
import * as mediasoup from 'mediasoup';
import { Worker } from 'mediasoup/lib/types';
import config from './config';
import conferenceFactory from './lib/conference-factory';
import ConferenceManager from './lib/conference-manager';
import Connection from './lib/connection';
import Logger from './lib/Logger';
import {
   CallbackMessage,
   ConferenceInfo,
   ConnectionMessage,
   ConnectTransportRequest,
   CreateTransportRequest,
   InitializeConnectionRequest,
   TransportProduceRequest,
} from './lib/types';
import { channels, newConferencesKey, onClientDisconnected, rtpCapabilitiesKey } from './redis-communication';

const logger = new Logger();

// redis for commands
const redis = new Redis();

// redis for subscriptions
const subRedis = new Redis();

// mediasoup Workers.
const mediasoupWorkers: Worker[] = [];

// Index of next mediasoup Worker to use.
let nextMediasoupWorkerIdx = 0;

const conferenceManager = new ConferenceManager();

logger.info('Starting server...');

main();

async function main() {
   // Run a mediasoup Worker.
   await runMediasoupWorkers();

   await subscribeRedis();

   initializeExistingConferences();
}

function initializeExistingConferences() {
   redis.hgetall('conferences', (err, result) => {
      for (const key in result) {
         initializeConference({ id: key });
      }
   });
}

async function initializeConference(conferenceInfo: ConferenceInfo) {
   const worker = getMediasoupWorker();
   const conference = await conferenceFactory(conferenceInfo, worker, redis);
   conferenceManager.createConference(conference);

   await redis.set(rtpCapabilitiesKey.getName(conferenceInfo.id), JSON.stringify(conference.routerCapabilities));

   logger.info(`Added new conference ${conferenceInfo.id}`);

   // subscribe to all requests targeting this conference
   subRedis.psubscribe(`${conference.conferenceId}/req::*`);
   subRedis.subscribe(onClientDisconnected.getName(conferenceInfo.id));
}

async function subscribeRedis() {
   subRedis.subscribe(channels.newConferenceCreated);

   subRedis.on('pmessage', async (_, channel, message) => {
      logger.debug('Redis pmessage in channel %s received: %s', channel, message);

      if (channels.request.initializeConnection.match(channel)) {
         const { callbackChannel, payload: request }: CallbackMessage<InitializeConnectionRequest> = JSON.parse(
            message,
         );

         const conference = conferenceManager.getConference(request.meta.conferenceId);

         logger.debug('Initialize connection in conference %s', conference.conferenceId);

         const connection = new Connection(
            request.payload.rtpCapabilities,
            request.payload.sctpCapabilities,
            request.meta.connectionId,
            request.meta.participantId,
         );

         conference.addConnection(connection);

         redis.publish(callbackChannel, 'null');
      } else if (channels.request.createTransport.match(channel)) {
         const { payload: request, callbackChannel }: CallbackMessage<CreateTransportRequest> = JSON.parse(message);

         const conference = conferenceManager.getConference(request.meta.conferenceId);
         const response = await conference.createTransport(request);
         await redis.publish(callbackChannel, JSON.stringify(response));
      } else if (channels.request.connectTransport.match(channel)) {
         const { payload: request, callbackChannel }: CallbackMessage<ConnectTransportRequest> = JSON.parse(message);

         const conference = conferenceManager.getConference(request.meta.conferenceId);
         await conference.connectTransport(request);

         redis.publish(callbackChannel, 'null');
      } else if (channels.request.transportProduce.match(channel)) {
         const { payload: request, callbackChannel }: CallbackMessage<TransportProduceRequest> = JSON.parse(message);

         const conference = conferenceManager.getConference(request.meta.conferenceId);
         const response = await conference.transportProduce(request);

         redis.publish(callbackChannel, JSON.stringify(response));
      }
   });

   subRedis.on('message', async (channel: string, message: string) => {
      logger.debug('Redis message in channel %s received: %s', channel, message);

      switch (channel) {
         case channels.newConferenceCreated:
            const conferenceStr = await redis.lpop(newConferencesKey);
            if (conferenceStr) {
               const conferenceInfo: ConferenceInfo = JSON.parse(conferenceStr);
               initializeConference(conferenceInfo);
            }
            break;
         default:
            if (onClientDisconnected.match(channel)) {
               const request: ConnectionMessage<undefined> = JSON.parse(message);

               const conference = conferenceManager.getConference(request.meta.conferenceId);
               conference.removeConnection(request.meta.connectionId);
            }
            break;
      }
   });
}

/**
 * Get next mediasoup Worker.
 */
function getMediasoupWorker() {
   const worker = mediasoupWorkers[nextMediasoupWorkerIdx];

   if (++nextMediasoupWorkerIdx === mediasoupWorkers.length) nextMediasoupWorkerIdx = 0;

   return worker;
}

/**
 * Launch as many mediasoup Workers as given in the configuration file.
 */
async function runMediasoupWorkers() {
   const { numWorkers } = config.mediasoup;

   logger.info('running %d mediasoup Workers...', numWorkers);

   for (let i = 0; i < numWorkers; ++i) {
      const worker = await mediasoup.createWorker(config.mediasoup.workerSettings);

      worker.on('died', () => {
         logger.error('mediasoup Worker died, exiting  in 2 seconds... [pid:%d]', worker.pid);

         setTimeout(() => process.exit(1), 2000);
      });

      mediasoupWorkers.push(worker);

      // Log worker resource usage every X seconds.
      setInterval(async () => {
         const usage = await worker.getResourceUsage();

         logger.info('mediasoup Worker resource usage [pid:%d]: %o', worker.pid, usage);
      }, 120000);
   }
}
