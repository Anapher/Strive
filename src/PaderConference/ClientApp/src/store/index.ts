import { RootAction, RootState, Services } from 'pader-conference';
import { applyMiddleware, createStore } from 'redux';
import { createEpicMiddleware } from 'redux-observable';
import createReduxPromiseListener from 'redux-promise-listener';
import services from '../services';
import rootEpic from './root-epic';
import rootReducer from './root-reducer';
import createMiddleware from './signalr/create-middleware';
import { loadState, persistState } from './storage';
import { composeEnhancers } from './utils';

export const epicMiddleware = createEpicMiddleware<RootAction, RootAction, RootState, Services>({
   dependencies: services,
});

const reduxPromiseListener = createReduxPromiseListener();

const signalrMiddleware = createMiddleware({
   getOptions: state => ({
      accessTokenFactory: () => (state() as RootState).auth.token!.accessToken,
   }),
   url: '/signalr',
});

// configure middlewares
const middlewares = [epicMiddleware, reduxPromiseListener.middleware, signalrMiddleware];

// compose enhancers
const enhancer = composeEnhancers(applyMiddleware(...middlewares));

// rehydrate state on app start
const initialState = loadState({});

// create store
const store = createStore(rootReducer, initialState, enhancer);
persistState(store, persistInLocalStorage, persistInSessionStorage);

epicMiddleware.run(rootEpic);

export const promiseListener = reduxPromiseListener;

// export store singleton instance
export default store;

// Store persistence
function persistInLocalStorage(state: RootState): Partial<RootState> {
   return { auth: state.auth.rememberMe ? state.auth : undefined };
}

function persistInSessionStorage(state: RootState): Partial<RootState> {
   return { auth: !state.auth.rememberMe ? state.auth : undefined };
}
