import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import AuthRoute from './AuthRoute';

export default function AnonymousRoutes() {
   return (
      <Switch>
         <Route exact path="/login" component={AuthRoute} />
         <Route
            path="/c/:id"
            render={({
               match: {
                  params: { id },
               },
            }) => <Redirect to={`/login?redirectToConference=${id}`} />}
         />
         <Route path="/" render={() => <Redirect to="/login" />} />
      </Switch>
   );
}
