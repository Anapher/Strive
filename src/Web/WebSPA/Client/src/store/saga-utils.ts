import { PayloadAction } from '@reduxjs/toolkit';
import { takeEvery } from 'redux-saga/effects';
import { events } from 'src/core-hub';
import { SyncStatePayload } from 'src/core-hub.types';
import { onEventOccurred } from './signal/actions';
import { isSyncObjOfId } from './signal/synchronization/synchronized-object-id';

export function* takeEverySynchronizedObjectChange(
   id: string,
   worker: (action: PayloadAction<SyncStatePayload>) => any,
) {
   yield takeEvery(
      onEventOccurred(events.onSynchronizedObjectUpdated).type,
      function* (action: PayloadAction<SyncStatePayload>) {
         if (isSyncObjOfId(action.payload.id, id)) {
            yield worker(action);
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
