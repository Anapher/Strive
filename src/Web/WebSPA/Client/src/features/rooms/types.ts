export type RoomInfo = {
   roomId: string;
   displayName: string;
};

export type SynchronizedRooms = {
   rooms: RoomInfo[];
   defaultRoomId: string;
   participants: { [participanId: string]: string };
};

export type RoomViewModel = RoomInfo & {
   participants: string[];
   isDefaultRoom: boolean;
};
