import { Action, configureStore, Middleware, ThunkAction } from '@reduxjs/toolkit';
import { persistStore } from 'redux-persist';
import createSagaMiddleware from 'redux-saga';
import notifierMiddleware from './notifier/create-middleware';
import rootReducer from './root-reducer';
import rootSaga from './root-saga';
import createMiddleware from './signal/create-middleware';

const { middleware: signalrMiddleware } = createMiddleware();

const sagaMiddleware = createSagaMiddleware();

// configure middlewares
const middlewares: Middleware[] = [signalrMiddleware, sagaMiddleware, notifierMiddleware];

// create store
const store = configureStore({
   reducer: rootReducer,
   middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(middlewares),
});

export const persistor = persistStore(store);

// run redux saga
sagaMiddleware.run(rootSaga);

// export store singleton instance
export default store;

export type AppThunk = ThunkAction<void, RootState, unknown, Action<string>>;
export type RootState = ReturnType<typeof rootReducer>;
export type AppDispatch = typeof store.dispatch;
