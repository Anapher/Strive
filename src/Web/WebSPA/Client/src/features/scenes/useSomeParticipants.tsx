import _ from 'lodash';
import { useSelector } from 'react-redux';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectParticipantsOfRoomWebcamAvailable } from '../media/selectors';
import { selectParticipantsOfCurrentRoom } from '../rooms/selectors';
import { selectActiveParticipants } from './selectors';

export type UseParticipantsOptions = {
   /** only return active participants (participants that are speaking). Included participants are still included */
   activeOnly?: boolean;

   /** only return participants with active webcam. this also applied to included participants */
   webcamOnly?: boolean;

   /** participants that should always be included. Please note that filter properties also apply to them */
   includedParticipants?: Participant[];

   /** excluded participants */
   excludedParticipants?: string[];
};

export default function useSomeParticipants(
   { includedParticipants, excludedParticipants, webcamOnly, activeOnly }: UseParticipantsOptions = {},
   count?: number,
): Participant[] {
   const activeParticipants = useSelector(selectActiveParticipants);
   const allParticipants = useSelector(selectParticipants);
   const participantsInRoom = useSelector(selectParticipantsOfCurrentRoom);
   const participantsWithWebcam = useSelector(selectParticipantsOfRoomWebcamAvailable);

   const orderedActiveParticipantsOfCurrentRoom = _(Object.entries(activeParticipants))
      .filter(([id]) => participantsInRoom.includes(id))
      .orderBy(([, state]) => state.orderNumber)
      .map(([participantId]) => allParticipants[participantId])
      .filter((x): x is Participant => !!x)
      .value();

   let query = _(includedParticipants ?? [])
      .concat(orderedActiveParticipantsOfCurrentRoom)
      .concat(participantsInRoom.map((x) => allParticipants[x]).filter((x): x is Participant => !!x))
      .uniqBy((x) => x.id);

   if (activeOnly) {
      query = query.filter(
         (x) => !activeParticipants[x.id]?.inactive || !!includedParticipants?.find((y) => y.id === x.id),
      );
   }

   if (excludedParticipants) {
      query = query.filter((x) => !excludedParticipants?.includes(x.id));
   }

   if (webcamOnly) {
      query = query.filter((x) => participantsWithWebcam.includes(x.id));
   }

   if (count !== undefined) {
      query = query.slice(0, count);
   }

   return query.value();
}
