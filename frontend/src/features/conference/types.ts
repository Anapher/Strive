export type ParticipantDto = {
   participantId: string;
   role: string;
   displayName?: string;
   attributes: { [key: string]: string };
};
