import { useReactOidc } from '@axa-fr/react-oidc-context';
import React, { useEffect } from 'react';
import { useHistory } from 'react-router-dom';

export default function RedirectToConference() {
   const history = useHistory();
   const { oidcUser } = useReactOidc();

   const redirectUrl = oidcUser.state?.url;

   useEffect(() => {
      if (oidcUser.state?.url) {
         if (history.location.pathname !== oidcUser.state?.url) {
            history.replace(oidcUser.state.url);
         }
      }
   }, [redirectUrl]);

   return null;
}
