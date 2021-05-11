import { Typography } from '@material-ui/core';
import React, { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import * as equipmentHub from 'src/equipment-hub';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import Equipment from '../features/equipment/components/Equipment';
import { WebRtcManager } from 'src/store/webrtc/WebRtcManager';
import FullScreenError from 'src/components/FullscreenError';
import RequestPermissions from '../features/equipment/components/RequestPermissions';
import { formatErrorMessage } from 'src/utils/error-utils';
import RequestUserInteractionView from 'src/features/conference/components/RequestUserInteractionView';

const defaultEvents: string[] = [
   equipmentHub.events.onRequestDisconnect,
   equipmentHub.events.onConnectionError,
   equipmentHub.events.onEquipmentCommand,
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
   const userInteractionMade = useSelector((state: RootState) => state.media.userInteractionMade);

   const dispatch = useDispatch();
   const webRtc = useRef(new WebRtcManager({ sendMedia: true, receiveMedia: false })).current;

   let token: string | null = null;
   let participantId: string | null = null;

   if (location.search) {
      const params = new URLSearchParams(location.search);
      token = params.get('token');
      participantId = params.get('participantId');
   }

   useEffect(() => {
      if (!token) return;
      if (!participantId) return;

      dispatch(equipmentHub.joinConference(id, participantId, token, defaultEvents));
      webRtc.beginConnecting();

      return () => {
         dispatch(close());
      };
   }, [id, close, token, participantId, webRtc]);

   const [hasPermissions, setHasPermissions] = useState(false);

   if (!token) {
      return <Typography>No token provided.</Typography>;
   }

   if (connectionError) {
      return <FullScreenError message={formatErrorMessage(connectionError)}></FullScreenError>;
   }

   if (!isConnected) {
      return <ConferenceConnecting isReconnecting={isReconnecting} />;
   }

   if (!hasPermissions) {
      return (
         <RequestPermissions
            onPermissionsGranted={() => {
               setHasPermissions(true);
            }}
         />
      );
   }

   if (!userInteractionMade) {
      // as request permissions may return automatically
      return <RequestUserInteractionView />;
   }

   return (
      <WebRtcContext.Provider value={webRtc}>
         <Equipment />
      </WebRtcContext.Provider>
   );
}
