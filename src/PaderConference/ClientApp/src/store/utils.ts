import { compose } from 'redux';

export const composeEnhancers =
   (process.env.NODE_ENV === 'development' &&
      window &&
      (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__) ||
   compose;
