import { Link } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import FullscreenError from 'src/components/FullscreenError';
import * as coreHub from 'src/core-hub';
import ConferenceIndex from 'src/features/conference/components';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import SettingsDialog from 'src/features/settings/components/SettingsDialog';
import { fetchDevices } from 'src/features/settings/thunks';
import { RootState } from 'src/store';
import { close } from 'src/store/signal/actions';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import { WebRtcManager } from 'src/store/webrtc/WebRtcManager';
import to from 'src/utils/to';
import { ConferenceRouteParams } from '../types';

const defaultEvents: string[] = [
   coreHub.events.onSynchronizeObjectState,
   coreHub.events.onSynchronizedObjectUpdated,
   'OnError',
   coreHub.events.onPermissionsUpdated,
   coreHub.events.onEquipmentUpdated,
];

type Props = RouteComponentProps<ConferenceRouteParams>;

function ConferenceRoute({
   match: {
      params: { id },
   },
}: Props) {
   const error = useSelector((state: RootState) => state.conference.connectionError);
   const accessToken = useSelector((state: RootState) => state.auth.token?.accessToken);
   const conferenceState = useSelector((state: RootState) => state.conference.conferenceState);
   const { isConnected, isReconnecting } = useSelector((state: RootState) => state.signalr);
   const webRtc = useRef(new WebRtcManager()).current;

   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(fetchDevices());
   }, []);

   useEffect(() => {
      if (!accessToken) return; // should not happen as this is an authenticated route

      dispatch(coreHub.joinConference(id, defaultEvents, accessToken));
      return () => {
         dispatch(close());
      };
   }, [id, close, dispatch, accessToken]);

   useEffect(() => {
      if (isConnected && conferenceState) {
         webRtc.initialize({ sendMedia: true, receiveMedia: true });
      }
   }, [webRtc, isConnected, conferenceState]);

   if (error) {
      return (
         <FullscreenError message={error.message}>
            <Link {...to('/')}>Back to start</Link>
         </FullscreenError>
      );
   }

   if (!conferenceState || !isConnected) {
      return <ConferenceConnecting isReconnecting={isReconnecting} />;
   }

   return (
      <WebRtcContext.Provider value={webRtc}>
         <ConferenceIndex conference={conferenceState} />
         <SettingsDialog />
      </WebRtcContext.Provider>
   );
}

export default ConferenceRoute;
