import _ from 'lodash';
import { ChatMessageDto } from 'src/core-hub.types';

export function mergeChatMessages(a: ChatMessageDto[], b: ChatMessageDto[]) {
   return _(a)
      .concat(b)
      .orderBy((x) => x.id)
      .uniqBy((x) => x.id)
      .value();
}

export function createPrivatChatChannel(p1: string, p2: string) {
   const arr = [p1, p2];
   arr.sort();

   return `chat?p1=${arr[0]}&p2=${arr[1]}&type=private`;
}
