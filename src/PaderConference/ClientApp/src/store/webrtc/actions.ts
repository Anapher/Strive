import { createAction } from '@reduxjs/toolkit';

export type MediaSoupInitializedPayload = {
   canProduceVideo: boolean;
   canProduceAudio: boolean;
};

export const initialized = createAction<MediaSoupInitializedPayload>(`MEDIA_SOUP::INITIALIZED`);
export const initialize = createAction(`MEDIA_SOUP::INITIALIZE`);
