export type RoomInfo = {
   roomId: string;
   displayName: string;
   isEnabled: boolean;
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
