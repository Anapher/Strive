import { Box, ClickAwayListener, Grow, makeStyles, Paper, Popper, PopperProps, Typography } from '@material-ui/core';
import React from 'react';
import { PollViewModel } from '../types';
import PollResultsView from './PollResultsView';

const useStyles = makeStyles((theme) => ({
   root: {
      zIndex: theme.zIndex.modal,
   },
   paper: {
      width: 600,
      backgroundColor: theme.palette.background.paper,
   },
   question: {
      margin: theme.spacing(2),
   },
   chartContainer: {
      height: 300,
      minHeight: 0,
      margin: 8,
   },
}));

type Props = {
   open: boolean;
   viewModel: PollViewModel;
   anchorEl: PopperProps['anchorEl'];
   onClose: () => void;
};

export default function PollCardResultsPopup({ open, viewModel, anchorEl, onClose }: Props) {
   const classes = useStyles();

   return (
      <Popper open={open} anchorEl={anchorEl} transition placement="right-start" className={classes.root}>
         {({ TransitionProps }) => (
            <Grow {...TransitionProps} style={{ transformOrigin: 'right top' }}>
               <Paper className={classes.paper} elevation={5}>
                  <ClickAwayListener onClickAway={onClose}>
                     <Box display="flex" flexDirection="column">
                        <Typography align="center" variant="h6" className={classes.question}>
                           {viewModel.poll.config.question}
                        </Typography>
                        <div className={classes.chartContainer}>
                           <PollResultsView viewModel={viewModel} />
                        </div>
                     </Box>
                  </ClickAwayListener>
               </Paper>
            </Grow>
         )}
      </Popper>
   );
}
