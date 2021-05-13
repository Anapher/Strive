import {
   Accordion,
   AccordionDetails,
   AccordionSummary,
   Chip,
   makeStyles,
   Table,
   TableBody,
   TableCell,
   TableHead,
   TableRow,
   Typography,
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Skeleton } from '@material-ui/lab';
import clsx from 'classnames';
import React, { useEffect, useState } from 'react';
import { TFunction, useTranslation } from 'react-i18next';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import useWebRtcStatus from 'src/store/webrtc/hooks/useWebRtcStatus';
import { WebRtcConnection } from 'src/store/webrtc/WebRtcConnection';
import { WebRtcStatus } from 'src/store/webrtc/WebRtcManager';
import { formatBytes } from 'src/utils/string-utils';

type StatusType = 'ok' | 'warn' | 'error' | 'none';

type StatusInfo = {
   message: string;
   type: StatusType;
   desc?: string;
};

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
   table: {
      width: '100%',
   },
}));

type StatusMonitor = {
   getCurrentStatus: (t: TFunction<'translation'>, status: WebRtcStatus, soup?: WebRtcConnection) => StatusInfo;
};

const connectionStatusMonitor: StatusMonitor = {
   getCurrentStatus: (t, status, soup) => {
      const getTranslations = (key: string, includeDesc = true) => ({
         message: t<string>(`conference.media.troubleshooting.webrtc.status.${key}.message`),
         desc: includeDesc ? t<string>(`conference.media.troubleshooting.webrtc.status.${key}.desc`) : undefined,
      });

      if (status === 'connecting' || !soup) {
         return {
            type: 'error',
            ...getTranslations('connecting'),
         };
      }
      if (!soup.sendTransport)
         return {
            type: 'error',
            ...getTranslations('connecting'),
         };
      if (soup.sendTransport.connectionState === 'connected')
         return { type: 'ok', ...getTranslations('connected', false) };
      if (soup.sendTransport.connectionState === 'connecting')
         return {
            type: 'warn',
            ...getTranslations('send_transport_connecting'),
         };
      if (soup.sendTransport.connectionState === 'new')
         return {
            type: 'none',
            ...getTranslations('new'),
         };

      return {
         type: 'error',
         message: soup.sendTransport.connectionState,
         desc: t<string>('conference.media.troubleshooting.webrtc.status.invalid_connection_state.desc'),
      };
   },
};

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootConnection({ expanded, onChange }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const connection = useWebRtc();
   const webRtcStatus = useWebRtcStatus();

   const [status, setStatus] = useState<StatusInfo | null>(null);

   useEffect(() => {
      const sendTransport = connection?.sendTransport;

      const changeHandler = () =>
         setStatus(connectionStatusMonitor.getCurrentStatus(t, webRtcStatus, connection ?? undefined));
      sendTransport?.on('connectionstatechange', changeHandler);
      changeHandler();

      return () => {
         sendTransport?.off('connectionstatechange', changeHandler);
      };
   }, [connection, webRtcStatus, t]);

   const [transports, setTransports] = useState<any[]>([]);

   useEffect(() => {
      if (!expanded) return;

      const refreshStats = async () => {
         const stats = await connection?.sendTransport?.getStats();

         if (stats) {
            console.log(Array.from(stats.entries()));
            const transports = Array.from(stats.entries())
               .filter((x) => x[1].type === 'transport')
               .map((x) => x[1]);

            setTransports(transports);
         }
      };
      refreshStats();

      const handle = setInterval(refreshStats, 1000);

      return () => clearInterval(handle);
   }, [connection?.sendTransport, expanded]);

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
            <Chip
               size="small"
               className={clsx(classes.statusChip, {
                  [classes.statusChipOk]: status?.type === 'ok',
                  [classes.statusChipError]: status?.type === 'error',
               })}
               label={status?.message}
            />
         </AccordionSummary>
         <AccordionDetails>
            <div className={classes.table}>
               {status?.desc && <Typography gutterBottom>{status?.desc}</Typography>}
               {status?.type === 'ok' && (
                  <Table aria-label="transports overview" className={classes.table}>
                     <TableHead>
                        <TableRow>
                           <TableCell>
                              <b>{t('conference.media.troubleshooting.webrtc.table.id')}</b>
                           </TableCell>
                           <TableCell>
                              <b>{t('conference.media.troubleshooting.webrtc.table.state')}</b>
                           </TableCell>
                           <TableCell>
                              <b>{t('conference.media.troubleshooting.webrtc.table.received')}</b>
                           </TableCell>
                           <TableCell>
                              <b>{t('conference.media.troubleshooting.webrtc.table.sent')}</b>
                           </TableCell>
                        </TableRow>
                     </TableHead>
                     <TableBody>
                        {transports.map((row) => (
                           <TableRow key={row.id}>
                              <TableCell>{row.id}</TableCell>
                              <TableCell>{row.dtlsState}</TableCell>
                              <TableCell>{formatBytes(row.bytesReceived)}</TableCell>
                              <TableCell>{formatBytes(row.bytesSent)}</TableCell>
                           </TableRow>
                        ))}
                        {transports.length === 0 &&
                           Array.from({ length: 1 }).map((_, i) => (
                              <TableRow key={i}>
                                 <TableCell>
                                    <Skeleton />
                                 </TableCell>
                                 <TableCell>
                                    <Skeleton />
                                 </TableCell>
                                 <TableCell>
                                    <Skeleton />
                                 </TableCell>
                                 <TableCell>
                                    <Skeleton />
                                 </TableCell>
                              </TableRow>
                           ))}
                     </TableBody>
                  </Table>
               )}
            </div>
         </AccordionDetails>
      </Accordion>
   );
}
