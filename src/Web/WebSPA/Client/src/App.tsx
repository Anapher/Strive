import { AuthenticationProvider, oidcLog, OidcSecure } from '@axa-fr/react-oidc-context';
import LuxonUtils from '@date-io/luxon';
import { createMuiTheme, CssBaseline, makeStyles } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import { ThemeProvider } from '@material-ui/styles';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import { ocidConfig } from 'src/config';
import AuthCallback from 'src/features/auth/components/AuthCallback';
import NotAuthenticated from 'src/features/auth/components/NotAuthenticated';
import AuthenticatingComponent from './features/auth/components/AuthenticatingComponent';
import SessionLostComponent from './features/auth/components/SessionLostComponent';
import UserInteractionListener from './features/media/components/UserInteractionListener';
import RedirectToConference from './RedirectToConference';
import AuthenticatedRoutes from './routes/AuthenticatedRoutes';
import EquipmentRoute from './routes/EquipmentRoute';

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
               <Switch>
                  <Route path="/c/:id/as-equipment" exact component={EquipmentRoute} />
                  <Route path="/">
                     <AuthenticationProvider
                        configuration={ocidConfig}
                        loggerLevel={oidcLog.DEBUG}
                        isEnabled
                        callbackComponentOverride={AuthCallback}
                        notAuthenticated={NotAuthenticated}
                        sessionLostComponent={SessionLostComponent}
                        authenticating={AuthenticatingComponent}
                     >
                        <OidcSecure>
                           <AuthenticatedRoutes />
                           <RedirectToConference />
                        </OidcSecure>
                     </AuthenticationProvider>
                  </Route>
               </Switch>
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
