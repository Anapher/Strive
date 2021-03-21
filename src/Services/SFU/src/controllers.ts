import express, { Express, Request } from 'express';
import * as errors from './errors';
import ConferenceManager from './lib/conference/conference-manager';
import Logger from './utils/logger';
import jwt from 'express-jwt';
import config from './config';

const logger = new Logger('Controllers');

type JwtProperties = { sub: string; conference: string; connection: string };
type RequestInfo = { participantId: string; conferenceId: string; connectionId: string };

export default function configureEndpoints(app: Express, conferenceManager: ConferenceManager): void {
   app.use(express.json());
   app.use(jwt({ algorithms: ['HS256'], secret: config.services.tokenSecret }));
   app.use((req, res, next) => {
      const conferenceId: string = req.params.conferenceId;
      const tokenConference = (req.user as JwtProperties).conference;

      if (conferenceId !== tokenConference) {
         res.status(400).end();
         return;
      }

      next();
   });

   const getJwtProps = (req: Request): RequestInfo => {
      const jwt = req.user as JwtProperties;
      return { conferenceId: jwt.conference, participantId: jwt.sub, connectionId: jwt.connection };
   };

   app.post('/:conferenceId/init-connection', async (req, res) => {
      const { conferenceId, connectionId, participantId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      logger.debug('Initialize connection in conference %s', conference.conferenceId);

      await conference.addConnection({
         ...req.body,
         connectionId,
         participantId,
      });
      res.status(200).end();
   });

   app.get('/:conferenceId/router-capabilities', async (req, res) => {
      const { conferenceId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      res.json(conference.routerCapabilities);
   });

   app.post('/:conferenceId/create-transport', async (req, res) => {
      const { conferenceId, connectionId, participantId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.createTransport(req.body, connectionId);
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json(result.response);
   });

   app.post('/:conferenceId/connect-transport', async (req, res) => {
      const { conferenceId, connectionId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.connectTransport(req.body, connectionId);
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.status(200).end();
   });

   app.post('/:conferenceId/transport-produce', async (req, res) => {
      const { conferenceId, connectionId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.transportProduce(req.body, connectionId);
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.json(result.response);
   });

   app.post('/:conferenceId/change-stream', async (req, res) => {
      const { conferenceId, connectionId } = getJwtProps(req);

      const conference = await conferenceManager.getConference(conferenceId);
      if (!conference) {
         res.status(404).json(errors.conferenceNotFound(conferenceId));
         return;
      }

      const result = await conference.changeStream(req.body, connectionId);
      if (!result.success) {
         res.status(400).json(result.error);
         return;
      }

      res.status(200).end();
   });
}
