import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import MainRoute from './MainRoute';

export default function AuthenticatedRoutes() {
   return (
      <Switch>
         <Route exact path="/" component={MainRoute} />
         <Route path="/" render={() => <Redirect to="/" />} />
      </Switch>
   );
}
