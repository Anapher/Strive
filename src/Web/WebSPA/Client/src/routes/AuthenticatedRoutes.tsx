import { useReactOidc } from '@axa-fr/react-oidc-context';
import Axios from 'axios';
import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Route, Switch } from 'react-router-dom';
import { setParticipantId } from 'src/features/auth/reducer';
import ConferenceRoute from './ConferenceRoute';
import MainRoute from './MainRoute';

export default function AuthenticatedRoutes() {
   const { oidcUser } = useReactOidc();
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(setParticipantId(oidcUser.profile.sub));
      Axios.defaults.headers.common = {
         Authorization: `Bearer ${oidcUser.access_token}`,
      };
   }, [oidcUser]);

   return (
      <Switch>
         <Route exact path="/" component={MainRoute} />
         <Route path="/c/:id" component={ConferenceRoute} />
      </Switch>
   );
}
