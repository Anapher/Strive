import { makeStyles, Paper } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from 'src/store';
import ParticipantItem from './ParticipantItem';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      padding: theme.spacing(2),
      borderTopLeftRadius: 0,
      borderBottomLeftRadius: 0,
   },
}));

export default function ParticipantsList() {
   const participants = useSelector((state: RootState) => state.conference.participants);
   const classes = useStyles();

   return (
      <Paper className={classes.root}>
         {participants ? (
            participants.map((x) => <ParticipantItem participant={x} key={x.participantId} />)
         ) : (
            <>
               <ParticipantItem />
               <ParticipantItem />
               <ParticipantItem />
            </>
         )}
      </Paper>
   );
}
