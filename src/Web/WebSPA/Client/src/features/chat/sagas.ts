import { put, select } from '@redux-saga/core/effects';
import { sendChatMessage } from 'src/core-hub';
import { showErrorOn } from 'src/store/notifier/utils';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { CHAT } from 'src/store/signal/synchronization/synchronized-object-ids';
import { ChatChannelWithId } from './channel-serializer';
import { setSelectedChannel } from './reducer';
import { selectChannels, selectSelectedChannel } from './selectors';

export default function* mySaga() {
   yield showErrorOn(sendChatMessage.returnAction);
   yield* takeEverySynchronizedObjectChange(CHAT, adjustSelectedChannel);
}

function* adjustSelectedChannel(): any {
   console.log('update');

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

function getDefaultChannel(channels: ChatChannelWithId[]): ChatChannelWithId {
   const globalChannel = channels.find((x) => x.type === 'global');
   return globalChannel ?? channels[0];
}
