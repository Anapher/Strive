import LuxonUtils from '@date-io/luxon';
import { createMuiTheme, CssBaseline } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import { ThemeProvider } from '@material-ui/styles';
import { SnackbarProvider } from 'notistack';
import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
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
         default: '#242424',
         paper: '#383838',
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
               <CssBaseline />
               <BrowserRouter>{isAuthenticated ? <AuthenticatedRoutes /> : <AnonymousRoutes />}</BrowserRouter>
            </ThemeProvider>
         </SnackbarProvider>
      </MuiPickersUtilsProvider>
   );
}

export default App;
