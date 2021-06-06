import { Divider, makeStyles, Paper } from '@material-ui/core';
import _ from 'lodash';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectPollViewModels } from '../selectors';
import PollCard from './PollCard';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'auto',
      borderColor: theme.palette.divider,
      borderRadius: theme.shape.borderRadius,
      marginBottom: theme.spacing(1),
      maxHeight: '35%',
   },
}));

export default function CurrentPollsBar() {
   const classes = useStyles();

   const polls = useSelector(selectPollViewModels);
   if (polls.length === 0) return null;

   return (
      <Paper className={classes.root} elevation={12} square>
         {_(polls)
            .orderBy([(x) => x.poll.state.isOpen, (x) => x.poll.createdOn], ['desc', 'desc'])
            .map((x, i) => (
               <div key={x.poll.id}>
                  {i !== 0 && <Divider />}
                  <PollCard poll={x} />
               </div>
            ))
            .value()}
      </Paper>
   );
}
