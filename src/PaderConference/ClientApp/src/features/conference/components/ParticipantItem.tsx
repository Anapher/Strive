import { makeStyles, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';
import { useSelector } from 'react-redux';
import { Roles } from 'src/consts';
import { RootState } from 'src/store';
import { ParticipantDto } from 'src/store/conference-signal/types';
import MicOffIcon from '@material-ui/icons/MicOff';
import MicIcon from '@material-ui/icons/Mic';
import { getParticipantProducers } from 'src/features/media/selectors';
import ToggleIcon from './ToggleIcon';

const useStyles = makeStyles({
   root: {
      marginLeft: 16,
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'center',
   },
});

type Props = {
   participant?: ParticipantDto;
};

export default function ParticipantItem({ participant }: Props) {
   const classes = useStyles();
   const producers = useSelector((state: RootState) => getParticipantProducers(state, participant?.participantId));

   return (
      <div className={classes.root}>
         <Typography color={participant?.role === Roles.Moderator ? 'secondary' : undefined} variant="subtitle1">
            {participant ? participant?.displayName : <Skeleton />}
         </Typography>
         <ToggleIcon IconEnable={MicIcon} IconDisable={MicOffIcon} enabled={producers?.mic?.paused} />
      </div>
   );
}
