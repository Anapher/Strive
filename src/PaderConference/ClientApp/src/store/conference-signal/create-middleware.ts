import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Middleware, MiddlewareAPI } from 'redux';
import * as actions from './actions';
import { Options } from './types';

export default (options: Options): Middleware => {
   const { url } = options;

   let connection: HubConnection | undefined;

   // Define the list of handlers, now that we have an instance of ReduxWebSocket.
   const handlers: {
      [s: string]: (middleware: MiddlewareAPI, action: any) => void;
   } = {
      [actions.joinConference.type]: async ({ dispatch, getState }, action) => {
         const conferenceId: string = action.payload.conferenceId;

         if (!connection) {
            const conferenceUrl =
               url + `?access_token=${options.getAccessToken(getState())}&conferenceId=${conferenceId}`;

            connection = new HubConnectionBuilder()
               .withUrl(conferenceUrl)
               .withAutomaticReconnect()
               .configureLogging(LogLevel.Information)
               .build();

            try {
               await connection.start();
               connection.onclose((error) => dispatch(actions.onConferenceConnectionClosed(conferenceId, error)));
               connection.onreconnecting((error) => dispatch(actions.onConferenceReconnected(conferenceId, error)));
               connection.onreconnecting((error) => dispatch(actions.onConferenceReconnecting(conferenceId, error)));

               dispatch(actions.onConferenceJoined(conferenceId));
            } catch (error) {
               dispatch(actions.onConferenceJoinFailed(conferenceId, error));
            }
         }
      },
      [actions.subscribeEvent.type]: ({ dispatch }, { name }) => {
         if (connection) {
            connection.on(name, (args) => dispatch(actions.onEventOccurred(name)(args.length === 1 ? args[0] : args)));
         }
      },
      [actions.send.type]: (_, { name, payload }) => {
         if (connection) {
            connection.send(name, ...payload);
         }
      },
      [actions.close.type]: async () => {
         if (connection) {
            await connection.stop();
            connection = undefined;
         }
      },
   };

   // Middleware function.
   return (store: MiddlewareAPI) => (next) => (action: any) => {
      const { type: actionType } = action;

      // Check if action type matches prefix
      if (actionType) {
         const handler = handlers[actionType];

         if (handler) {
            try {
               handler(store, action);
            } catch (err) {
               console.error(err);
            }
         }
      }

      return next(action);
   };
};
