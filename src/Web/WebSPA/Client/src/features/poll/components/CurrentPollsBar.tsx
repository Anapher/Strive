import { Divider, makeStyles, Paper } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectPollViewModels } from '../selectors';
import PollCard from './PollCard';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'hidden',
      borderColor: theme.palette.divider,
      borderRadius: theme.shape.borderRadius,
      marginBottom: theme.spacing(1),
   },
}));

export default function CurrentPollsBar() {
   const classes = useStyles();

   const polls = useSelector(selectPollViewModels);
   if (polls.length === 0) return null;

   return (
      <Paper className={classes.root} elevation={12} square>
         {polls.map((x) => (
            <div key={x.poll.id}>
               <PollCard poll={x} />
               <Divider />
            </div>
         ))}
      </Paper>
   );
}
