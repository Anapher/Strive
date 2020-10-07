import { applyOperation } from 'fast-json-patch';
import { onEventOccurred } from './actions';

export function createSynchronizeObjectReducer(name: string, stateName?: string) {
   if (!stateName) stateName = name;

   return {
      [onEventOccurred('OnSynchronizedObjectState').type]: (state: any, action: any) => {
         state[stateName!] = action.payload[name];
      },
      [onEventOccurred('OnSynchronizedObjectUpdated').type]: (state: any, action: any) => {
         if (action.payload.name === name) {
            state[stateName!] = applyOperation(state[stateName!], action.payload.patch);
         }
      },
   };
}
