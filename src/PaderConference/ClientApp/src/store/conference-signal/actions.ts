import { ActionCreatorWithPayload, createAction } from '@reduxjs/toolkit';
import { IRestError } from 'src/utils/error-result';

export const DEFAULT_PREFIX = 'SIGNALR';

// Action creators for user dispatched actions. These actions are all optionally
// prefixed.

export const joinConference = createAction(`${DEFAULT_PREFIX}::JOIN_CONFERENCE`, (conferenceId: string) => ({
   payload: { conferenceId },
}));

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

export const onConferenceJoined = createAction(`${DEFAULT_PREFIX}::ON_CONFERENCE_JOINED`, (conferenceId: string) => ({
   payload: { conferenceId },
}));

export const onConferenceJoinError = createAction(
   `${DEFAULT_PREFIX}::ON_CONFERENCE_JOIN_ERROR`,
   (error?: IRestError) => ({
      payload: error,
   }),
);

export const onConferenceConnectionClosed = createAction(
   `${DEFAULT_PREFIX}::ON_CONFERENCE_CONNECTION_CLOSED`,
   (conferenceId: string, error?: Error) => ({
      payload: { conferenceId, error },
   }),
);

export const onConferenceReconnecting = createAction(
   `${DEFAULT_PREFIX}::ON_CONFERENCE_RECONNECTING`,
   (conferenceId: string, error?: Error) => ({
      payload: { conferenceId, error },
   }),
);

export const onConferenceReconnected = createAction(
   `${DEFAULT_PREFIX}::ON_CONFERENCE_RECONNECTED`,
   (conferenceId: string, error?: Error) => ({
      payload: { conferenceId, error },
   }),
);

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
