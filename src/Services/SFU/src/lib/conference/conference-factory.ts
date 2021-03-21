import MediaSoupWorkers from '../../media-soup-workers';
import RabbitMqConn from '../../rabbitmq/rabbit-mq-conn';
import Logger from '../../utils/logger';
import { ConferenceRepository } from '../synchronization/conference-repository';
import RabbitPub from '../synchronization/rabbit-pub';
import { Conference } from './conference';
import { ConferenceManagerOptions } from './conference-manager';

const logger = new Logger('conference-factory');

export default async function conferenceFactory(
   id: string,
   workers: MediaSoupWorkers,
   repository: ConferenceRepository,
   rabbit: RabbitMqConn,
   { routerOptions, webrtcOptions, maxIncomingBitrate }: ConferenceManagerOptions,
): Promise<Conference> {
   logger.info('createConference() [conferenceId: %s]', id);

   // initialize conference info, subscribe messages
   await repository.getConference(id);

   // Create a mediasoup Router.
   const assignedWorker = workers.getNextWorker();
   const mediasoupRouter = await assignedWorker.createRouter(routerOptions);

   const messenger = new RabbitPub(rabbit);
   return new Conference(mediasoupRouter, id, messenger, repository, webrtcOptions, maxIncomingBitrate);
}
