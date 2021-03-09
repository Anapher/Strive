import { AppBar, Tab, Tabs } from '@material-ui/core';
import { ChatChannel, ChatChannelWithId } from '../channel-serializer';

type Props = {
   channels: ChatChannelWithId[];
   selected?: string | null;
   onSelectedChanged: (channel: ChatChannelWithId) => void;
};

export default function ChatChannelTabs({ channels, selected, onSelectedChanged }: Props) {
   const handleChange = (_: React.ChangeEvent<unknown>, newValue: number) => {
      onSelectedChanged(channels[newValue]);
   };

   return (
      <AppBar position="static" color="inherit">
         <Tabs value={channels.findIndex((x) => x.id === selected)} onChange={handleChange}>
            {channels.map((channel) => (
               <Tab key={channel.id} label={ChannelToString(channel)}></Tab>
            ))}
         </Tabs>
      </AppBar>
   );
}

function ChannelToString(channel: ChatChannel): string {
   switch (channel.type) {
      case 'global':
         return 'All Chat';
      case 'room':
         return 'Room';
      case 'private':
         return 'Private';
   }
}
