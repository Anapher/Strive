import { PayloadAction } from '@reduxjs/toolkit';
import { Operation } from 'fast-json-patch';
import { takeEvery } from 'redux-saga/effects';
import { events } from 'src/core-hub';
import { SyncStatePayload } from 'src/core-hub.types';
import { onEventOccurred } from './signal/actions';
import { isSyncObjOfId } from './signal/synchronization/synchronized-object-id';
import { SUBSCRIPTIONS } from './signal/synchronization/synchronized-object-ids';

export function* takeEverySynchronizedObjectChange(
   id: string,
   worker: (action: PayloadAction<SyncStatePayload>) => any,
) {
   yield takeEvery(
      onEventOccurred(events.onSynchronizedObjectUpdated).type,
      function* (action: PayloadAction<SyncStatePayload>) {
         if (isSyncObjOfId(action.payload.id, id)) {
            yield worker(action);
         } else if (isSyncObjOfId(action.payload.id, SUBSCRIPTIONS)) {
            const patch = action.payload.value as Operation[];
            const removedSubcription = patch
               .filter((x) => x.op === 'remove')
               .map((x) => x.path.substring('/subscriptions/'.length))
               .find((x) => isSyncObjOfId(x, id));

            if (removedSubcription) yield worker(action);
         }
      },
   );

   yield takeEvery(
      onEventOccurred(events.onSynchronizeObjectState).type,
      function* (action: PayloadAction<SyncStatePayload>) {
         if (isSyncObjOfId(action.payload.id, id)) {
            yield worker(action);
         }
      },
   );
}
