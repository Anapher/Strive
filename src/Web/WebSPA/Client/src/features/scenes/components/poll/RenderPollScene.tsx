import { Button, Chip, Collapse, makeStyles, Typography } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { Incognito, IncognitoOff, Pencil, PencilOff } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { updatePollState } from 'src/core-hub';
import PollResultsView from 'src/features/poll/components/PollResultsView';
import { selectPollViewModelFactory } from 'src/features/poll/selectors';
import usePermission from 'src/hooks/usePermission';
import useSelectorFactory from 'src/hooks/useSelectorFactory';
import { POLL_CAN_OPEN } from 'src/permissions';
import { RootState } from 'src/store';
import { PollScene, RenderSceneProps } from '../../types';
import AutoSceneLayout from '../AutoSceneLayout';

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
   chipsContainer: {
      marginTop: theme.spacing(2),
   },
   chip: {
      backgroundColor: '#292929',
      marginLeft: 4,
      marginRight: 4,
   },
   alert: {
      marginTop: theme.spacing(2),
      maxWidth: 720,
   },
}));

export default function RenderPollScene({ className, scene, dimensions }: RenderSceneProps<PollScene>) {
   const classes = useStyles();
   const { t } = useTranslation();
   const canOpenPoll = usePermission(POLL_CAN_OPEN);
   const dispatch = useDispatch();

   const viewModel = useSelectorFactory(selectPollViewModelFactory, (state: RootState, selector) =>
      selector(state, scene.pollId),
   );

   if (!viewModel) return null;

   const poll = viewModel.poll;
   const config = poll.config;

   const handlePublishResults = () => {
      dispatch(updatePollState({ pollId: poll.id, state: { ...poll.state, resultsPublished: true } }));
   };

   return (
      <AutoSceneLayout className={className} {...dimensions} center>
         <div className={classes.content}>
            <Typography variant="h4">{config.question}</Typography>
            {viewModel.results && (
               <div className={classes.pollContainer}>
                  <PollResultsView viewModel={viewModel} />
               </div>
            )}
            <div className={classes.chipsContainer}>
               <Chip
                  className={classes.chip}
                  icon={config.isAnonymous ? <Incognito fontSize="small" /> : <IncognitoOff fontSize="small" />}
                  label={
                     config.isAnonymous ? t('conference.poll.desc_anonymous') : t('conference.poll.desc_anonymous_not')
                  }
               />
               <Chip
                  className={classes.chip}
                  icon={config.isAnswerFinal ? <PencilOff fontSize="small" /> : <Pencil fontSize="small" />}
                  label={
                     config.isAnswerFinal
                        ? t('conference.poll.desc_answer_change_not')
                        : t('conference.poll.desc_answer_change')
                  }
               />
               <Chip
                  className={classes.chip}
                  label={
                     poll.state.isOpen ? t('conference.poll.desc_poll_open') : t('conference.poll.desc_poll_open_not')
                  }
               />
            </div>
            {canOpenPoll && (
               <Collapse in={!poll.state.resultsPublished}>
                  <Alert
                     className={classes.alert}
                     severity="warning"
                     action={
                        <Button color="inherit" size="small" onClick={handlePublishResults}>
                           {t('conference.poll.scene.publish')}
                        </Button>
                     }
                  >
                     {t('conference.poll.scene.warn_results_not_published')}
                  </Alert>
               </Collapse>
            )}
         </div>
      </AutoSceneLayout>
   );
}
