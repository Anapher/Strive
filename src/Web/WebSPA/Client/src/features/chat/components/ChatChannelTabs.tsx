import { AppBar, Tab, Tabs } from '@material-ui/core';
import _ from 'lodash';
import { useSelector } from 'react-redux';
import { Participant } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { ChatChannel, ChatChannelWithId } from '../channel-serializer';
import NewMessagesIndicator from './NewMessagesIndicator';

type Props = {
   channels: ChatChannelWithId[];
   selected?: string | null;
   onSelectedChanged: (channel: ChatChannelWithId) => void;
   myParticipantId: string;
   participants: Participant[];
};

export default function ChatChannelTabs({
   channels,
   selected,
   onSelectedChanged,
   myParticipantId,
   participants,
}: Props) {
   const handleChange = (_: React.ChangeEvent<unknown>, newValue: number) => {
      onSelectedChanged(channels[newValue]);
   };

   const selectedIndex = channels.findIndex((x) => x.id === selected);

   const viewModels = useSelector((state: RootState) => state.chat.channels);

   return (
      <AppBar position="static" color="inherit">
         <Tabs
            value={selectedIndex === -1 ? false : selectedIndex}
            onChange={handleChange}
            variant={channels.length <= 2 ? 'fullWidth' : 'scrollable'}
         >
            {_.orderBy(channels, getChannelOrder).map((channel) => (
               <Tab
                  key={channel.id}
                  label={
                     <div style={{ display: 'flex', alignItems: 'center' }}>
                        {ChannelToString(channel, myParticipantId, participants)}
                        {viewModels?.[channel.id]?.viewModel?.newMessages && <NewMessagesIndicator />}
                     </div>
                  }
               ></Tab>
            ))}
         </Tabs>
      </AppBar>
   );
}

function ChannelToString(channel: ChatChannel, myParticipantId: string, participants: Participant[]): string {
   switch (channel.type) {
      case 'global':
         return 'All Chat';
      case 'room':
         return 'Room';
      case 'private': {
         const otherParticipantId =
            channel.participants[0] === myParticipantId ? channel.participants[1] : channel.participants[0];
         const otherParticipant = participants.find((x) => x.id === otherParticipantId);
         return otherParticipant ? `@${otherParticipant?.displayName}` : '@Unknown';
      }
   }
}

function getChannelOrder(channel: ChatChannel) {
   if (channel.type === 'global') return 1;
   if (channel.type === 'room') return 2;
   return 3;
}
