export type ConnectionMessageMetadata = {
   conferenceId: string;
   connectionId: string;
   participantId: string;
};

export type ConnectionMessage<TPayload> = {
   meta: ConnectionMessageMetadata;
   payload: TPayload;
};
