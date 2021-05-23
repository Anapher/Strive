import { PresenterScene } from './types';
import { createSelector } from '@reduxjs/toolkit';
import _ from 'lodash';
import { RootState } from 'src/store';
import { selectMyParticipantId } from '../auth/selectors';
import { selectStreams } from '../media/selectors';

export const selectSceneOptions = (state: RootState) => state.conference.conferenceState?.sceneOptions;

export const selectActiveParticipants = (state: RootState) => state.scenes.activeParticipants;
export const selectAvailableScenes = (state: RootState) => state.scenes.synchronized?.availableScenes ?? [];

export const selectSelectedScene = (state: RootState) => state.scenes.synchronized?.selectedScene;
export const selectOverwrittenScene = (state: RootState) => state.scenes.synchronized?.overwrittenContent;
export const selectSceneStack = (state: RootState) => state.scenes.synchronized?.sceneStack;

export const selectTalkingStickCurrentSpeakerId = (state: RootState) => state.scenes.talkingStick?.currentSpeakerId;
export const selectTalkingStickQueue = (state: RootState) => state.scenes.talkingStick?.speakerQueue ?? [];

export const selectTalkingStickIsMeSpeaker = createSelector(
   selectTalkingStickCurrentSpeakerId,
   selectMyParticipantId,
   (currentSpeaker, myId) => currentSpeaker === myId,
);

const getId = (_: unknown, id: string | undefined) => id;
export const selectIsParticipantPresenter = createSelector(selectSceneStack, getId, (scenes, id) => {
   if (!scenes) return false;

   const presenterScene = _.findLast(scenes, (x) => x.type === 'presenter') as PresenterScene | undefined;
   return presenterScene?.presenterParticipantId === id;
});

export const selectIsMeInQueue = createSelector(selectTalkingStickQueue, selectMyParticipantId, (queue, myId) =>
   myId ? queue.includes(myId) : false,
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
