import { Chip, Table, TableBody, TableCell, TableHead, TableRow, Tooltip, Typography } from '@material-ui/core';
import PauseIcon from '@material-ui/icons/Pause';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import _ from 'lodash';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectParticipantAudio, selectStreams } from 'src/features/media/selectors';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';

export default function DiagnosticsView() {
   const participantsOfRoom = useSelector(selectParticipantsOfCurrentRoom);
   const participants = useSelector(selectParticipants);
   const myId = useMyParticipantId();

   const streams = useSelector(selectStreams);
   const audio = useSelector(selectParticipantAudio);

   const connection = useWebRtc();
   const [consumers, setConsumers] = useState<Consumer[]>([]);

   useEffect(() => {
      if (connection) {
         const handleUpdateConsumers = () => {
            setConsumers(Array.from(connection.getConsumers()));
         };

         connection.eventEmitter.addListener('onConsumersChanged', handleUpdateConsumers);
         connection.eventEmitter.addListener('onConsumerUpdated', handleUpdateConsumers);

         handleUpdateConsumers();

         return () => {
            connection.eventEmitter.removeListener('onConsumersChanged', handleUpdateConsumers);
            connection.eventEmitter.removeListener('onConsumerUpdated', handleUpdateConsumers);
         };
      }
   }, [connection]);

   return (
      <div>
         <Table>
            <TableHead>
               <TableRow>
                  <TableCell>Display Name</TableCell>
                  <TableCell>Same Room</TableCell>
                  <TableCell>Producers</TableCell>
                  <TableCell>Consumers</TableCell>
                  <TableCell>Audio registered</TableCell>
                  <TableCell>Audio info</TableCell>
               </TableRow>
            </TableHead>
            <TableBody>
               {_.orderBy(participants, (x) => participantsOfRoom.includes(x.id), 'desc').map(
                  ({ id: participantId, displayName }) => (
                     <TableRow
                        key={participantId}
                        style={{ backgroundColor: myId === participantId ? 'rgba(230, 126, 34, 0.15)' : undefined }}
                     >
                        <TableCell>
                           <Tooltip title={participantId} PopperProps={{ disablePortal: true }}>
                              <Typography>{displayName}</Typography>
                           </Tooltip>
                        </TableCell>
                        <TableCell>{Boolean(participantsOfRoom.includes(participantId)).toString()}</TableCell>
                        <TableCell>
                           {Object.entries(streams?.[participantId]?.producers ?? {}).map(([id, info]) => (
                              <Tooltip key={id} title={`Paused=${info.paused}`} PopperProps={{ disablePortal: true }}>
                                 <Chip
                                    style={{ margin: 2 }}
                                    label={id}
                                    color="primary"
                                    icon={info.paused ? <PauseIcon /> : <PlayArrowIcon />}
                                    size="small"
                                 />
                              </Tooltip>
                           ))}
                        </TableCell>
                        <TableCell>
                           {Object.entries(streams?.[participantId]?.consumers ?? {}).map(([participantId, info]) => {
                              const participant = participants?.find((x) => x.id === info.participantId);
                              const consumer = consumers.find((x) => x.id === participantId);

                              return (
                                 <Tooltip
                                    key={participantId}
                                    title={`Id=${participantId}, participant id=${info.participantId}, remote paused=${info.paused}, local paused=${consumer?.paused}`}
                                    PopperProps={{ disablePortal: true }}
                                 >
                                    <Chip
                                       style={{ margin: 2 }}
                                       label={
                                          participantId === myId
                                             ? `${consumer?.appData.source} of ${
                                                  participant?.displayName ?? info.participantId
                                               }`
                                             : participant?.displayName ?? info.participantId
                                       }
                                       color={consumer ? 'primary' : 'default'}
                                       icon={info.paused ? <PauseIcon /> : <PlayArrowIcon />}
                                       size="small"
                                    />
                                 </Tooltip>
                              );
                           })}
                        </TableCell>
                        <TableCell>{Boolean(audio[participantId]?.registered).toString()}</TableCell>
                        <TableCell>{`speaking=${audio[participantId]?.speaking}, speaking=${audio[participantId]?.volume}, muted=${audio[participantId]?.muted}`}</TableCell>
                     </TableRow>
                  ),
               )}
            </TableBody>
         </Table>
      </div>
   );
}
