import express from 'express';
import { createLightship } from 'lightship';
import config from './config';
import configureEndpoints from './controllers';
import ConferenceManager from './lib/conference/conference-manager';
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
   await rabbitConn.getChannel();
   rabbitConn.on('error', async () => {
      try {
         await rabbitConn.getChannel();
      } catch (error) {
         lightship.shutdown();
      }
   });

   const workers = new MediaSoupWorkers();
   await workers.run(config.mediasoup.numWorkers, config.mediasoup.workerSettings);

   const client = new ConferenceManagementClient(config.services.conferenceManagementUrl);

   const conferenceManager = new ConferenceManager(
      rabbitConn,
      workers,
      client,
      config.router,
      config.webRtcTransport.options,
      config.webRtcTransport.maxIncomingBitrate,
   );

   const app = express();
   configureEndpoints(app, conferenceManager);

   const server = app
      .listen(config.http.port, () => {
         lightship.signalReady();
         logger.info('HTTP server is listening on port %i', config.http.port);
      })
      .on('error', () => lightship.shutdown());

   lightship.registerShutdownHandler(async () => {
      // Allow sufficient amount of time to allow all of the existing
      // HTTP requests to finish before terminating the service.
      const minute = 60 * 1000;
      await sleep(minute);
      server.close();

      workers.close();
   });

   return app;
}
