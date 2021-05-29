import { AppBar, Tab, Tabs } from '@material-ui/core';
import { TFunction } from 'i18next';
import _ from 'lodash';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { ParticipantsMap, selectParticipants } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import { ChatChannel, ChatChannelWithId } from '../channel-serializer';
import NewMessagesIndicator from './NewMessagesIndicator';

type Props = {
   channels: ChatChannelWithId[];
   selected?: string | null;
   onSelectedChanged: (channel: ChatChannelWithId) => void;
   myParticipantId: string;
};

export default function ChatChannelTabs({ channels, selected, onSelectedChanged, myParticipantId }: Props) {
   const { t } = useTranslation();
   const viewModels = useSelector((state: RootState) => state.chat.channels);
   const participants = useSelector(selectParticipants);

   const handleChange = (_: React.ChangeEvent<unknown>, newValue: number) => {
      onSelectedChanged(channels[newValue]);
   };

   const selectedIndex = channels.findIndex((x) => x.id === selected);

   return (
      <AppBar position="static" color="inherit">
         <Tabs
            id="chat-tabs"
            value={selectedIndex === -1 ? false : selectedIndex}
            onChange={handleChange}
            variant={channels.length <= 2 ? 'fullWidth' : 'scrollable'}
         >
            {_.orderBy(channels, getChannelOrder).map((channel) => (
               <Tab
                  key={channel.id}
                  label={
                     <div style={{ display: 'flex', alignItems: 'center' }}>
                        {ChannelToString(channel, myParticipantId, participants, t)}
                        {viewModels?.[channel.id]?.viewModel?.newMessages && <NewMessagesIndicator />}
                     </div>
                  }
               />
            ))}
         </Tabs>
      </AppBar>
   );
}

function ChannelToString(
   channel: ChatChannel,
   myParticipantId: string,
   participants: ParticipantsMap,
   t: TFunction,
): string {
   switch (channel.type) {
      case 'global':
         return t('glossary:all_chat');
      case 'room':
         return t('common:room');
      case 'private': {
         const otherParticipantId =
            channel.participants[0] === myParticipantId ? channel.participants[1] : channel.participants[0];
         const otherParticipant = participants[otherParticipantId];
         return otherParticipant ? `@${otherParticipant?.displayName}` : t('conference.chat.private_chat_disconnected');
      }
   }
}

function getChannelOrder(channel: ChatChannel) {
   if (channel.type === 'global') return 1;
   if (channel.type === 'room') return 2;
   return 3;
}
