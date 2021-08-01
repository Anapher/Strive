import _ from 'lodash';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectActiveParticipants } from '../selectors';
import ParticipantChips from './ParticipantChips';

type Props = {
   className: string;
};

export default function ActiveParticipantsChips({ className }: Props) {
   const activeParticipants = useSelector(selectActiveParticipants);
   return (
      <ParticipantChips
         className={className}
         participantIds={_(Object.entries(activeParticipants))
            .filter(([, state]) => !state.inactive)
            .orderBy(([, state]) => state.orderNumber)
            .map(([id]) => id)
            .value()}
      />
   );
}
