import { Room } from 'src/store/signal/synchronization/synchronized-object-ids';

export type RoomViewModel = Room & {
   participants: string[];
   isDefaultRoom: boolean;
};
