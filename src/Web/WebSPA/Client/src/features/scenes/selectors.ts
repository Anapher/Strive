import { createSelector } from '@reduxjs/toolkit';
import _ from 'lodash';
import { RootState } from 'src/store';
import { selectMyParticipantId } from '../auth/selectors';
import { selectStreams } from '../media/selectors';

export const selectActiveParticipants = (state: RootState) => state.scenes.activeParticipants;
export const selectAvailableScenes = (state: RootState) => state.scenes.synchronized?.availableScenes ?? [];

export const selectSelectedScene = (state: RootState) => state.scenes.synchronized?.selectedScene;
export const selectOverwrittenScene = (state: RootState) => state.scenes.synchronized?.overwrittenContent;
export const selectSceneStack = (state: RootState) => state.scenes.synchronized?.sceneStack;

export const selectTalkingStickCurrentSpeakerId = (state: RootState) => state.scenes.talkingStick?.currentSpeakerId;

export const selectIsMePresenter = createSelector(
   selectTalkingStickCurrentSpeakerId,
   selectMyParticipantId,
   (currentSpeaker, myId) => currentSpeaker === myId,
);

export const selectActiveParticipantsWithWebcam = createSelector(
   selectActiveParticipants,
   selectStreams,
   (participants, streams) => {
      if (!streams) return [];

      return _(Object.entries(participants))
         .filter(([participantId]) => streams[participantId]?.producers.webcam?.paused === false)
         .orderBy(([, info]) => info.orderNumber, 'asc')
         .map(([participantId]) => participantId)
         .value();
   },
);
