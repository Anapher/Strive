export type SendChatMessageDto = {
   message: string;
   filter?: ChatMessageFilter;
};

export type ChatMessageFilter = {
   include?: string[];
   exclude?: string[];
};

export type CreateRoomDto = {
   displayName: string;
};

export type SwitchRoomDto = {
   roomId: string;
};
