import { Operation } from 'fast-json-patch';
import { events } from 'src/core-hub';
import { SyncStatePayload } from 'src/core-hub.types';
import { onEventOccurred } from './actions';
import { parseSynchronizedObjectId } from './synchronization/synchronized-object-id';
import { SUBSCRIPTIONS } from './synchronization/synchronized-object-ids';
import { synchronizedPropertyFactory, SynchronizeProperty } from './synchronization/synchronized-property';

export function synchronizeObjectState(requests: SynchronizeProperty | SynchronizeProperty[]) {
   if (!Array.isArray(requests)) {
      requests = [requests];
   }

   const synchronizedProperties = requests.map((x) => synchronizedPropertyFactory(x));
   return {
      [onEventOccurred(events.onSynchronizeObjectState).type]: (state: any, action: any) => {
         const payload = action.payload as SyncStatePayload;
         const trigger = synchronizedProperties.find((x) => x.isTriggeredBy(payload.id));
         if (trigger) {
            trigger.applyState(payload, state);
         }
      },
      [onEventOccurred(events.onSynchronizedObjectUpdated).type]: (state: any, action: any) => {
         const payload = action.payload as SyncStatePayload;
         const syncObjId = parseSynchronizedObjectId(payload.id);

         const trigger = synchronizedProperties.find((x) => x.isTriggeredBy(payload.id));
         if (trigger) {
            trigger.applyPatch(payload, state);
         }

         // if subscriptions are removed, make sure that we remove the synchronized objects observed
         // by this

         if (syncObjId.id === SUBSCRIPTIONS) {
            const patch = payload.value as Operation[];
            const removedSubscriptions = patch
               .filter((x) => x.op === 'remove')
               .map((x) => x.path.substring('/subscriptions/'.length));

            for (const syncObjId of removedSubscriptions) {
               const removeTrigger = synchronizedProperties.find((x) => x.isTriggeredBy(syncObjId));
               if (removeTrigger) removeTrigger.remove(syncObjId, state);
            }
         }
      },
   };
}
