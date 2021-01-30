import { AuthenticationProvider, InMemoryWebStorage, oidcLog, OidcSecure } from '@axa-fr/react-oidc-context';
import LuxonUtils from '@date-io/luxon';
import { createMuiTheme, CssBaseline, makeStyles } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import { ThemeProvider } from '@material-ui/styles';
import React from 'react';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter } from 'react-router-dom';
import UserInteractionListener from './features/media/components/UserInteractionListener';
import appSettings, { ocidConfig } from './config';
import AuthenticatedRoutes from './routes';
import AuthCallback from './features/auth/components/AuthCallback';
import NotAuthenticated from './features/auth/components/NotAuthenticated';
import { User } from 'oidc-client';
import Axios from 'axios';

Axios.defaults.baseURL = appSettings.conferenceUrl;

const useStyles = makeStyles((theme) => ({
   toast: {
      backgroundColor: theme.palette.background.paper,
      color: theme.palette.text.primary,
   },
}));

const theme = createMuiTheme({
   palette: {
      type: 'dark',
      primary: {
         main: blue[500],
      },
      secondary: {
         main: pink[500],
      },
      background: {
         default: 'rgb(20, 20, 22)',
         paper: '#303030',
      },
   },
});

function App() {
   return (
      <MuiPickersUtilsProvider utils={LuxonUtils}>
         <ThemeProvider theme={theme}>
            <MaterialUiToaster />
            <UserInteractionListener />
            <CssBaseline />
            <BrowserRouter>
               <AuthenticationProvider
                  configuration={ocidConfig}
                  loggerLevel={oidcLog.DEBUG}
                  isEnabled
                  UserStore={InMemoryWebStorage}
                  callbackComponentOverride={AuthCallback}
                  notAuthenticated={NotAuthenticated}
                  customEvents={
                     {
                        onUserLoaded: (user: User) =>
                           (Axios.defaults.headers.common = {
                              Authorization: `Bearer ${user.access_token}`,
                           }),
                     } as any
                  }
               >
                  <OidcSecure>
                     <AuthenticatedRoutes />
                  </OidcSecure>
               </AuthenticationProvider>
            </BrowserRouter>
         </ThemeProvider>
      </MuiPickersUtilsProvider>
   );
}

function MaterialUiToaster() {
   const classes = useStyles();

   return <Toaster position="top-center" toastOptions={{ className: classes.toast }} />;
}

export default App;
