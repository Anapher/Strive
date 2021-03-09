import { parseSynchronizedObjectId } from 'src/store/signal/synchronization/synchronized-object-id';
import { CHAT } from 'src/store/signal/synchronization/synchronized-object-ids';

export type GlobalChatChannel = { type: 'global' };
export type RoomChatChannel = { type: 'room'; roomId: string };
export type PrivateChatChannel = { type: 'private'; participants: string[] };

export type ChatChannel = GlobalChatChannel | RoomChatChannel | PrivateChatChannel;

export type ChatChannelWithId = ChatChannel & { id: string };

export function decode(id: string): ChatChannelWithId {
   const syncObjId = parseSynchronizedObjectId(id);
   if (syncObjId.id !== CHAT) throw new Error('Can only decode a synchronized object that is a chat.');

   const type = syncObjId.parameters['type'];
   switch (type) {
      case 'global':
         return { type, id };
      case 'room':
         return { type, roomId: syncObjId.parameters['roomId'], id };
      case 'private':
         return { type, participants: [syncObjId.parameters['p1'], syncObjId.parameters['p2']], id };
      default:
         throw new Error(`Invalid type: ${type}`);
   }
}
