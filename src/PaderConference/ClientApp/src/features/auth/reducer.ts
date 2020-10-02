import { RootAction } from 'pader-conference';
import { AccessInfo } from 'MyModels';
import { combineReducers } from 'redux';
import { getType } from 'typesafe-actions';
import * as actions from './actions';

export type AuthState = Readonly<{
   isAuthenticated: boolean;
   rememberMe: boolean;
   token: AccessInfo | null;
}>;

export default combineReducers<AuthState, RootAction>({
   isAuthenticated: (state = false, action) => {
      switch (action.type) {
         case getType(actions.signInAsync.success):
            return true;
         case getType(actions.signOut):
            return false;
         default:
            return state;
      }
   },
   rememberMe: (state = false, action) => {
      switch (action.type) {
         case getType(actions.signInAsync.request):
            return action.payload.rememberMe;
         default:
            return state;
      }
   },
   token: (state = null, action) => {
      switch (action.type) {
         case getType(actions.signInAsync.success):
            return action.payload;
         case getType(actions.signOut):
            return null;
         case getType(actions.refreshTokenAsync.success):
            return action.payload;
         default:
            return state;
      }
   },
});
