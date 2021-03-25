import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { PayloadAction } from '@reduxjs/toolkit';
import { Middleware, MiddlewareAPI } from 'redux';
import { events } from 'src/core-hub';
import * as errors from 'src/errors';
import { signalrError } from 'src/ui-errors';
import * as actions from './actions';
import appHubConn from './app-hub-connection';

type SignalRResult = {
   middleware: Middleware;
   getConnection: () => HubConnection | undefined;
};

export default (): SignalRResult => {
   let connection: HubConnection | undefined;

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
               .withAutomaticReconnect()
               .configureLogging(LogLevel.Information)
               .build();

            connection.on(events.onConnectionError, (err) => {
               dispatch(actions.onConnectionError(err));
               dispatch(actions.close());
            });

            for (const eventName of defaultEvents) {
               connection.on(eventName, (args) => dispatch(actions.onEventOccurred(eventName)(args)));
            }

            try {
               await connection.start();
               connection.onclose((error) => {
                  connection = undefined;
                  appHubConn.remove();

                  dispatch(actions.onConnectionClosed(appData, error));
               });
               connection.onreconnected(() => dispatch(actions.onReconnected(appData)));
               connection.onreconnecting((error) => dispatch(actions.onReconnecting(appData, error)));

               appHubConn.register(connection);

               dispatch(actions.onConnected(appData));
            } catch (error) {
               dispatch(actions.onConnectionError(errors.signalRConnectionUnavailable(error.toString())));

               await connection.stop();
               connection = undefined;
            }
         }
      },
      [actions.subscribeEvent.type]: ({ dispatch }, { payload: { name } }) => {
         if (connection) {
            console.log('subscribe ' + name);
            connection.on(name, (args) => dispatch(actions.onEventOccurred(name)(args)));
         }
      },
      [actions.close.type]: async () => {
         if (connection) {
            await connection.stop();
            connection = undefined;
            appHubConn.remove();
         }
      },
   };

   const invokeMethodHandler = ({ dispatch }: MiddlewareAPI, { payload: { payload, name } }: any) => {
      if (connection) {
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
         console.error(err);
      }

      return next(action);
   };

   const getConnection = () => connection;

   return { middleware, getConnection };
};
