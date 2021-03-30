import { useReactOidc } from '@axa-fr/react-oidc-context';
import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Redirect, Route, Switch } from 'react-router-dom';
import { setParticipantId } from 'src/features/auth/reducer';
import ConferenceRoute from './ConferenceRoute';
import MainRoute from './MainRoute';
import Axios from 'axios';

export default function AuthenticatedRoutes() {
   const { oidcUser } = useReactOidc();
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(setParticipantId(oidcUser.profile.sub));
      Axios.defaults.headers.common = {
         Authorization: `Bearer ${oidcUser.access_token}`,
      };
      console.log('Bearer token loaded', oidcUser.access_token);
   }, [oidcUser]);

   return (
      <Switch>
         <Route exact path="/" component={MainRoute} />
         <Route path="/c/:id" component={ConferenceRoute} />
         <Route
            path="/"
            render={({ location }) => {
               if (location.search) {
                  const params = new URLSearchParams(location.search);
                  if (params.get('redirectToConference')) {
                     return <Redirect to={`/c/${params.get('redirectToConference')}`} />;
                  }
               }

               return <Redirect to="/" />;
            }}
         />
      </Switch>
   );
}
