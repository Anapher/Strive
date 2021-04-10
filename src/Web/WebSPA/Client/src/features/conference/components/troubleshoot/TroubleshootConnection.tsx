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
import React, { useEffect, useState } from 'react';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import clsx from 'classnames';
import { formatBytes } from 'src/utils/string-utils';
import { Skeleton } from '@material-ui/lab';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import { WebRtcConnection } from 'src/store/webrtc/WebRtcConnection';

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
   table: {
      width: '100%',
   },
}));

type StatusMonitor = {
   getCurrentStatus: (soup?: WebRtcConnection) => StatusInfo;
};

const connectionStatusMonitor: StatusMonitor = {
   getCurrentStatus: (soup) => {
      if (!soup)
         return {
            type: 'error',
            message: 'WebRTC not initialized',
            desc:
               'The WebRTC module was not initialized. Please refresh this page and if you continue to have this error, please contact the support.',
         };
      if (!soup.sendTransport)
         return {
            type: 'error',
            message: 'send transport not created',
            desc:
               'The send transport was not created. Please refresh this page and if you continue to have this error, please contact the support.',
         };
      if (soup.sendTransport.connectionState === 'connected') return { type: 'ok', message: 'connected' };
      if (soup.sendTransport.connectionState === 'connecting')
         return {
            type: 'warn',
            message: 'connecting...',
            desc: 'WebRTC is currently trying to connect, please stand by.',
         };
      if (soup.sendTransport.connectionState === 'new')
         return {
            type: 'none',
            message: 'new',
            desc: 'The connection was created and initialized, but not yet established.',
         };

      return {
         type: 'error',
         message: soup.sendTransport.connectionState,
         desc: 'The WebRTC connection seems to have problems. Please check your browser or refresh this page.',
      };
   },
};

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;
};

export default function TroubleshootConnection({ expanded, onChange }: Props) {
   const connection = useWebRtc();
   const classes = useStyles();

   const [status, setStatus] = useState<StatusInfo | null>(null);

   useEffect(() => {
      const sendTransport = connection?.sendTransport;

      const changeHandler = () => setStatus(connectionStatusMonitor.getCurrentStatus(connection ?? undefined));
      sendTransport?.on('connectionstatechange', changeHandler);
      changeHandler();

      return () => {
         sendTransport?.off('connectionstatechange', changeHandler);
      };
   }, [connection]);

   const [transports, setTransports] = useState<any[]>([]);

   useEffect(() => {
      if (!expanded) return;

      const handle = setInterval(async () => {
         const stats = await connection?.sendTransport?.getStats();

         if (stats) {
            const transports = Array.from(stats.entries())
               .filter((x) => x[1].type === 'transport')
               .map((x) => x[1]);

            setTransports(transports);
         }
      }, 1000);

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
            <Typography className={classes.heading}>WebRTC Connection</Typography>
            <Chip
               size="small"
               className={clsx(classes.statusChip, { [classes.statusChipOk]: status?.type === 'ok' })}
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
                              <b>Id</b>
                           </TableCell>
                           <TableCell>
                              <b>State</b>
                           </TableCell>
                           <TableCell>
                              <b>Received</b>
                           </TableCell>
                           <TableCell>
                              <b>Sent</b>
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
                           Array.from({ length: 2 }).map((_, i) => (
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
