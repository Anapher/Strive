import { applyPatch } from 'fast-json-patch';
import { onEventOccurred } from './actions';

type SyncObjRequest = {
   name: string;
   stateName?: string;
};

export function createSynchronizeObjectReducer(
   request: SyncObjRequest | (SyncObjRequest | string)[] | string | string,
) {
   if (!Array.isArray(request)) {
      request = [request];
   }

   const triggers = request.map((x) => (typeof x === 'string' ? { name: x } : x)) as SyncObjRequest[];
   return {
      [onEventOccurred('OnSynchronizeObjectState').type]: (state: any, action: any) => {
         for (const trigger of triggers) {
            state[trigger.stateName ?? trigger.name] = action.payload[trigger.name];
         }
      },
      [onEventOccurred('OnSynchronizedObjectUpdated').type]: (state: any, action: any) => {
         const trigger = triggers.find((x) => x.name === action.payload.name);
         if (trigger) {
            applyPatch(state[trigger.stateName ?? trigger.name], action.payload.patch);
         }
      },
   };
}
