import {
   HubConnection,
   HubConnectionBuilder,
   IHttpConnectionOptions,
   LogLevel,
} from '@aspnet/signalr';
import { Dispatch, MiddlewareAPI } from 'redux';
import * as actions from './actions';
import { Action } from './types';

const reconnectIntervalMs = 5000;
const timeoutSignalR = 60000;

interface ReduxSignalROptions {
   prefix: string;
   url: string;
   onOpen?: (hub: HubConnection) => void;
   getOptions?: (getState: () => any) => IHttpConnectionOptions;
}

/**
 * SignalR client manager that reconnected automatically and only connects if handlers are active
 */
export default class ReduxSignalR {
   private connection: HubConnection | null = null;

   /** if SignalR is currently connecting, this variable contains a Promise that will resolve once the connection is established */
   private connectingPromise: Promise<void> | null = null;

   /** the counter for event handlers. if this reaches zero, SignalR is disconnected */
   private subscriptionsCount = 0;

   /** set to true if the SignalR close was intended */
   private intendedClose = false;

   /** this counts the calls to methods of this class */
   private callCounter = 0;

   constructor(private options: ReduxSignalROptions) {}

   public send = (store: MiddlewareAPI, action: Action) => {
      this.callCounter++;

      if (this.connection === null) {
         this.connect(store).then(() => this.send(store, action));
         return;
      }

      const { payload } = action;
      this.connection.send(payload.methodName, ...payload.args);
      this.verifyConnectionNeeded(store);
   };

   public addHandler = (store: MiddlewareAPI, action: Action) => {
      this.callCounter++;

      if (this.connection === null) {
         this.connect(store).then(() => this.addHandler(store, action));
         return;
      }

      const { prefix } = this.options;
      const methodNames: string[] = action.payload;

      for (const methodName of methodNames) {
         this.connection.on(methodName, (...args) =>
            this.handleMessage(store.dispatch, prefix, methodName, args),
         );

         this.subscriptionsCount++;
      }
   };

   public removeHandler = (store: MiddlewareAPI, { payload }: Action) => {
      this.callCounter++;

      if (this.connection === null) {
         throw new Error('SignalR is not inialized.');
      }

      const methodNames: string[] = payload;
      for (const methodName of methodNames) {
         this.connection.off(methodName);

         this.subscriptionsCount--;
      }

      this.verifyConnectionNeeded(store);
   };

   public disconnect = () => {
      if (this.connection === null) {
         throw new Error('SignalR is not inialized.');
      }

      this.intendedClose = true;
      this.connection.stop();
   };

   private verifyConnectionNeeded = (store: MiddlewareAPI) => {
      if (this.connection === null) {
         return;
      }
      if (this.subscriptionsCount > 0) {
         return;
      }

      const currentCallCounter = this.callCounter;
      setTimeout(() => {
         if (this.callCounter === currentCallCounter) {
            // nothing happend in the time and we aren't subscribed to any handlers (because then the counter would be increased)
            this.disconnect();
         }
      }, timeoutSignalR);
   };

   private connect = async (store: MiddlewareAPI): Promise<void> => {
      if (this.connectingPromise !== null) {
         // SignalR is already connecting, we just have to wait...
         await this.connectingPromise;
         return;
      }

      let connected: () => void | undefined;
      this.connectingPromise = new Promise(resolve => (connected = resolve));

      try {
         const { dispatch, getState } = store;
         const { prefix, url, getOptions } = this.options;

         const connection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Trace)
            .withUrl(url, getOptions === undefined ? {} : getOptions(getState))
            .build();

         connection.onclose(() => {
            this.connection = null;
            store.dispatch(actions.disconnected(prefix));

            if (!this.intendedClose) {
               // reconnect automatically
               setTimeout(() => {
                  this.connect(store);
               }, reconnectIntervalMs);
            } else {
               this.intendedClose = false;
            }
         });

         while (true) {
            const promise = connection.start();
            promise.then(() => dispatch(actions.connected(prefix)));

            try {
               await promise;
            } catch (error) {
               await new Promise(resolve => setTimeout(resolve, reconnectIntervalMs));
               continue;
            }

            // we are connected
            this.connection = connection;
            break;
         }
      } finally {
         connected!(); // resolve connecting promise
         this.connectingPromise = null;
      }
   };

   /**
    * Handle a message event.
    *
    * @param {Dispatch} dispatch
    * @param {string} prefix
    * @param {MessageEvent} event
    */
   private handleMessage = (dispatch: Dispatch, prefix: string, name: string, args: any[]) => {
      dispatch(actions.message(prefix, name, args.length === 1 ? args[0] : args));
   };
}
