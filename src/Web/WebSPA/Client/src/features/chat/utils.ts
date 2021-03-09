import _ from 'lodash';
import { ChatMessageDto } from 'src/core-hub.types';

export function mergeChatMessages(a: ChatMessageDto[], b: ChatMessageDto[]) {
   return _(a)
      .concat(b)
      .orderBy((x) => x.id)
      .uniqBy((x) => x.id)
      .value();
}
