import { Accordion, AccordionDetails, AccordionSummary, makeStyles, Typography } from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import React from 'react';
import { isFirefox } from 'react-device-detect';
import { useTranslation } from 'react-i18next';
import StatusChip from './StatusChip';
import DetailedErrorStatus from './troubleshoot-connection/DetailedErrorStatus';
import { getStatusMessage } from './troubleshoot-connection/status-utils';
import TransportStats from './troubleshoot-connection/TransportStats';
import useWebRtcHealth from './useWebRtcHealth';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
   },
   statusChip: {
      marginRight: theme.spacing(2),
   },
   statusChipOk: {
      backgroundColor: '#27ae60',
   },
   statusChipError: {
      backgroundColor: theme.palette.error.main,
   },
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

   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };

   return (
      <Accordion expanded={expanded} onChange={handleChange}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls="troubleshoot-connection-content"
            id="troubleshoot-connection-header"
         >
            <Typography className={classes.heading}>{t('conference.media.troubleshooting.webrtc.title')}</Typography>
            <StatusChip
               size="small"
               status={health.status}
               label={getStatusMessage(health, t)}
               className={classes.statusChip}
            />
         </AccordionSummary>
         <AccordionDetails>
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
         </AccordionDetails>
      </Accordion>
   );
}
