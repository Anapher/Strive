import { put, select } from '@redux-saga/core/effects';
import { PayloadAction } from '@reduxjs/toolkit';
import { takeEvery } from 'redux-saga/effects';
import { events, sendChatMessage } from 'src/core-hub';
import { ChatMessageDto } from 'src/core-hub.types';
import { showErrorOn } from 'src/store/notifier/utils';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { onEventOccurred } from 'src/store/signal/actions';
import { CHAT } from 'src/store/signal/synchronization/synchronized-object-ids';
import { ChatChannelWithId } from './channel-serializer';
import { addAnnouncement, setSelectedChannel } from './reducer';
import { selectChannels, selectSelectedChannel } from './selectors';

export default function* mySaga() {
   yield showErrorOn(sendChatMessage.returnAction);
   yield* takeEverySynchronizedObjectChange(CHAT, adjustSelectedChannel);
   yield takeEvery(onEventOccurred(events.chatMessage).type, onChatMessage);
}

function* adjustSelectedChannel(): any {
   const channels: ChatChannelWithId[] = yield select(selectChannels);
   const selectedChannel: string | null = yield select(selectSelectedChannel);

   if (channels.length === 0) {
      if (selectedChannel) {
         yield put(setSelectedChannel(null));
         return;
      }
   }

   if (selectedChannel == null) {
      const defaultChannel = getDefaultChannel(channels);
      yield put(setSelectedChannel(defaultChannel.id));
      return;
   }

   if (!channels.find((x) => x.id === selectedChannel)) {
      const defaultChannel = getDefaultChannel(channels);
      yield put(setSelectedChannel(defaultChannel.id));
      return;
   }
}

function* onChatMessage({ payload }: PayloadAction<ChatMessageDto>) {
   if (payload.options.isAnnouncement) {
      yield put(addAnnouncement(payload));
   }
}

function getDefaultChannel(channels: ChatChannelWithId[]): ChatChannelWithId {
   const globalChannel = channels.find((x) => x.type === 'global');
   return globalChannel ?? channels[0];
}
