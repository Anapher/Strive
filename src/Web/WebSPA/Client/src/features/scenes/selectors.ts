import { createSelector } from '@reduxjs/toolkit';
import _ from 'lodash';
import { selectParticipantsOfRoomWebcamAvailable } from 'src/features/media/selectors';
import { RootState } from 'src/store';
import { createArrayEqualSelector } from 'src/utils/reselect';
import { selectMyParticipantId } from '../auth/selectors';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectParticipantsOfCurrentRoom } from '../rooms/selectors';
import { PresenterScene } from './types';

export const selectSceneOptions = (state: RootState) => state.conference.conferenceState?.sceneOptions;

export const selectActiveParticipants = (state: RootState) => state.scenes.activeParticipants;
export const selectAvailableScenes = (state: RootState) => state.scenes.synchronized?.availableScenes ?? [];

export const selectSelectedScene = (state: RootState) => state.scenes.synchronized?.selectedScene;
export const selectOverwrittenScene = (state: RootState) => state.scenes.synchronized?.overwrittenContent;
export const selectSceneStack = (state: RootState) => state.scenes.synchronized?.sceneStack;

export const selectTalkingStickCurrentSpeakerId = (state: RootState) => state.scenes.talkingStick?.currentSpeakerId;
export const selectTalkingStickQueue = (state: RootState) => state.scenes.talkingStick?.speakerQueue ?? [];

export const selectTalkingStickIsMeSpeaker = (state: RootState) =>
   selectMyParticipantId(state) === selectTalkingStickCurrentSpeakerId(state);

const getId = (_: unknown, id: string | undefined) => id;
export const selectIsParticipantPresenterFactory = () =>
   createSelector(selectSceneStack, getId, (scenes, id) => {
      if (!scenes) return false;

      const presenterScene = _.findLast(scenes, (x) => x.type === 'presenter') as PresenterScene | undefined;
      return presenterScene?.presenterParticipantId === id;
   });

export const selectIsMeInQueue = (state: RootState) => {
   const myId = selectMyParticipantId(state);
   if (!myId) return false;

   const queue = selectTalkingStickQueue(state);
   return queue.includes(myId);
};

export const selectHideParticipantsWithoutWebcam = (state: RootState) =>
   selectSceneOptions(state)?.hideParticipantsWithoutWebcam;

export const selectSceneLayoutType = (state: RootState) => selectSceneOptions(state)?.sceneLayout;
export const selectScreenShareSceneLayoutType = (state: RootState) => selectSceneOptions(state)?.screenShareLayout;
export const hasAnyParticipantInCurrentRoomWebcamActicated = (state: RootState) =>
   selectParticipantsOfRoomWebcamAvailable(state).length > 0;

export const selectParticipantGridList = createArrayEqualSelector(
   createSelector(
      selectParticipants,
      selectParticipantsOfCurrentRoom,
      selectParticipantsOfRoomWebcamAvailable,
      selectHideParticipantsWithoutWebcam,
      (participants, participantsOfRoom, participantsWithWebcam, hideWithoutWebcam) => {
         let result = participantsOfRoom.map((id) => participants[id]).filter((x): x is Participant => Boolean(x));

         if (hideWithoutWebcam) {
            result = result.filter((x) => participantsWithWebcam.includes(x.id));
         }

         return result;
      },
   ),
   (x) => x,
);
