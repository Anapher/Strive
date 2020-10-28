import { Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import * as coreHub from 'src/core-hub';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';

const defaultEvents: string[] = [
   'OnSynchronizeObjectState',
   'OnSynchronizedObjectUpdated',
   'OnError',
   'OnPermissionsUpdated',
];

type Props = RouteComponentProps<ConferenceRouteParams>;

export default function EquipmentRoute({
   location,
   match: {
      params: { id },
   },
}: Props) {
   const conferenceState = useSelector((state: RootState) => state.conference.conferenceState);
   const { isConnected, isReconnecting } = useSelector((state: RootState) => state.signalr);

   const dispatch = useDispatch();

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

   if (!token) {
      return <Typography>No token provided.</Typography>;
   }

   if (!conferenceState || !isConnected) {
      return <ConferenceConnecting isReconnecting={isReconnecting} />;
   }

   return <div>Use as equipment</div>;
}
