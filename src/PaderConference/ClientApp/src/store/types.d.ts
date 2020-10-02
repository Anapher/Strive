declare module 'pader-conference' {
   import { StateType } from 'typesafe-actions';

   export type Store = StateType<typeof import('./index').default>;
   export type RootState = StateType<typeof import('./root-reducer').default>;
   export type RootAction = ActionType<typeof import('./root-action').default>;

   import { Epic } from 'redux-observable';
   export type RootEpic = Epic<RootAction, RootAction, RootState, Services>;
}
