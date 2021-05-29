import { Chip, makeStyles } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import useWebRtcHealth from '../troubleshoot/troubleshoot-connection/useWebRtcHealth';

const useStyles = makeStyles((theme) => ({
   errorChip: {
      backgroundColor: theme.palette.error.main,
      color: theme.palette.error.contrastText,
   },
}));

export default function WebRtcStatusChip() {
   const { t } = useTranslation();
   const classes = useStyles();
   const health = useWebRtcHealth();

   if (health.status !== 'error') return null;

   return (
      <Chip
         id="appbar-status-chip-webrtc"
         className={classes.errorChip}
         style={{ marginRight: 8 }}
         label={t('conference.appbar.webrtc_error')}
         size="small"
      />
   );
}
