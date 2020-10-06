import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import ConferenceRoute from './ConferenceRoute';
import MainRoute from './MainRoute';

export default function AuthenticatedRoutes() {
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
               console.log(location);

               return <Redirect to="/" />;
            }}
         />
      </Switch>
   );
}
