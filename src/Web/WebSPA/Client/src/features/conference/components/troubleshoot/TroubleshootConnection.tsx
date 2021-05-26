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
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootConnection({ expanded, onChange }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const health = useWebRtcHealth();

   return (
      <TroubleshootAccordion
         title={t('conference.troubleshooting.webrtc.title')}
         expanded={expanded}
         onChange={onChange}
         renderStatus={() => <StatusChip size="small" status={health.status} label={getStatusMessage(health, t)} />}
      >
         {health.status === 'ok' ? (
            <TransportStats className={classes.transportStats} />
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
