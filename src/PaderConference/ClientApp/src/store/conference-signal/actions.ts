import {
   CONFERENCE_JOIN,
   ON_CONFERENCE_JOINED,
   DEFAULT_PREFIX,
   ON_CONFERENCE_JOIN_FAILED,
   ON_CONFERENCE_CONNECTION_CLOSED,
   ON_CONFERENCE_RECONNECTING,
   ON_CONFERENCE_RECONNECTED,
   SUBSCRIBE_EVENT,
   CLOSE,
   SEND,
} from './action-types';

// Action creators for user dispatched actions. These actions are all optionally
// prefixed.

/**
 * Join a conference
 * @param conferenceId The conference that should be joined
 * @param prefix The prefix of the SignalR middleware this action is intend for
 */
export const joinConference = (conferenceId: string, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${CONFERENCE_JOIN}`, { conferenceId });

export const subscribeEvent = (name: string, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${SUBSCRIBE_EVENT}`, { name });

export const send = (name: string, payload?: any, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${SEND}`, { name, payload });

export const close = (prefix?: string) => buildAction(`${prefix || DEFAULT_PREFIX}::${CLOSE}`);

// Events

export const onConferenceJoined = (conferenceId: string, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${ON_CONFERENCE_JOINED}`, { conferenceId });

export const onConferenceJoinFailed = (conferenceId: string, error?: Error, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${ON_CONFERENCE_JOIN_FAILED}`, { conferenceId, error });

export const onConferenceConnectionClosed = (conferenceId: string, error?: Error, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${ON_CONFERENCE_CONNECTION_CLOSED}`, { conferenceId, error });

export const onConferenceReconnecting = (conferenceId: string, error?: Error, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${ON_CONFERENCE_RECONNECTING}`, { conferenceId, error });

export const onConferenceReconnected = (conferenceId: string, error?: Error, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::${ON_CONFERENCE_RECONNECTED}`, { conferenceId, error });

export const onEventOccurred = (name: string, payload: any, prefix?: string) =>
   buildAction(`${prefix || DEFAULT_PREFIX}::ON_${name}`, payload);

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
