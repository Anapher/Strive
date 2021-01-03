import { ActionCreatorWithPayload, createAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';

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

export const invokePrefix = `${DEFAULT_PREFIX}::INVOKE_`;

export const invoke = (name: string) =>
   createAction(invokePrefix + name, (payload?: any) => ({
      payload: { name, payload },
   }));

export const close = createAction(`${DEFAULT_PREFIX}::CLOSE`);

// Events

export const onConnected = createAction(`${DEFAULT_PREFIX}::ON_CONNECTED`, (appData?: any) => ({
   payload: { appData },
}));

export const onConnectionError = createAction(
   `${DEFAULT_PREFIX}::ON_CONNECTION_ERROR`,
   (error?: DomainError, appData?: any) => ({
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

export function onInvokeReturn<T>(methodName: string): ActionCreatorWithPayload<SuccessOrError<T>, string> {
   return createAction(`${DEFAULT_PREFIX}::RETURN_${methodName}`, (payload: SuccessOrError<T>) => ({
      payload,
   }));
}
