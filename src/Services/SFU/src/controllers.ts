import express, { Express } from 'express';
import * as errors from './errors';
import ConferenceManager from './lib/conference/conference-manager';
import Logger from './utils/logger';

const logger = new Logger('Controllers');

export default function configureEndpoints(app: Express, conferenceManager: ConferenceManager): void {
   app.use(express.json());

   app.post('/:conferenceId/init-connection', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      logger.debug('Initialize connection in conference %s', conference.conferenceId);

      await conference.addConnection(req.body);
      res.status(200).end();
   });

   app.get('/:conferenceId/router-capabilities', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      res.json(conference.routerCapabilities);
   });

   app.post('/:conferenceId/create-transport', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.createTransport(req.body, 'TODO: Connection id');
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json(result.response);
   });

   app.post('/:conferenceId/connect-transport', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.connectTransport(req.body, 'TODO: Connection id');
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.status(200).end();
   });

   app.post('/:conferenceId/transport-produce', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.transportProduce(req.body, 'TODO: Connection id');
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json(result.response);
   });

   app.post('/:conferenceId/change-stream', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.changeStream(req.body, 'TODO: Connection id');
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json();
   });

   app.post('/:conferenceId/change-producer-source', async (req, res) => {
      const conferenceId: string = req.params.conferenceId;

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.changeProducerSource(req.body, 'TODO: Connection id');
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json();
   });
}
