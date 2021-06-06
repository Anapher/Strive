import { Button, IconButton, makeStyles, SvgIconTypeMap, Tooltip, Typography } from '@material-ui/core';
import { OverridableComponent } from '@material-ui/core/OverridableComponent';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import { Incognito, IncognitoOff, Pencil, PencilOff } from 'mdi-material-ui';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import usePermission from 'src/hooks/usePermission';
import { POLL_CAN_OPEN } from 'src/permissions';
import { PollAnswer, PollViewModel } from '../types';
import pollTypes from '../types/register';
import PollCardResultsPopup from './PollCardResultsPopup';
import PollCardResultSummary from './PollCardResultSummary';
import PollContextMenu from './PollContextMenu';

type PollStatusIconProps = {
   Icon: OverridableComponent<SvgIconTypeMap>;
   description: string;
};

function PollStatusIconProps({ Icon, description }: PollStatusIconProps) {
   return (
      <Tooltip title={description}>
         <Icon
            color="disabled"
            aria-label={description}
            style={{ width: 16, height: 16, marginLeft: 4, marginRight: 2 }}
            fontSize="small"
         />
      </Tooltip>
   );
}

const useStyles = makeStyles((theme) => ({
   root: { padding: theme.spacing(1) },
   bottomInfo: {
      paddingTop: theme.spacing(1),
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
   },
   bottomInfoButtons: {
      display: 'flex',
   },
   header: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'flex-start',
   },
   questionText: {
      flex: 1,
      marginRight: 8,
      minWidth: 0,
   },
   moreIcon: {
      marginRight: -8,
      marginTop: -8,
   },
}));

type Props = {
   poll: PollViewModel;
};
export default function PollCard({ poll: viewModel }: Props) {
   const { poll } = viewModel;
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const canOpenPoll = usePermission(POLL_CAN_OPEN);

   const [contextMenuOpen, setContextMenuOpen] = useState(false);
   const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

   const [resultsOpen, setResultsOpen] = useState(false);
   const [resultsAnchorEl, setResultsAnchorEl] = useState<null | HTMLElement>(null);

   const footerContainer = useRef(null);

   const presenter = pollTypes.find((x) => x.instructionType === poll.instruction.type);
   if (!presenter) return null;

   const onSubmitAnswer = (answer: PollAnswer) => {
      dispatch(coreHub.submitPollAnswer({ answer, pollId: poll.id }));
   };

   const deleteAnswer = () => {
      dispatch(coreHub.deletePollAnswer({ pollId: poll.id }));
   };

   const handleToggleContextMenu = (event: React.MouseEvent<HTMLButtonElement>) => {
      setAnchorEl(event.currentTarget);
      setContextMenuOpen((val) => !val);
   };

   const handleOpenResults = (event: React.MouseEvent<HTMLButtonElement>) => {
      setResultsAnchorEl(event.currentTarget);
      setResultsOpen(true);
   };

   const handleCloseResults = () => setResultsOpen(false);

   return (
      <div className={classes.root}>
         <div className={classes.header}>
            <div className={classes.questionText}>
               <Typography variant="caption">
                  {presenter.getPollDescription(viewModel.poll.instruction, t as any)}
               </Typography>
               <Typography gutterBottom style={{ marginTop: 8, marginBottom: 8 }}>
                  {poll.config.question}
               </Typography>
            </div>
            {canOpenPoll && (
               <>
                  <IconButton className={classes.moreIcon} onClick={handleToggleContextMenu}>
                     <MoreVertIcon fontSize="small" />
                  </IconButton>
                  <PollContextMenu
                     poll={viewModel}
                     open={contextMenuOpen}
                     onClose={() => setContextMenuOpen(false)}
                     anchorEl={anchorEl}
                  />
               </>
            )}
         </div>
         {poll.state.isOpen && (
            <presenter.PollAnswerForm
               onSubmit={onSubmitAnswer}
               onDelete={deleteAnswer}
               poll={viewModel}
               footerPortalRef={footerContainer.current}
            />
         )}
         {!poll.state.isOpen && <PollCardResultSummary viewModel={viewModel} />}
         <div className={classes.bottomInfo}>
            <div className={classes.bottomInfoButtons}>
               <div ref={footerContainer} />
               <Button size="small" disabled={!poll.state.resultsPublished && !canOpenPoll} onClick={handleOpenResults}>
                  {t('conference.poll.results')}
               </Button>
            </div>
            <div>
               <PollStatusIconProps
                  Icon={poll.config.isAnonymous ? Incognito : IncognitoOff}
                  description={
                     poll.config.isAnonymous
                        ? t('conference.poll.desc_anonymous')
                        : t('conference.poll.desc_anonymous_not')
                  }
               />
               <PollStatusIconProps
                  Icon={poll.config.isAnswerFinal ? PencilOff : Pencil}
                  description={
                     poll.config.isAnswerFinal
                        ? t('conference.poll.desc_answer_change_not')
                        : t('conference.poll.desc_answer_change')
                  }
               />
            </div>
         </div>
         <PollCardResultsPopup
            open={resultsOpen}
            onClose={handleCloseResults}
            viewModel={viewModel}
            anchorEl={resultsAnchorEl}
         />
      </div>
   );
}
