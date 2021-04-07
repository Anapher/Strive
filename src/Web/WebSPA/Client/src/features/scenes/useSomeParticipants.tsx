import _ from 'lodash';
import { useSelector } from 'react-redux';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectActiveParticipants } from './selectors';

export default function useSomeParticipants(
   count: number,
   includedParticipants?: Participant[],
   excludedParticipants?: string[],
): Participant[] {
   const activeParticipants = useSelector(selectActiveParticipants);
   const allParticipants = useSelector(selectParticipants);

   const orderedActiveParticipants = _.orderBy(Object.entries(activeParticipants), (x) => x[1].orderNumber)
      .map(([participantId]) => allParticipants.find((x) => x.id === participantId))
      .filter((x): x is Participant => !!x);

   return _(includedParticipants ?? [])
      .concat(orderedActiveParticipants)
      .concat(allParticipants)
      .uniqBy((x) => x.id)
      .filter((x) => !excludedParticipants?.includes(x.id))
      .slice(0, count)
      .value();
}
