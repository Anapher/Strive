import { makeStyles, useTheme } from '@material-ui/core';
import Chip from '@material-ui/core/Chip';
import clsx from 'classnames';
import React from 'react';
import { useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import { Participant } from 'src/features/conference/types';
import { selectParticipantMicActivated } from 'src/features/media/selectors';
import { ParticipantAudioInfo } from 'src/features/media/types';
import { RootState } from 'src/store';

const useStyles = makeStyles((theme) => ({
   chip: {
      padding: theme.spacing(0, 1),
      minWidth: 96,
      transition: 'border 200ms ease-out',
      alignItems: 'left',
   },
   chipSpeaking: {
      borderColor: theme.palette.primary.main,
   },
   chipMicDeactivated: {
      borderColor: theme.palette.error.main,
   },
   chipLabel: {
      flex: 1,
      textAlign: 'center',
   },
}));

type Props = {
   participantId: string;
   participant?: Participant;
   audioInfo?: ParticipantAudioInfo;
};

export default function ParticipantInfoChip({ participantId, participant, audioInfo }: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const micActivated = useSelector((state: RootState) => selectParticipantMicActivated(state, participantId));

   return (
      <Chip
         size="small"
         className={clsx(classes.chip, {
            [classes.chipSpeaking]: audioInfo?.speaking,
            [classes.chipMicDeactivated]: !micActivated,
         })}
         classes={{ label: classes.chipLabel }}
         label={participant?.displayName ?? participantId}
         variant="outlined"
         icon={<AnimatedMicIcon activated={micActivated} disabledColor={theme.palette.error.main} />}
      />
   );
}
