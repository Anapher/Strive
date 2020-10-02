import {
   DEFAULT_PREFIX,
   SIGNALR_ADDHANDLER,
   SIGNALR_CONNECTED,
   SIGNALR_DISCONNECT,
   SIGNALR_DISCONNECTED,
   SIGNALR_MESSAGE,
   SIGNALR_REMOVEHANDLER,
   SIGNALR_SEND,
   WEBSOCKET_ERROR,
} from './action-types';
import { Action } from './types';

// Action creators for user dispatched actions. These actions are all optionally
// prefixed.

/**
 * Disconnect the SignalR connection
 * @param prefix The prefix of the SignalR middleware this action is intend for
 */
export const disconnect = (prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${SIGNALR_DISCONNECT}`);

/**
 * Invokes a hub method on the server using the specified name and arguments.
 * @param methodName The name of the server method to invoke
 * @param args The arguments used to invoke the server method.
 * @param prefix The prefix of the SignalR middleware this action is intend for
 */
export const send = (methodName: any, args: any[], prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${SIGNALR_SEND}`, { methodName, args });

/**
 * Invokes a hub method on the server using the specified name and arguments. The action will have the default prefix.
 * @param methodName The name of the server method to invoke
 * @param args The arguments used to invoke the server method.
 */
export const sendDefault = (methodName: any, ...args: any[]) =>
   buildAction(`${DEFAULT_PREFIX}::${SIGNALR_SEND}`, { methodName, args });

/**
 * Registers a handler that will dispatch action when the hub method with the specified method name(s) is invoked.
 * @param methodNames The name of the method(s)
 * @param prefix The prefix of the SignalR middleware this action is intend for
 */
export const addHandler = (methodNames: string[] | string, prefix?: string) => {
   if (typeof methodNames === 'string') {
      methodNames = [methodNames];
   }
   return buildAction(`${prefix || DEFAULT_PREFIX}::${SIGNALR_ADDHANDLER}`, methodNames);
};

/**
 * Removes all handlers for the specified hub method(s).
 * @param methodNames The name of the method(s)
 * @param prefix The prefix of the SignalR middleware this action is intend for
 */
export const removeHandler = (methodNames: string[] | string, prefix?: string) => {
   if (typeof methodNames === 'string') {
      methodNames = [methodNames];
   }
   return buildAction(`${prefix || DEFAULT_PREFIX}::${SIGNALR_REMOVEHANDLER}`, methodNames);
};

// Action creators for actions dispatched by redux-signalr. All of these must
// take a prefix. The default prefix should be used unless a user has created
// this middleware with the prefix option set.

export const connected = (prefix: string) => buildAction(`${prefix}::${SIGNALR_CONNECTED}`);
export const disconnected = (prefix: string) => buildAction(`${prefix}::${SIGNALR_DISCONNECTED}`);
export const message = (prefix: string, name: string, payload: any) =>
   buildAction(`${prefix}::${SIGNALR_MESSAGE}/${name}`, payload);

export const error = (originalAction: Action | null, err: Error, prefix: string) =>
   buildAction(`${prefix}::${WEBSOCKET_ERROR}`, err, {
      message: err.message,
      name: err.name,
      originalAction,
   });

/**
 * Create an FSA compliant action.
 *
 * @param {string} actionType
 * @param {T} payload
 *
 * @returns {BuiltAction<T>}
 */
function buildAction<T>(actionType: string, payload?: T, meta?: any): BuiltAction<T> {
   const base = {
      type: actionType,
      meta: {
         timestamp: new Date(),
         ...meta,
      },
      // Mixin the `error` key if the payload is an Error.
      ...(payload instanceof Error ? { error: true } : null),
   };

   return payload ? { ...base, payload } : base;
}

type BuiltAction<T> = {
   type: string;
   meta: {
      timestamp: Date;
   };
   payload?: T;
};
