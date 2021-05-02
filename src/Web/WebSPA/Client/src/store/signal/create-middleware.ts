import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { PayloadAction } from '@reduxjs/toolkit';
import { Middleware, MiddlewareAPI } from 'redux';
import { events } from 'src/core-hub';
import * as errors from 'src/errors';
import { signalrError } from 'src/ui-errors';
import * as actions from './actions';
import appHubConn from './app-hub-connection';
import debug from 'debug';

type SignalRResult = {
   middleware: Middleware;
   getConnection: () => HubConnection | undefined;
};

const log = debug('signalr:middleware');
const logLibrary = debug('signalr:library');

export default (): SignalRResult => {
   let connection: HubConnection | undefined;
   let subscribedEvents = new Array<string>();

   // Define the list of handlers, now that we have an instance of ReduxWebSocket.
   const handlers: {
      [s: string]: (middleware: MiddlewareAPI, action: any) => void;
   } = {
      [actions.connectSignal.type]: async (
         { dispatch },
         { payload: { signalUrl, appData, urlParams, defaultEvents } },
      ) => {
         if (!connection) {
            const queryString = Object.keys(urlParams)
               .map((key) => key + '=' + urlParams[key])
               .join('&');

            const url = signalUrl + '?' + queryString;

            connection = new HubConnectionBuilder()
               .withUrl(url)
               .withAutomaticReconnect({ nextRetryDelayInMilliseconds: () => 5000 })
               .configureLogging({ log: (level, message) => logLibrary('[%s]: %s', LogLevel[level], message) })
               .build();

            connection.on(events.onConnectionError, (err) => {
               log('onConnectionError() | %O', err);

               dispatch(actions.onConnectionError(err));
               dispatch(actions.close());
            });

            subscribedEvents = [...defaultEvents];
            for (const eventName of defaultEvents) {
               log('subscribe event %s', eventName);
               connection.on(eventName, (args) => dispatch(actions.onEventOccurred(eventName)(args)));
            }

            try {
               log('connect to %s', url);
               await connection.start();
               connection.onclose((error) => {
                  log('connection | onclose %O', error);

                  connection?.stop();
                  connection = undefined;
                  appHubConn.remove();

                  dispatch(actions.onConnectionClosed(appData, error));
               });
               connection.onreconnected(() => {
                  log('connection | onreconnected');
                  dispatch(actions.onReconnected(appData));
               });
               connection.onreconnecting((error) => {
                  log('connection | onreconnecting');
                  dispatch(actions.onReconnecting(appData, error));
               });

               appHubConn.register(connection);

               log('connected successfully');
               dispatch(actions.onConnected(appData));
            } catch (error) {
               log('an error occurred on connecting %O', error);

               dispatch(actions.onConnectionError(errors.signalRConnectionUnavailable(error.toString())));

               await connection.stop();
               connection = undefined;
            }
         }
      },
      [actions.subscribeEvent.type]: ({ dispatch }, { payload: { name } }) => {
         if (connection && !subscribedEvents.includes(name)) {
            log('subscribe event %s', name);
            subscribedEvents.push(name);
            connection.on(name, (args) => dispatch(actions.onEventOccurred(name)(args)));
         }
      },
      [actions.close.type]: async () => {
         if (connection) {
            log('request close connection');

            await connection.stop();
            connection = undefined;
            appHubConn.remove();
         }
      },
   };

   const invokeMethodHandler = ({ dispatch }: MiddlewareAPI, { payload: { payload, name } }: any) => {
      if (connection) {
         log('Invoke method %s with payload %O', name, payload);
         connection.invoke(name, ...(payload !== undefined ? [payload] : [])).then(
            (returnVal) => {
               dispatch(actions.onInvokeReturn(name)(returnVal, payload));
            },
            (error) => dispatch(actions.onInvokeReturn(name)({ success: false, error: signalrError(error) }, payload)),
         );
      }
   };

   // Middleware function.
   const middleware: Middleware = (store: MiddlewareAPI) => (next) => (action: PayloadAction) => {
      const { type } = action;

      try {
         if (type.startsWith(actions.invokePrefix)) {
            invokeMethodHandler(store, action);
         } else {
            const handler = handlers[type];
            if (handler) handler(store, action);
         }
      } catch (err) {
         log('Error on executing middleware function %O', err);
      }

      return next(action);
   };

   const getConnection = () => connection;

   return { middleware, getConnection };
};
