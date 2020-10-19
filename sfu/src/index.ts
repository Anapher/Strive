import Redis from 'ioredis';
import * as mediasoup from 'mediasoup';
import { Worker } from 'mediasoup/lib/types';
import config from './config';
import conferenceFactory from './lib/conference-factory';
import ConferenceManager from './lib/conference-manager';
import Logger from './lib/logger';
import { RedisMessageProcessor } from './lib/redis-message-processor';
import { CallbackMessage, CallbackResponse, ConferenceInfo } from './lib/types';
import { ChannelName, channels, onClientDisconnected, rtpCapabilitiesKey } from './lib/pader-conference/redis-channels';

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

async function initializeConference(conferenceInfo: ConferenceInfo): Promise<void> {
   const worker = getMediasoupWorker();
   const conference = await conferenceFactory(conferenceInfo, worker, redis);
   conferenceManager.createConference(conference);

   await redis.set(rtpCapabilitiesKey.getName(conferenceInfo.id), JSON.stringify(conference.routerCapabilities));

   logger.info(`Added new conference ${conferenceInfo.id}`);

   // subscribe to all requests targeting this conference
   subRedis.psubscribe(`${conference.conferenceId}/req::*`);
   subRedis.subscribe(onClientDisconnected.getName(conferenceInfo.id));
}

type MappedMessage = {
   channel: ChannelName | string;
   handler: (request: any) => any | Promise<any>;
};

const processor = new RedisMessageProcessor(redis, conferenceManager, initializeConference);

const messagesMap: MappedMessage[] = [
   { channel: channels.request.initializeConnection, handler: processor.initializeConnection },
   { channel: channels.request.createTransport, handler: processor.createTransport },
   { channel: channels.request.connectTransport, handler: processor.connectTransport },
   { channel: channels.request.transportProduce, handler: processor.transportProduce },
   { channel: onClientDisconnected, handler: processor.clientDisconnected },
   { channel: channels.newConferenceCreated, handler: processor.newConferenceCreated },
];

async function subscribeRedis() {
   subRedis.subscribe(channels.newConferenceCreated);

   const handleMessage = async (channel: string, message: string) => {
      logger.debug('Redis pmessage in channel %s received: %s', channel, message);
      let callbackChannel: string | undefined;

      try {
         // try to find the mapped message
         const mappedMessage = messagesMap.find((x) => x.channel.match(channel));
         if (!mappedMessage) {
            throw new Error('Channel is not mapped');
         }

         let param: any | undefined;
         if (message) {
            // extract the parameter
            const data: CallbackMessage<any> = JSON.parse(message);
            if (data.callbackChannel) {
               // we have a callback message with a channel
               callbackChannel = data.callbackChannel;
               param = data.payload;
            } else param = data;
         }

         // resolve
         const methodResponse = mappedMessage.handler(param);
         const result = await Promise.resolve(methodResponse);

         if (callbackChannel) {
            // return response
            const response: CallbackResponse<any> = { payload: result };
            redis.publish(callbackChannel, JSON.stringify(response));
         }
      } catch (error) {
         logger.error('Error occurred when executing channel %s: %s', channel, error.toString());

         if (callbackChannel) {
            // return error
            const response: CallbackResponse<undefined> = { error: true, errorMesage: error.toString() };
            redis.publish(callbackChannel, JSON.stringify(response));
         }
      }
   };

   subRedis.on('pmessage', (_, channel, message) => handleMessage(channel, message));
   subRedis.on('message', async (channel: string, message: string) => handleMessage(channel, message));
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
