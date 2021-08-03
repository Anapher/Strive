import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { isFirefox } from 'react-device-detect';
import { useTranslation } from 'react-i18next';
import StatusChip from './StatusChip';
import DetailedErrorStatus from './troubleshoot-connection/DetailedErrorStatus';
import { getStatusMessage } from './troubleshoot-connection/status-utils';
import TransportStats from './troubleshoot-connection/TransportStats';
import useWebRtcHealth from './troubleshoot-connection/useWebRtcHealth';
import TroubleshootAccordion from './TroubleshootAccordion';

const useStyles = makeStyles(() => ({
   transportStats: {
      width: '100%',
   },
   transportStatsContainer: {
      width: '100%',
      maxHeight: 400,
      overflowY: 'auto',
   },
}));

type Props = {
   className?: string;
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootConnection({ expanded, onChange, className }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const health = useWebRtcHealth();

   return (
      <TroubleshootAccordion
         className={className}
         title={t('conference.troubleshooting.webrtc.title')}
         expanded={expanded}
         onChange={onChange}
         renderStatus={() => (
            <StatusChip
               id="troubleshooting-connection-badge"
               size="small"
               status={health.status}
               label={getStatusMessage(health, t)}
            />
         )}
      >
         {health.status === 'ok' ? (
            <div className={classes.transportStatsContainer}>
               <TransportStats className={classes.transportStats} />
            </div>
         ) : (
            <div>
               <DetailedErrorStatus health={health} />
               {isFirefox && location.hostname === 'localhost' && (
                  <Typography color="error">
                     It seems like you are using Firefox and connecting on localhost. Please note that this is not
                     supported by Firefox.
                  </Typography>
               )}
            </div>
         )}
      </TroubleshootAccordion>
   );
}
