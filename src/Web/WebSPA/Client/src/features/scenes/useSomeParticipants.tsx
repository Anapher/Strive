import _ from 'lodash';
import { useSelector } from 'react-redux';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectParticipantsOfCurrentRoom } from '../rooms/selectors';
import { selectActiveParticipants } from './selectors';

export default function useSomeParticipants(
   count: number,
   includedParticipants?: Participant[],
   excludedParticipants?: string[],
): Participant[] {
   const activeParticipants = useSelector(selectActiveParticipants);
   const allParticipants = useSelector(selectParticipants);
   const participantsInRoom = useSelector(selectParticipantsOfCurrentRoom);

   const orderedActiveParticipants = _.orderBy(Object.entries(activeParticipants), (x) => x[1].orderNumber)
      .map(([participantId]) => allParticipants[participantId])
      .filter((x): x is Participant => !!x);

   return _(includedParticipants ?? [])
      .concat(orderedActiveParticipants)
      .concat(participantsInRoom.map((x) => allParticipants[x]).filter((x): x is Participant => !!x))
      .uniqBy((x) => x.id)
      .filter((x) => !excludedParticipants?.includes(x.id))
      .slice(0, count)
      .value();
}
