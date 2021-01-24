import Redis from 'ioredis';
import * as mediasoup from 'mediasoup';
import { Worker } from 'mediasoup/lib/types';
import config from './config';
import { SuccessOrError } from './lib/communication-types';
import conferenceFactory from './lib/conference-factory';
import ConferenceManager from './lib/conference-manager';
import * as errors from './lib/errors';
import Logger from './lib/logger';
import {
   ChannelName,
   channels,
   onClientDisconnected,
   onRoomSwitched,
   rtpCapabilitiesKey,
} from './lib/pader-conference/redis-channels';
import { newConferences, openConferences } from './lib/pader-conference/redis-keys';
import { RedisMessageProcessor } from './lib/redis-message-processor';
import { CallbackMessage, ConferenceInfo } from './lib/types';

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
const processor = new RedisMessageProcessor(conferenceManager);

logger.info('Starting server...');

main();

async function main() {
   // Run a mediasoup Worker.
   await runMediasoupWorkers();

   await subscribeRedis();

   initializeExistingConferences();
}

function initializeExistingConferences() {
   redis.hgetall(openConferences, (err, result) => {
      for (const key in result) {
         initializeConference({ id: key });
      }
   });
}

async function initializeConference(conferenceInfo: ConferenceInfo): Promise<void> {
   if (conferenceManager.hasConference(conferenceInfo.id)) return;

   const worker = getMediasoupWorker();
   const conference = await conferenceFactory(conferenceInfo, worker, redis);
   conferenceManager.createConference(conference);

   await redis.set(rtpCapabilitiesKey.getName(conferenceInfo.id), JSON.stringify(conference.routerCapabilities));

   logger.info(`Added new conference ${conferenceInfo.id}`);

   // subscribe to all requests targeting this conference
   subRedis.psubscribe(`${conference.conferenceId}/req::*`);

   // events
   subRedis.subscribe(onClientDisconnected.getName(conferenceInfo.id));
   subRedis.subscribe(onRoomSwitched.getName(conferenceInfo.id));
}

const onNewConferenceCreated: () => Promise<SuccessOrError> = async () => {
   const conferenceStr = await redis.lpop(newConferences);
   if (conferenceStr) {
      const conferenceInfo: ConferenceInfo = JSON.parse(conferenceStr);
      initializeConference(conferenceInfo);
   }

   return { success: true };
};

type MappedMessage = {
   channel: ChannelName | string;
   handler: (request: any) => Promise<SuccessOrError> | SuccessOrError;
};

const messagesMap: MappedMessage[] = [
   { channel: channels.request.initializeConnection, handler: processor.initializeConnection.bind(processor) },
   { channel: channels.request.createTransport, handler: processor.createTransport.bind(processor) },
   { channel: channels.request.connectTransport, handler: processor.connectTransport.bind(processor) },
   { channel: channels.request.transportProduce, handler: processor.transportProduce.bind(processor) },
   { channel: channels.request.changeStream, handler: processor.changeStream.bind(processor) },
   { channel: channels.request.changeProducerSource, handler: processor.changeProducerSource.bind(processor) },
   { channel: onRoomSwitched, handler: processor.roomSwitched.bind(processor) },
   { channel: onClientDisconnected, handler: processor.clientDisconnected.bind(processor) },
   { channel: channels.newConferenceCreated, handler: onNewConferenceCreated },
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
            const data: CallbackMessage<any> | null = JSON.parse(message);
            if (data?.callbackChannel) {
               // we have a callback message with a channel
               callbackChannel = data.callbackChannel;
               param = data.payload;
            } else param = data;
         }

         // resolve
         const methodResponse = mappedMessage.handler(param);
         const result = await Promise.resolve(methodResponse);

         if (!result.success) {
            logger.warn('Channel %s executed unsuccessful: %s', channel, result.error);
         }

         if (callbackChannel) {
            // return response
            redis.publish(callbackChannel, JSON.stringify(result));
         }
      } catch (error) {
         logger.error('Error occurred when executing channel %s: %s', channel, error.toString());

         if (callbackChannel) {
            // return error
            const response: SuccessOrError = {
               success: false,
               error: errors.internalError(`Error executing message from channel ${channel}: ${error}`),
            };
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
