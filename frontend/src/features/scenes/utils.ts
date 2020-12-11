import { DateTime } from 'luxon';
import { ActiveParticipants } from './types';

const PARTICIPANT_REMOVAL_SLIDING_SECONDS = 5;

export type ActiveParticipantsChanged = {
   newParticipants: string[];
   updatedParticipants: ActiveParticipants;
   removedParticipants: string[];
};

export function generateActiveParticipantsPatch(
   applied: ActiveParticipants,
   current: string[],
): ActiveParticipantsChanged {
   const now = DateTime.utc();

   return {
      newParticipants: current.filter((x) => !applied[x]),
      updatedParticipants: Object.fromEntries([
         // participants that were marked as deleted but are speaking now
         ...Object.entries(applied)
            .filter(([participantId, state]) => state.deletedOn && current.includes(participantId))
            .map(([participantId]) => [participantId, {}]),
         // participants that are not marked as deleted and are not speaking now
         ...Object.entries(applied)
            .filter(([participantId, state]) => !state.deletedOn && !current.includes(participantId))
            .map(([participantId]) => [participantId, { deletedOn: now.toISO() }]),
      ]),
      // participants who's sliding duration timed out
      removedParticipants: Object.entries(applied)
         .filter(
            ([, state]) =>
               state.deletedOn &&
               now > DateTime.fromISO(state.deletedOn).plus({ seconds: PARTICIPANT_REMOVAL_SLIDING_SECONDS }),
         )
         .map(([participantId]) => participantId),
   };
}

export function applyPatch(update: ActiveParticipantsChanged, applied: ActiveParticipants): ActiveParticipants {
   return Object.fromEntries(
      Object.entries({
         ...applied,
         ...update.updatedParticipants,
         ...Object.fromEntries(update.newParticipants.map((x) => [x, {}])),
      }).filter(([participantId]) => !update.removedParticipants.includes(participantId)),
   );
}
