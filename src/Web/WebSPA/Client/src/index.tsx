import { AuthenticationProvider, InMemoryWebStorage, oidcLog } from '@axa-fr/react-oidc-context';
import 'fontsource-roboto';
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import App from './App';
import { ocidConfig } from './config';
import * as serviceWorker from './serviceWorker';
import store from './store';

ReactDOM.render(
   <Provider store={store}>
      <AuthenticationProvider
         configuration={ocidConfig}
         loggerLevel={oidcLog.DEBUG}
         isEnabled
         UserStore={InMemoryWebStorage}
      >
         <App />
      </AuthenticationProvider>
   </Provider>,
   document.getElementById('root'),
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
