import { createMuiTheme, CssBaseline } from '@material-ui/core';
import { blue, pink } from '@material-ui/core/colors';
import { ThemeProvider } from '@material-ui/styles';
import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
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
      <ThemeProvider theme={theme}>
         <CssBaseline />
         <BrowserRouter>{isAuthenticated ? <AuthenticatedRoutes /> : <AnonymousRoutes />}</BrowserRouter>
      </ThemeProvider>
   );
}

export default App;
