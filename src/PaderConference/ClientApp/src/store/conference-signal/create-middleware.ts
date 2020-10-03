import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Middleware, MiddlewareAPI } from 'redux';
import * as actionTypes from './action-types';
import * as actions from './actions';
import { Options } from './types';

export default (options: Options): Middleware => {
   const { prefix, url } = options;
   const actionPrefixExp = RegExp(`^${prefix}::`);

   let connection: HubConnection | undefined;

   // Define the list of handlers, now that we have an instance of ReduxWebSocket.
   const handlers: {
      [s: string]: (middleware: MiddlewareAPI, action: any) => void;
   } = {
      [actionTypes.CONFERENCE_JOIN]: async ({ dispatch }, action) => {
         const conferenceId: string = action.conferenceId;

         if (!connection) {
            const conferenceUrl = new URL(url);
            conferenceUrl.searchParams.append('conferenceId', conferenceId);

            connection = new HubConnectionBuilder()
               .withUrl(conferenceUrl.toString())
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
      [actionTypes.SUBSCRIBE_EVENT]: ({ dispatch }, { name }) => {
         if (connection) {
            connection.on(name, (args) => dispatch(actions.onEventOccurred(name, args.length === 1 ? args[0] : args)));
         }
      },
      [actionTypes.SEND]: (_, { name, payload }) => {
         if (connection) {
            connection.send(name, ...payload);
         }
      },
      [actionTypes.CLOSE]: async () => {
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
      if (actionType && actionType.match(actionPrefixExp)) {
         const baseActionType = action.type.replace(actionPrefixExp, '');
         const handler = handlers[baseActionType];

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
