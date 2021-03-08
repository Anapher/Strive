import { useTheme } from '@material-ui/core';
import Chip from '@material-ui/core/Chip';
import React from 'react';
import { useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import { Participant } from 'src/features/conference/types';
import { selectParticipantProducers } from 'src/features/media/selectors';
import { ParticipantAudioInfo } from 'src/features/media/types';
import { RootState } from 'src/store';

type Props = {
   participantId: string;
   participant?: Participant;
   audioInfo?: ParticipantAudioInfo;
};

export default function ParticipantInfoChip({ participantId, participant, audioInfo }: Props) {
   const theme = useTheme();
   const producers = useSelector((state: RootState) => selectParticipantProducers(state, participantId));
   const micActivated = producers?.mic?.paused === false;

   return (
      <Chip
         size="small"
         style={{
            paddingLeft: 8,
            paddingRight: 8,
            borderColor: micActivated
               ? audioInfo?.speaking
                  ? theme.palette.primary.main
                  : undefined
               : theme.palette.error.main,
            transition: 'border 200ms ease-out',
         }}
         label={participant?.displayName ?? participantId}
         variant="outlined"
         icon={<AnimatedMicIcon activated={micActivated} disabledColor={theme.palette.error.main} />}
      />
   );
}
