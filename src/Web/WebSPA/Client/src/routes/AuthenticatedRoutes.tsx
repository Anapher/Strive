import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Redirect, Route, Switch } from 'react-router-dom';
import { setParticipantId } from 'src/features/auth/reducer';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import ConferenceRoute from './ConferenceRoute';
import MainRoute from './MainRoute';

export default function AuthenticatedRoutes() {
   const userId = useMyParticipantId();
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(setParticipantId(userId));
   }, [userId]);

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
