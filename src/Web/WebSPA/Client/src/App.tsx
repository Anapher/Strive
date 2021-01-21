import LuxonUtils from '@date-io/luxon';
import { createMuiTheme, CssBaseline, makeStyles } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import { ThemeProvider } from '@material-ui/styles';
import React from 'react';
import { Toaster } from 'react-hot-toast';
import { useSelector } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import UserInteractionListener from './features/media/components/UserInteractionListener';
import AnonymousRoutes from './routes/anonymous';
import AuthenticatedRoutes from './routes/authenticated';
import { RootState } from './store';

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
   const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

   return (
      <MuiPickersUtilsProvider utils={LuxonUtils}>
         <ThemeProvider theme={theme}>
            <MaterialUiToaster />
            <UserInteractionListener />
            <CssBaseline />
            <BrowserRouter>{isAuthenticated ? <AuthenticatedRoutes /> : <AnonymousRoutes />}</BrowserRouter>
         </ThemeProvider>
      </MuiPickersUtilsProvider>
   );
}

function MaterialUiToaster() {
   const classes = useStyles();

   return <Toaster position="top-center" toastOptions={{ className: classes.toast }} />;
}

export default App;
