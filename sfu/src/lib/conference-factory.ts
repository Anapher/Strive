import { SignalWrapper } from './signal-wrapper';
import { Redis } from 'ioredis';
import { AudioLevelObserverVolume, Worker } from 'mediasoup/lib/types';
import config from '../config';
import { Conference } from './conference';
import Logger from './logger';
import { ConferenceInfo } from './types';
import { channels } from './pader-conference/redis-channels';

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

   // configure audio level observer
   const audioLevelObserver = await mediasoupRouter.createAudioLevelObserver({
      maxEntries: 1000000,
      threshold: -80,
      interval: 1000,
   });

   audioLevelObserver.on('volumes', (volumes: AudioLevelObserverVolume[]) => {
      logger.debug('on volumes');
      redis.publish(
         channels.audioObserver.getName(conferenceInfo.id),
         JSON.stringify(volumes.map((x) => ({ volume: x.volume, participantId: x.producer.appData.participantId }))),
      );
   });
   audioLevelObserver.on('silence', () => {
      logger.debug('on silence');
      redis.publish(channels.audioObserver.getName(conferenceInfo.id), JSON.stringify([]));
   });

   return new Conference(mediasoupRouter, conferenceInfo.id, wrapper, redis, audioLevelObserver);
}
