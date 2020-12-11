import { Chip } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectParticipantAudio } from 'src/features/media/selectors';

type Props = {
   participantIds: string[];
};

export default function ParticipantChips({ participantIds }: Props) {
   const participants = useSelector(selectParticipants);
   const audioInfo = useSelector(selectParticipantAudio);

   return (
      <div>
         {participantIds.map((participantId) => (
            <Chip
               key={participantId}
               label={participants?.find((x) => x.participantId === participantId)?.displayName ?? participantId}
               variant="outlined"
               icon={<AnimatedMicIcon activated={audioInfo[participantId]?.speaking === true} />}
            />
         ))}
      </div>
   );
}
