import { makeStyles } from '@material-ui/core';
import React, { useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { ChatChannelWithId } from 'src/features/chat/channel-serializer';
import Chat from 'src/features/chat/components/Chat';
import ChatChannelTabs from 'src/features/chat/components/ChatChannelTabs';
import { setSelectedChannel } from 'src/features/chat/reducer';
import { selectChannels, selectSelectedChannel } from 'src/features/chat/selectors';
import { getParticipantColor } from 'src/features/chat/utils';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { selectParticipantList } from '../../selectors';

const useStyles = makeStyles({
   root: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'hidden',
      minHeight: 0,
   },
});

export default function MobileChatTab() {
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
      <div className={classes.root}>
         <ChatChannelTabs
            channels={channels}
            selected={selectedChannelId}
            onSelectedChanged={handleChangeSelectedChannel}
            myParticipantId={myParticipantId}
         />
         {selectedChannelId && (
            <Chat channel={selectedChannelId} participantColors={participantColors} participantId={myParticipantId} />
         )}
         <div style={{ height: 2 }} />
      </div>
   );
}
