import { RouterOptions, WebRtcTransportOptions } from 'mediasoup/lib/types';
import MediaSoupWorkers from '../../media-soup-workers';
import Logger from '../../utils/logger';
import { ConferenceRepository } from '../synchronization/conference-repository';
import RabbitMqConn from '../../rabbitmq/rabbit-mq-conn';
import RabbitPub from '../synchronization/rabbit-pub';
import { Conference } from './conference';

const logger = new Logger('conference-factory');

export default async function conferenceFactory(
   id: string,
   workers: MediaSoupWorkers,
   repository: ConferenceRepository,
   rabbit: RabbitMqConn,
   routerOptions: RouterOptions,
   webrtcOptions: WebRtcTransportOptions,
   maxIncomingBitrate?: number,
): Promise<Conference> {
   logger.info('createConference() [conferenceId: %s]', id);

   // initialize conference info, subscribe messages
   await repository.getConference(id);

   // Create a mediasoup Router.
   const assignedWorker = workers.getNextWorker();
   const mediasoupRouter = await assignedWorker.createRouter(routerOptions);

   const messenger = new RabbitPub(rabbit, id);
   return new Conference(mediasoupRouter, id, messenger, repository, webrtcOptions, maxIncomingBitrate);
}
