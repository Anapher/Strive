import { useReactOidc } from '@axa-fr/react-oidc-context';
import { Link } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import FullscreenError from 'src/components/FullscreenError';
import * as coreHub from 'src/core-hub';
import ConferenceIndex from 'src/features/conference/components';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import CreateConferenceDialog from 'src/features/create-conference/components/CreateConferenceDialog';
import SettingsDialog from 'src/features/settings/components/SettingsDialog';
import { fetchDevices } from 'src/features/settings/thunks';
import usePageTitle from 'src/hooks/usePageTitle';
import { RootState } from 'src/store';
import { close } from 'src/store/signal/actions';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import { WebRtcManager } from 'src/store/webrtc/WebRtcManager';
import { formatErrorMessage } from 'src/utils/error-utils';
import to from 'src/utils/to';
import { ConferenceRouteParams } from './types';

const defaultEvents: string[] = [
   coreHub.events.onSynchronizeObjectState,
   coreHub.events.onSynchronizedObjectUpdated,
   coreHub.events.onRequestDisconnect,
];

type Props = RouteComponentProps<ConferenceRouteParams>;

function ConferenceRoute({
   match: {
      params: { id },
   },
}: Props) {
   const error = useSelector((state: RootState) => state.conference.connectionError);
   const conferenceState = useSelector((state: RootState) => state.conference.conferenceState);
   const { isConnected, isReconnecting } = useSelector((state: RootState) => state.signalr);
   const webRtc = useRef(new WebRtcManager({ sendMedia: true, receiveMedia: true })).current;
   const { t } = useTranslation();

   const { oidcUser } = useReactOidc();

   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(fetchDevices());
   }, []);

   useEffect(() => {
      dispatch(coreHub.joinConference(id, defaultEvents, oidcUser.access_token));
      return () => {
         dispatch(close());
      };
   }, [id, close, dispatch, oidcUser]);

   useEffect(() => {
      webRtc.beginConnecting();
   }, [webRtc]);

   usePageTitle(conferenceState?.name);

   if (error) {
      return (
         <FullscreenError message={formatErrorMessage(error)}>
            <Link {...to('/')}>{t('common:back_to_start')}</Link>
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
         <CreateConferenceDialog />
      </WebRtcContext.Provider>
   );
}

export default ConferenceRoute;
