import { Dialog, DialogContent, DialogTitle, makeStyles } from '@material-ui/core';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import TwoLineFab from 'src/components/TwoLineFab';
import { selectTalkingStickQueue } from '../../selectors';
import PassStickList from './PassStickList';
import * as coreHub from 'src/core-hub';

const useStyles = makeStyles((theme) => ({
   primaryAction: {
      minWidth: 220,
      padding: theme.spacing(0, 4),
   },
}));

export default function PassStickFab() {
   const { t } = useTranslation();
   const dispatch = useDispatch();
   const classes = useStyles();

   const queue = useSelector(selectTalkingStickQueue);

   const [open, setOpen] = useState(false);

   const handleOpen = () => setOpen(true);
   const handleClose = () => setOpen(false);
   const handlePassStick = (participantId: string) => {
      dispatch(coreHub.talkingStickPass(participantId));
      handleClose();
   };

   return (
      <>
         <TwoLineFab
            variant="extended"
            color="secondary"
            onClick={handleOpen}
            className={classes.primaryAction}
            subtitle={t('conference.scenes.talking_stick_modes.add_to_list_status', {
               count: queue.length,
            })}
         >
            {t<string>('conference.scenes.talking_stick_modes.pass_stick')}
         </TwoLineFab>
         <Dialog onClose={handleClose} open={open} aria-labelledby="pass-stick-title" fullWidth maxWidth="sm">
            <DialogTitle id="pass-stick-title">
               {t('conference.scenes.talking_stick_modes.pass_stick_to_participant')}
            </DialogTitle>
            <DialogContent>
               <PassStickList onPassStick={handlePassStick} />
            </DialogContent>
         </Dialog>
      </>
   );
}
