import { Chip, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import PollResultsView from 'src/features/poll/components/PollResultsView';
import { selectPollViewModel } from 'src/features/poll/selectors';
import { RootState } from 'src/store';
import { PollScene, RenderSceneProps } from '../../types';
import ActiveChipsLayout from '../ActiveChipsLayout';
import { Incognito, IncognitoOff, Pencil, PencilOff } from 'mdi-material-ui';

const useStyles = makeStyles((theme) => ({
   root: {
      margin: theme.spacing(3),
      display: 'flex',
      justifyContent: 'center',
      flexDirection: 'column',
   },
   content: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
   pollContainer: {
      height: 400,
      maxWidth: 1000,
      width: '100%',
   },
}));

export default function RenderPollScene({ className, scene }: RenderSceneProps<PollScene>) {
   const classes = useStyles();
   const viewModel = useSelector((state: RootState) => selectPollViewModel(state, scene.pollId));

   if (!viewModel) return null;

   return (
      <ActiveChipsLayout className={className} contentClassName={classes.root}>
         <div className={classes.content}>
            <Typography variant="h4">{viewModel.poll.config.question}</Typography>
            {viewModel.results && (
               <div className={classes.pollContainer}>
                  <PollResultsView viewModel={viewModel} />
               </div>
            )}
            <div style={{ marginTop: 16 }}>
               <Chip
                  style={{ backgroundColor: '#292929' }}
                  icon={viewModel.poll.config.isAnonymous ? <Incognito /> : <IncognitoOff />}
                  label="Anonymous"
               />
               <Chip
                  style={{ backgroundColor: '#292929', marginLeft: 8, marginRight: 8 }}
                  icon={viewModel.poll.config.isAnswerFinal ? <PencilOff /> : <Pencil />}
                  label="Answer can be changed"
               />
               <Chip style={{ backgroundColor: '#292929' }} label="Poll is open" />
            </div>
         </div>
      </ActiveChipsLayout>
   );
}
