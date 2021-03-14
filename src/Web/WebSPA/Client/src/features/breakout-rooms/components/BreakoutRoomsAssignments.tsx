import {
   Box,
   Typography,
   Button,
   Grid,
   List,
   ListItem,
   ListItemIcon,
   ListItemText,
   makeStyles,
   useMediaQuery,
   useTheme,
} from '@material-ui/core';
import { DragDropContext, Droppable, Draggable, DropResult } from 'react-beautiful-dnd';
import React, { useEffect } from 'react';
import clsx from 'classnames';
import PersonIcon from '@material-ui/icons/Person';
import _ from 'lodash';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { Participant } from 'src/features/conference/types';

const useStyles = makeStyles((theme) => ({
   roomListDragOver: {
      backgroundColor: theme.palette.background.paper,
   },
   roomListItemDragging: {
      backgroundColor: theme.palette.primary.main,
   },
}));

type Props = {
   participants: Participant[];
   createdRooms: number;

   data: string[][];
   onChange: (data: string[][]) => void;
};

const copyInsert = (list: string[], i: number, item: string) => {
   const copy = [...list];
   copy.splice(i, 0, item);
   return copy;
};

const copyRemoveAt = (list: string[], i: number) => {
   const copy = [...list];
   copy.splice(i, 1);
   return copy;
};

const formatDroppableId = (i: number) => `room-${i}`;
const droppableIdToRoom = (s: string) => (s === 'defaultRoom' ? undefined : Number(s.substr(5)));

export default function BreakoutRoomsAssignments({ data, participants, createdRooms, onChange }: Props) {
   const unassignedParticipants = participants.filter((x) => !data.find((y) => y.includes(x.id)));
   const myParticipantId = useMyParticipantId();

   useEffect(() => {
      if (data.length > createdRooms) {
         onChange(data.slice(0, createdRooms));
      }
   }, [createdRooms]);

   const handleDragEnd = ({ source, destination, draggableId }: DropResult) => {
      if (!destination) return;

      if (source.droppableId === destination.droppableId) {
         const sourceRoomId = droppableIdToRoom(source.droppableId);
         if (sourceRoomId === undefined) return;

         const result = [...data[sourceRoomId]];
         const [removed] = result.splice(source.index, 1);
         result.splice(destination.index, 0, removed);

         onChange(data.map((x, i) => (i === sourceRoomId ? result : x)));
      } else {
         const sourceRoomId = droppableIdToRoom(source.droppableId);

         const destRoomId = droppableIdToRoom(destination.droppableId);
         const participantId = draggableId;

         const newSourceList =
            sourceRoomId !== undefined && (data[sourceRoomId] ? copyRemoveAt(data[sourceRoomId], source.index) : []);
         const newDestList =
            destRoomId !== undefined &&
            (data[destRoomId] ? copyInsert(data[destRoomId], destination.index, participantId) : [participantId]);

         const changed = Array.from({ length: Math.max((destRoomId ?? -1) + 1, data.length) }).map((_, i) =>
            i === sourceRoomId
               ? (newSourceList as string[])
               : i === destRoomId
               ? (newDestList as string[])
               : data[i] ?? [],
         );

         onChange(changed);
      }
   };

   const theme = useTheme();
   const isLg = useMediaQuery(theme.breakpoints.up('lg'));

   const handleRandomlyAssignParticipants = () => {
      const pool = _(participants)
         .filter((x) => x.id !== myParticipantId)
         .map((x) => x.id)
         .shuffle()
         .value();
      const participantsPerRoom = Math.ceil(pool.length / createdRooms);
      const data = Array.from({ length: createdRooms }).map((_, i) =>
         pool.slice(i * participantsPerRoom, (i + 1) * participantsPerRoom),
      );

      onChange(data);
   };

   const selectParticipants = (ids: string[]) =>
      ids?.map((x) => participants.find((y) => y.id === x)).filter((x): x is Participant => !!x) ?? [];

   return (
      <div>
         <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
            <Typography variant="h6">Assign participants to rooms</Typography>
            <Button size="small" variant="contained" color="secondary" onClick={handleRandomlyAssignParticipants}>
               Randomly assign rooms
            </Button>
         </Box>
         <DragDropContext onDragEnd={handleDragEnd}>
            <Grid container spacing={2}>
               <Grid item lg={4} md={6} xs={12}>
                  <RoomList
                     id="defaultRoom"
                     title="Unassigned"
                     participants={unassignedParticipants ?? []}
                     fullHeight
                  />
               </Grid>
               {isLg && (
                  <>
                     <Grid item lg={4}>
                        {Array.from({ length: createdRooms }).map(
                           (_, i) =>
                              i % 2 === 0 && (
                                 <RoomList
                                    key={i}
                                    id={formatDroppableId(i)}
                                    title={`Room #${i + 1}`}
                                    participants={selectParticipants(data[i])}
                                 />
                              ),
                        )}
                     </Grid>
                     <Grid item lg={4}>
                        {Array.from({ length: createdRooms }).map(
                           (_, i) =>
                              i % 2 === 1 && (
                                 <RoomList
                                    key={i}
                                    id={formatDroppableId(i)}
                                    title={`Room #${i + 1}`}
                                    participants={selectParticipants(data[i])}
                                 />
                              ),
                        )}
                     </Grid>
                  </>
               )}
               {!isLg && (
                  <Grid item md={6} xs={12}>
                     {Array.from({ length: createdRooms }).map((_, i) => (
                        <RoomList
                           key={i}
                           id={formatDroppableId(i)}
                           title={`Room #${i + 1}`}
                           participants={selectParticipants(data[i])}
                        />
                     ))}
                  </Grid>
               )}
            </Grid>
         </DragDropContext>
      </div>
   );
}

type RoomListProps = {
   id: string;
   title: string;
   participants: Participant[];
   fullHeight?: boolean;
};

function RoomList({ id, participants, title, fullHeight }: RoomListProps) {
   const classes = useStyles();

   return (
      <Droppable droppableId={id}>
         {(provided, snapshot) => (
            <div
               ref={provided.innerRef}
               className={clsx(snapshot.isDraggingOver && classes.roomListDragOver)}
               style={{ marginBottom: 8, height: fullHeight ? '100%' : undefined }}
            >
               <Typography variant="caption" color={snapshot.isDraggingOver ? 'secondary' : undefined}>
                  {title}
               </Typography>
               <List disablePadding>
                  {participants.map((item, index) => (
                     <Draggable key={item.id} draggableId={item.id} index={index}>
                        {(provided, snapshot) => (
                           <ListItem
                              dense
                              button
                              ref={provided.innerRef}
                              {...provided.draggableProps}
                              {...provided.dragHandleProps}
                              className={clsx(snapshot.isDragging && classes.roomListItemDragging)}
                              style={provided.draggableProps.style}
                           >
                              <ListItemIcon>
                                 <PersonIcon />
                              </ListItemIcon>
                              <ListItemText primary={item.displayName} />
                           </ListItem>
                        )}
                     </Draggable>
                  ))}
                  {provided.placeholder}
               </List>
            </div>
         )}
      </Droppable>
   );
}
