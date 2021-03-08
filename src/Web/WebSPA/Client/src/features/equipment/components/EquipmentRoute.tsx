import { Typography } from '@material-ui/core';
import React, { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import * as coreHub from 'src/core-hub';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import Equipment from './Equipment';
import { WebRtcManager } from 'src/store/webrtc/WebRtcManager';
import FullScreenError from 'src/components/FullscreenError';
import RequestPermissions from './RequestPermissions';

const defaultEvents: string[] = [
   coreHub.events.onSynchronizeObjectState,
   coreHub.events.onSynchronizedObjectUpdated,
   'OnError',
   coreHub.events.onEquipmentCommand,
];

type Props = RouteComponentProps<ConferenceRouteParams>;

export default function EquipmentRoute({
   location,
   match: {
      params: { id },
   },
}: Props) {
   const connectionError = useSelector((state: RootState) => state.conference.connectionError);
   const { isConnected, isReconnecting } = useSelector((state: RootState) => state.signalr);

   const dispatch = useDispatch();
   const webRtc = useRef(new WebRtcManager({ sendMedia: true, receiveMedia: false })).current;

   let token: string | null = null;

   if (location.search) {
      const params = new URLSearchParams(location.search);
      token = params.get('token');
   }

   useEffect(() => {
      if (!token) return;

      dispatch(coreHub.joinConferenceAsEquipment(id, defaultEvents, token));
      return () => {
         dispatch(close());
      };
   }, [id, close, token]);

   useEffect(() => {
      webRtc.beginConnecting();
   }, [webRtc]);

   const [hasPermissions, setHasPermissions] = useState(false);

   if (!token) {
      return <Typography>No token provided.</Typography>;
   }

   if (connectionError) {
      return <FullScreenError message={connectionError.message}></FullScreenError>;
   }

   if (!isConnected) {
      return <ConferenceConnecting isReconnecting={isReconnecting} />;
   }

   return (
      <WebRtcContext.Provider value={webRtc}>
         {hasPermissions ? <Equipment /> : <RequestPermissions onPermissionsGranted={() => setHasPermissions(true)} />}
      </WebRtcContext.Provider>
   );
}
