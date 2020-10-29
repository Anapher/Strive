import { Link } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import FullscreenError from 'src/components/FullscreenError';
import * as coreHub from 'src/core-hub';
import ClassConference from 'src/features/conference/components/ClassConference';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import ConferenceNotOpen from 'src/features/conference/components/ConferenceNotOpen';
import SettingsDialog from 'src/features/settings/components/SettingsDialog';
import { RootState } from 'src/store';
import { close } from 'src/store/signal/actions';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import { WebRtcManager } from 'src/store/webrtc/WebRtcManager';
import to from 'src/utils/to';
import { ConferenceRouteParams } from '../types';

const defaultEvents: string[] = [
   'OnSynchronizeObjectState',
   'OnSynchronizedObjectUpdated',
   'OnError',
   'OnPermissionsUpdated',
   'OnEquipmentUpdated',
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
      if (!accessToken) return; // should not happen as this is an authenticated route

      dispatch(coreHub.joinConference(id, defaultEvents, accessToken));
      return () => {
         dispatch(close());
      };
   }, [id, close, dispatch, accessToken]);

   useEffect(() => {
      if (isConnected && conferenceState?.isOpen) {
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

   if (!conferenceState.isOpen) {
      return <ConferenceNotOpen conferenceInfo={conferenceState} />;
   }

   switch (conferenceState.conferenceType) {
      default:
         return (
            <WebRtcContext.Provider value={webRtc}>
               <ClassConference />
               <SettingsDialog />
            </WebRtcContext.Provider>
         );
   }
}

export default ConferenceRoute;
