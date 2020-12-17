import { PayloadAction } from '@reduxjs/toolkit';

export type Size = {
   width: number;
   height: number;
};

export type ParticipantPayloadAction<T> = PayloadAction<{ data: T; participantId: string }>;
