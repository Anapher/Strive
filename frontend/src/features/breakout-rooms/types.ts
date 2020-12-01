export type ActiveBreakoutRoomState = {
   amount: number;
   deadline?: string;
   description?: string;
};

export type BreakoutRoomsInfo = {
   active: ActiveBreakoutRoomState | null;
};
