import { PayloadAction } from '@reduxjs/toolkit';
import { put, takeEvery } from 'redux-saga/effects';
import * as coreHub from 'src/core-hub';
import { RequestDisconnectDto } from 'src/equipment-hub.types';
import { equipmentKickedParticipantLeft } from 'src/errors';
import { close, onEventOccurred } from 'src/store/signal/actions';
import { setConnectionError } from '../conference/reducer';

function* onRequestDisconnect(action: PayloadAction<RequestDisconnectDto>) {
   yield put(close());
   switch (action.payload.reason) {
      case 'participantLeft':
         yield put(setConnectionError(equipmentKickedParticipantLeft()));
         break;
      default:
         break;
   }
}

export default function* mySaga() {
   yield takeEvery(onEventOccurred(coreHub.events.onRequestDisconnect), onRequestDisconnect);
}
