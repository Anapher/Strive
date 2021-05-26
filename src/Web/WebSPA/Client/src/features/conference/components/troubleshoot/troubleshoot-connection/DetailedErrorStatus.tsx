import { Grid, Typography, Box } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import StatusChip from '../StatusChip';
import { WebRtcHealth } from './useWebRtcHealth';

const webRtcNamespace = 'conference.troubleshooting.webrtc.status';

type Props = {
   health: WebRtcHealth;
};

export default function DetailedErrorStatus({ health: { connection, connector } }: Props) {
   const { t } = useTranslation();

   if (connector.status !== 'ok') {
      if (connector.webRtcStatus === 'connecting') {
         return (
            <div>
               <Typography gutterBottom>{t(`${webRtcNamespace}.connector_connecting.desc`)}</Typography>
               {connector.latestError && <Typography color="error">{connector.latestError.toString()}</Typography>}
            </div>
         );
      }

      if (connector.webRtcStatus === 'uninitialized')
         return <Typography>{t(`${webRtcNamespace}.uninitialized.message`)}</Typography>;

      return <>{'Connected but status not ok'}</>; // should not happen
   }

   if (!connection) return <>{'Connection does not exist but connector is status ok'}</>; // should not happen

   return (
      <div>
         <Grid container spacing={2}>
            <Grid item>
               <StatusChip
                  status={connection.receiveTransport.status}
                  label={t(`${webRtcNamespace}.transport_${connection.receiveTransport.transportState}.message`, {
                     transport: 'receive-transport',
                  })}
               />
            </Grid>
            <Grid item>
               <StatusChip
                  status={connection.sendTransport.status}
                  label={t(`${webRtcNamespace}.transport_${connection.sendTransport.transportState}.message`, {
                     transport: 'send-transport',
                  })}
               />
            </Grid>
         </Grid>
         <Box mt={2}>
            {connection.receiveTransport.transportState === 'new' &&
               connection.sendTransport.transportState === 'new' && (
                  <Typography>{t(`${webRtcNamespace}.transport_new.desc`)}</Typography>
               )}
            {connection.receiveTransport.transportState === 'connecting' &&
               connection.sendTransport.transportState === 'connecting' && (
                  <Typography>{t(`${webRtcNamespace}.transport_connecting.desc`)}</Typography>
               )}
         </Box>
      </div>
   );
}
