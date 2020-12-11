import LuxonUtils from '@date-io/luxon';
import { createMuiTheme, CssBaseline } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import { ThemeProvider } from '@material-ui/styles';
import { SnackbarProvider } from 'notistack';
import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import UserInteractionListener from './features/media/components/UserInteractionListener';
import Notifier from './features/notifier/components/Notifier';
import AnonymousRoutes from './routes/anonymous';
import AuthenticatedRoutes from './routes/authenticated';
import { RootState } from './store';

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
   const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

   return (
      <MuiPickersUtilsProvider utils={LuxonUtils}>
         <SnackbarProvider maxSnack={4}>
            <ThemeProvider theme={theme}>
               <Notifier />
               <UserInteractionListener />
               <CssBaseline />
               <BrowserRouter>{isAuthenticated ? <AuthenticatedRoutes /> : <AnonymousRoutes />}</BrowserRouter>
            </ThemeProvider>
         </SnackbarProvider>
      </MuiPickersUtilsProvider>
   );
}

export default App;
