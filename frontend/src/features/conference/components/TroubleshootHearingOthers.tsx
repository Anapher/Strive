import { Chip, makeStyles } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipantAudio } from 'src/features/media/selectors';
import { selectParticipantsOfCurrentRoomWithoutMe } from 'src/features/rooms/selectors';
import TroubleshootAccordion from './TroubleshootAccordion';
import clsx from 'classnames';

const useStyles = makeStyles((theme) => ({
   statusChip: {
      marginRight: theme.spacing(2),
   },
   statusChipOk: {
      backgroundColor: '#27ae60',
   },
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootHearingOthers({ expanded, onChange }: Props) {
   const classes = useStyles();
   const participants = useSelector(selectParticipantsOfCurrentRoomWithoutMe);
   const audio = useSelector(selectParticipantAudio);

   const audioOfRoom = Object.entries(audio).filter((x) => participants.includes(x[0]));
   const mutedAudioInRoom = audioOfRoom.filter((x) => x[1]?.muted);

   return (
      <TroubleshootAccordion
         title="Hearing others"
         expanded={expanded}
         onChange={onChange}
         renderStatus={() => (
            <Chip
               size="small"
               className={clsx(classes.statusChip, { [classes.statusChipOk]: true })}
               label={`Subscribed to ${audioOfRoom.filter((x) => x[1]?.registered).length} participants audio`}
            />
         )}
      >
         <div></div>
      </TroubleshootAccordion>
   );
}
