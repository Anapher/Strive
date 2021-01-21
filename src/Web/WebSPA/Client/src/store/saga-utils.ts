import { PayloadAction } from '@reduxjs/toolkit';
import { takeEvery } from 'redux-saga/effects';
import { events } from 'src/core-hub';
import { onEventOccurred } from './signal/actions';

export function* takeEverySynchronizedObjectChange(name: string, worker: (action: PayloadAction<any>) => any) {
   yield takeEvery(onEventOccurred(events.onSynchronizedObjectUpdated).type, function* (action: any) {
      if (action.payload.name === name) {
         yield worker(action);
      }
   });

   yield takeEvery(onEventOccurred(events.onSynchronizeObjectState).type, worker);
}
