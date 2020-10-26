import { Redis } from 'ioredis';
import { Worker } from 'mediasoup/lib/types';
import config from '../config';
import { Conference } from './conference';
import Logger from './logger';
import { SignalWrapper } from './signal-wrapper';
import { ConferenceInfo } from './types';

const logger = new Logger('conference-factory');

export default async function conferenceFactory(
   conferenceInfo: ConferenceInfo,
   assignedWorker: Worker,
   redis: Redis,
): Promise<Conference> {
   logger.info('createConference() [conferenceId:%s]', conferenceInfo.id);

   // Create a mediasoup Router.
   const mediasoupRouter = await assignedWorker.createRouter(config.router);
   const wrapper = new SignalWrapper(redis, conferenceInfo.id);

   return new Conference(mediasoupRouter, conferenceInfo.id, wrapper, redis);
}
