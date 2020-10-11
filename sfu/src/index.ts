import Redis from 'ioredis';
import * as mediasoup from 'mediasoup';
import { Worker } from 'mediasoup/lib/types';
import config from './config';
import createConference from './lib/conference-factory';
import ConferenceManager from './lib/conference-manager';
import Connection from './lib/connection';
import Logger from './lib/Logger';
import { ConferenceInfo, CreateTransportRequest, InitializeConnectionRequest } from './lib/types';
import { channels, newConferencesKey, rtpCapabilitiesKey } from './redis';

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

main();

async function main() {
   // Run a mediasoup Worker.
   await runMediasoupWorkers();

   await subscribeRedis();
}

async function subscribeRedis() {
   subRedis.subscribe(channels.newConferenceCreated);

   subRedis.on('message', async (channel: string, message: string) => {
      switch (channel) {
         case channels.newConferenceCreated:
            const conferenceStr = await redis.lpop(newConferencesKey);
            if (conferenceStr) {
               const conferenceInfo: ConferenceInfo = JSON.parse(conferenceStr);

               const worker = getMediasoupWorker();
               const conference = await createConference(conferenceInfo, worker);
               conferenceManager.createConference(conference);

               await redis.set(rtpCapabilitiesKey(conferenceInfo.id), JSON.stringify(conference.routerCapabilities));

               logger.info(`Added new conference ${conferenceInfo.id}`);

               // subscribe to all requests targeting this conference
               subRedis.psubscribe(`${conference.conferenceId}/req::*`);
            }
            break;
         default:
            if (channels.request.initializeConnection.match(channel)) {
               const request: InitializeConnectionRequest = JSON.parse(message);
               const conference = conferenceManager.getConference(request.meta.conferenceId);

               const connection = new Connection(
                  request.payload.rtpCapabilities,
                  request.payload.sctpCapabilities,
                  request.meta.connectionId,
                  request.meta.participantId,
               );

               conference.addConnection(connection);
            } else if (channels.request.createTransport.match(channel)) {
               const request: CreateTransportRequest = JSON.parse(message);

               const conference = conferenceManager.getConference(request.meta.conferenceId);
               const response = await conference.createTransport(request);
               await redis.publish(
                  channels.response.createTransport.getName(request.meta.conferenceId),
                  JSON.stringify(response),
               );
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
