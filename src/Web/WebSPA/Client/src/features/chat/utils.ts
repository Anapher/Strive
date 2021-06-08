import _ from 'lodash';
import { ChatMessageDto } from 'src/core-hub.types';
import { getArrayEntryByHashCode } from 'src/utils/array-utils';

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

const participantColors = [
   '#4e79a7',
   '#f28e2c',
   '#e15759',
   '#76b7b2',
   '#59a14f',
   '#edc949',
   '#af7aa1',
   '#ff9da7',
   '#9c755f',
   '#bab0ab',
   '#81ecec',
   '#fd79a8',
   '#ffeaa7',
   '#00cec9',
   '#55efc4',
   '#00b894',
   '#dfe6e9',
   '#b8e994',
   '#60a3bc',
   '#f6b93b',
   '#ffcccc',
   '#7d5fff',
   '#17c0eb',
];

export function getParticipantColor(id: string): string {
   return getArrayEntryByHashCode(participantColors, id);
}
