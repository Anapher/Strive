import { makeStyles, Paper } from '@material-ui/core';
import React, { useEffect, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectParticipantList } from 'src/features/conference/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import * as actions from '../actions';
import { ChatChannelWithId } from '../channel-serializer';
import { hashCode, numberToColor } from '../color-utils';
import { clearChat, setSelectedChannel } from '../reducer';
import { selectChannels, selectSelectedChannel } from '../selectors';
import Chat from './Chat';
import ChatChannelTabs from './ChatChannelTabs';

const useStyles = makeStyles((theme) => ({
   root: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'hidden',
      borderColor: theme.palette.divider,
      borderWidth: '0px 0px 0px 1px',
      borderStyle: 'solid',
      height: '100%',
   },
}));

export default function ChatBar() {
   const classes = useStyles();
   const participants = useSelector(selectParticipantList);
   const connected = useSelector((state: RootState) => state.signalr.isConnected);
   const channels = useSelector(selectChannels);
   const selectedChannelId = useSelector(selectSelectedChannel);
   const myParticipantId = useMyParticipantId();

   const dispatch = useDispatch();

   const handleChangeSelectedChannel = (channel: ChatChannelWithId) => {
      dispatch(setSelectedChannel(channel.id));
   };

   useEffect(() => {
      if (connected) {
         dispatch(clearChat());
         dispatch(actions.subscribeChatMessages());
      }
   }, [connected]);

   const participantColors = useMemo(
      () => Object.fromEntries(participants.map((x) => [x.id, numberToColor(hashCode(x.id))]) ?? []),
      [participants],
   );

   return (
      <Paper className={classes.root} elevation={12} square>
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
