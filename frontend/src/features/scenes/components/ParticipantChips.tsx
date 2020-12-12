import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectParticipantAudio } from 'src/features/media/selectors';
import ParticipantInfoChip from './ParticipantInfoChip';

const useStyles = makeStyles(() => ({
   root: {
      display: 'flex',
      justifyContent: 'flex-end',
      width: '100%',
   },
}));

type Props = {
   participantIds: string[];
   className?: string;
};

export default function ParticipantChips({ participantIds, className }: Props) {
   const participants = useSelector(selectParticipants);
   const audioInfo = useSelector(selectParticipantAudio);
   const classes = useStyles();

   return (
      <div className={clsx(className, classes.root)}>
         {participantIds.map((participantId) => (
            <ParticipantInfoChip
               participantId={participantId}
               key={participantId}
               audioInfo={audioInfo[participantId]}
               participantDto={participants?.find((x) => x.participantId === participantId)}
            />
         ))}
      </div>
   );
}
