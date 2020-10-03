import { RootAction } from 'pader-conference';
import { combineReducers } from 'redux';
import { getType } from 'typesafe-actions';
import * as actions from './actions';

export type ConferenceState = Readonly<{
   currentConference: string | null;

   createDialogOpen: boolean;
   isCreatingConference: boolean;
   createdConferenceId: string | null;
}>;

export default combineReducers<ConferenceState, RootAction>({
   currentConference: (state = null, action) => {
      switch (action.type) {
         default:
            return state;
      }
   },
   createDialogOpen: (state = false, action) => {
      switch (action.type) {
         case getType(actions.openCreateDialog):
            return true;
         case getType(actions.closeCreateDialog):
            return true;
         default:
            return state;
      }
   },
   isCreatingConference: (state = false, action) => {
      switch (action.type) {
         case getType(actions.createConferenceAsync.request):
            return true;
         case getType(actions.createConferenceAsync.success):
         case getType(actions.createConferenceAsync.failure):
            return false;
         default:
            return state;
      }
   },
   createdConferenceId: (state = null, action) => {
      switch (action.type) {
         case getType(actions.openCreateDialog):
            return null;
         case getType(actions.createConferenceAsync.success):
            return action.payload.conferenceId;
         default:
            return state;
      }
   },
});
