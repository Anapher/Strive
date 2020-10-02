import { ThemeProvider } from '@material-ui/styles';
import { RootState } from 'pader-conference';
import React from 'react';
import { connect } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import AnonymousRoutes from './routes/anonymous';
import AuthenticatedRoutes from './routes/authenticated';
import { CssBaseline, createMuiTheme } from '@material-ui/core';

const theme = createMuiTheme({
   palette: {
      type: 'dark',
   },
});

const mapStateToProps = (state: RootState) => ({ isAuthenticated: state.auth.isAuthenticated });

type Props = ReturnType<typeof mapStateToProps>;

function App({ isAuthenticated }: Props) {
   return (
      <ThemeProvider theme={theme}>
         <CssBaseline />
         <BrowserRouter>{isAuthenticated ? <AuthenticatedRoutes /> : <AnonymousRoutes />}</BrowserRouter>
      </ThemeProvider>
   );
}

export default connect(mapStateToProps)(App);
