import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import ConferenceRoute from '../ConferenceRoute';
import MainRoute from './MainRoute';

export default function AuthenticatedRoutes() {
   return (
      <Switch>
         <Route exact path="/" component={MainRoute} />
         <Route path="/c/" component={ConferenceRoute} />
         <Route path="/" render={() => <Redirect to="/" />} />
      </Switch>
   );
}
