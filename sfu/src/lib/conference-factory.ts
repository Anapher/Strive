import { Worker } from 'mediasoup/lib/types';
import config from '../config';
import { Conference } from './conference';
import Logger from './Logger';
import { ConferenceInfo } from './types';

const logger = new Logger('conference-factory');

export default async function createConference(
   conferenceInfo: ConferenceInfo,
   assignedWorker: Worker,
): Promise<Conference> {
   logger.info('createConference() [conferenceId:%s]', conferenceInfo.id);

   // Create a mediasoup Router.
   const mediasoupRouter = await assignedWorker.createRouter(config.router);

   return new Conference(mediasoupRouter, conferenceInfo.id);
}
