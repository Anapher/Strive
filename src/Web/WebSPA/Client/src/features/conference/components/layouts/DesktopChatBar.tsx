import { makeStyles, Paper } from '@material-ui/core';
import React, { useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { ChatChannelWithId } from 'src/features/chat/channel-serializer';
import Chat from 'src/features/chat/components/Chat';
import ChatChannelTabs from 'src/features/chat/components/ChatChannelTabs';
import { setSelectedChannel } from 'src/features/chat/reducer';
import { selectChannels, selectSelectedChannel } from 'src/features/chat/selectors';
import { getParticipantColor } from 'src/features/chat/utils';
import { selectParticipantList } from 'src/features/conference/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';

const useStyles = makeStyles((theme) => ({
   root: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'hidden',
      borderColor: theme.palette.divider,
      borderRadius: theme.shape.borderRadius,
      height: '100%',
   },
}));

export default function DesktopChatBar() {
   const classes = useStyles();
   const participants = useSelector(selectParticipantList);
   const channels = useSelector(selectChannels);
   const selectedChannelId = useSelector(selectSelectedChannel);
   const myParticipantId = useMyParticipantId();

   const dispatch = useDispatch();

   const handleChangeSelectedChannel = (channel: ChatChannelWithId) => {
      dispatch(setSelectedChannel(channel.id));
   };

   const participantColors = useMemo(
      () => Object.fromEntries(participants.map((x) => [x.id, getParticipantColor(x.id)]) ?? []),
      [participants],
   );

   return (
      <Paper id="chat-bar" className={classes.root} elevation={12} square>
         <ChatChannelTabs
            channels={channels}
            selected={selectedChannelId}
            onSelectedChanged={handleChangeSelectedChannel}
            myParticipantId={myParticipantId}
         />
         {selectedChannelId && (
            <Chat channel={selectedChannelId} participantColors={participantColors} participantId={myParticipantId} />
         )}
      </Paper>
   );
}
