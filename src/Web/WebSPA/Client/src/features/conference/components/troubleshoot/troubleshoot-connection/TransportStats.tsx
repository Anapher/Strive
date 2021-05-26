import { Table, TableBody, TableCell, TableHead, TableRow } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import { formatBytes } from 'src/utils/string-utils';

type Props = {
   className?: string;
};

export default function TransportStats({ className }: Props) {
   const { t } = useTranslation();
   const connection = useWebRtc();
   const [transports, setTransports] = useState<any[]>([]);

   useEffect(() => {
      const refreshStats = async () => {
         const stats = await connection?.sendTransport?.getStats();

         if (stats) {
            const transports = Array.from(stats.entries())
               .filter((x) => x[1].type === 'transport')
               .map((x) => x[1]);

            setTransports(transports);
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
   );
}
