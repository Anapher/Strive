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
   count: number,
   { includedParticipants, excludedParticipants, webcamOnly, activeOnly }: UseParticipantsOptions = {},
): Participant[] {
   const activeParticipants = useSelector(selectActiveParticipants);
   const allParticipants = useSelector(selectParticipants);
   const participantsInRoom = useSelector(selectParticipantsOfCurrentRoom);
   const participantsWithWebcam = useSelector(selectParticipantsOfRoomWebcamAvailable);

   const orderedActiveParticipants = _.orderBy(Object.entries(activeParticipants), (x) => x[1].orderNumber)
      .map(([participantId]) => allParticipants[participantId])
      .filter((x): x is Participant => !!x);

   return _(includedParticipants ?? [])
      .concat(orderedActiveParticipants)
      .concat(participantsInRoom.map((x) => allParticipants[x]).filter((x): x is Participant => !!x))
      .uniqBy((x) => x.id)
      .filter((x) => !activeOnly || !!activeParticipants[x.id] || includedParticipants?.find((y) => y.id === x.id))
      .filter((x) => !excludedParticipants?.includes(x.id))
      .filter((x) => !webcamOnly || participantsWithWebcam.includes(x.id))
      .slice(0, count)
      .value();
}
