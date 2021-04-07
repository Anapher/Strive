import express from 'express';
import { createLightship } from 'lightship';
import config from './config';
import configureEndpoints from './controllers';
import ConferenceManager, { ConferenceManagerOptions } from './lib/conference/conference-manager';
import { ConferenceManagementClient } from './lib/synchronization/conference-management-client';
import RabbitMqConn from './rabbitmq/rabbit-mq-conn';
import MediaSoupWorkers from './media-soup-workers';
import Logger from './utils/logger';
import { sleep } from './utils/promise-utils';

const logger = new Logger();
logger.info('Starting server...');

main();

async function main() {
   const lightship = createLightship({ detectKubernetes: false });

   const rabbitConn = new RabbitMqConn(config.services.rabbitMq);

   // try to connect
   try {
      await rabbitConn.getChannel();
   } catch (error) {
      logger.error('Connection to rabbit mq failed, shutdown');
      process.exit(1);
   }

   logger.info('Connection to rabbit mq established successfully');

   rabbitConn.on('fatal', async () => {
      await lightship.shutdown();
   });

   const workers = new MediaSoupWorkers();
   await workers.run(config.mediasoup.numWorkers, config.mediasoup.workerSettings);

   const client = new ConferenceManagementClient(config.services.conferenceInfoRequestUrl);

   const conferenceManagerOptions: ConferenceManagerOptions = {
      routerOptions: config.router,
      webrtcOptions: config.webRtcTransport.options,
      maxIncomingBitrate: config.webRtcTransport.maxIncomingBitrate,
   };

   const conferenceManager = new ConferenceManager(rabbitConn, workers, client, conferenceManagerOptions);

   const app = express();
   configureEndpoints(app, conferenceManager);

   const server = app
      .listen(config.http.port, () => {
         lightship.signalReady();
         logger.info('HTTP server is listening on port %i', config.http.port);
      })
      .on('error', () => lightship.shutdown());

   lightship.registerShutdownHandler(async () => {
      if (process.env.SERVER_ENVIRONMENT !== 'Development') {
         // Allow sufficient amount of time to allow all of the existing
         // HTTP requests to finish before terminating the service.
         const minute = 60 * 1000;
         await sleep(minute);
      }

      server.close();
      workers.close();
   });

   return app;
}
