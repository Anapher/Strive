import { Table, TableBody, TableCell, TableHead, TableRow } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import { formatBytes } from 'src/utils/string-utils';

type StatsRow = {
   id: string;
   name: string;
   state: string;
   received: string;
   sent: string;
};

const mapStats = (report: RTCStatsReport): StatsRow[] => {
   const stats = Array.from(report);
   return [
      ...stats
         .filter(([, value]) => value.type === 'transport')
         .map(([id, value]) => ({
            id,
            name: id,
            state: value.dtlsState,
            received: formatBytes(value.bytesReceived),
            sent: formatBytes(value.bytesSent),
         })),
      ...stats
         .filter(([, value]) => value.type === 'outbound-rtp' && Boolean(value.bytesSent))
         .map(([id, value]) => ({
            id,
            name: `${value.type} (${value.kind})`,
            state: 'ok',
            received: '-',
            sent: formatBytes(value.bytesSent),
         })),
      ...stats
         .filter(([, value]) => value.type === 'remote-inbound-rtp')
         .map(([id, value]) => ({
            id,
            name: `${value.type} (${value.kind})`,
            state: value.packetsLost !== undefined ? `${value.packetsLost} packet(s) lost` : 'ok',
            received: value.bytesReceived !== undefined ? formatBytes(value.bytesReceived) : '-',
            sent: '-',
         })),
   ];
};

type Props = {
   className?: string;
};

export default function TransportStats({ className }: Props) {
   const { t } = useTranslation();
   const connection = useWebRtc();
   const [transports, setTransports] = useState<StatsRow[]>([]);

   useEffect(() => {
      const refreshStats = async () => {
         const stats = await connection?.sendTransport?.getStats();
         if (stats) {
            setTransports(mapStats(stats));
         }
      };
      refreshStats();

      const handle = setInterval(refreshStats, 1000);

      return () => clearInterval(handle);
   }, [connection?.sendTransport]);

   return (
      <Table aria-label="transports overview" className={className}>
         <TableHead>
            <TableRow>
               <TableCell>
                  <b>{t('conference.troubleshooting.webrtc.table.id')}</b>
               </TableCell>
               <TableCell>
                  <b>{t('conference.troubleshooting.webrtc.table.state')}</b>
               </TableCell>
               <TableCell>
                  <b>{t('conference.troubleshooting.webrtc.table.received')}</b>
               </TableCell>
               <TableCell>
                  <b>{t('conference.troubleshooting.webrtc.table.sent')}</b>
               </TableCell>
            </TableRow>
         </TableHead>
         <TableBody>
            {transports.map((row) => (
               <TableRow key={row.id}>
                  <TableCell>{row.name}</TableCell>
                  <TableCell>{row.state}</TableCell>
                  <TableCell>{row.received}</TableCell>
                  <TableCell>{row.sent}</TableCell>
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
   );
}
