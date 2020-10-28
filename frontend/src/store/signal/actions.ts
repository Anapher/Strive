import { ActionCreatorWithPayload, createAction } from '@reduxjs/toolkit';
import { IRestError } from 'src/utils/error-result';

export const DEFAULT_PREFIX = 'SIGNALR';

// Action creators for user dispatched actions. These actions are all optionally
// prefixed.

export const connectSignal = createAction(
   `${DEFAULT_PREFIX}::CONNECT`,
   (urlParams: any, defaultEvents: string[], appData?: any) => ({
      payload: { appData, urlParams, defaultEvents },
   }),
);

export const subscribeEvent = createAction(`${DEFAULT_PREFIX}::SUBSCRIBE_EVENT`, (name: string) => ({
   payload: { name },
}));

export const send = createAction(`${DEFAULT_PREFIX}::SEND`, (name: string, payload?: any) => ({
   payload: { name, payload },
}));

export const invoke = createAction(`${DEFAULT_PREFIX}::INVOKE`, (name: string, payload?: any) => ({
   payload: { name, payload },
}));

export const close = createAction(`${DEFAULT_PREFIX}::CLOSE`);

// Events

export const onConnected = createAction(`${DEFAULT_PREFIX}::ON_CONFERENCE_JOINED`, (appData?: any) => ({
   payload: { appData },
}));

export const onConnectionError = createAction(
   `${DEFAULT_PREFIX}::ON_CONFERENCE_JOIN_ERROR`,
   (error?: IRestError, appData?: any) => ({
      payload: { error, appData },
   }),
);

export const onConnectionClosed = createAction(
   `${DEFAULT_PREFIX}::ON_CONNECTION_CLOSED`,
   (appData?: any, error?: Error) => ({
      payload: { appData, error },
   }),
);

export const onReconnecting = createAction(`${DEFAULT_PREFIX}::ON_RECONNECTING`, (appData: any, error?: Error) => ({
   payload: { appData, error },
}));

export const onReconnected = createAction(`${DEFAULT_PREFIX}::ON_RECONNECTED`, (appData: any, error?: Error) => ({
   payload: { appData, error },
}));

export function onEventOccurred<T>(eventName: string): ActionCreatorWithPayload<T, string> {
   return createAction(`${DEFAULT_PREFIX}::ON_${eventName}`, (payload: T) => ({
      payload,
   }));
}

export function onInvokeReturn<T>(methodName: string): ActionCreatorWithPayload<T, string> {
   return createAction(`${DEFAULT_PREFIX}::ONINVOKE_${methodName}`, (payload: T) => ({
      payload,
   }));
}

export function onInvokeFailed<T>(methodName: string): ActionCreatorWithPayload<T, string> {
   return createAction(`${DEFAULT_PREFIX}::ONINVOKE_FAILED_${methodName}`, (payload: T) => ({
      payload,
   }));
}
