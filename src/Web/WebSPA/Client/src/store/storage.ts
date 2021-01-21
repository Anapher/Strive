import _ from 'lodash';
import { Store } from 'redux';
import { RootState } from '.';

const storageKey = 'state';

let localStorageState: Partial<RootState> | null = null;
let sessionStorageState: Partial<RootState> | null = null;

type ExtractStateFn = (state: RootState) => Partial<RootState>;

/**
 * Automatically save parts of the state in the browser storage
 * @param store the redux store
 * @param getLocalStorageState get the part of the state that should be persisted in local storage
 * @param getSessionStorageState get the part of the state that should be persisted in session storage
 */
export function persistState(
   store: Store,
   getLocalStorageState: ExtractStateFn,
   getSessionStorageState: ExtractStateFn,
) {
   store.subscribe(() => {
      const state: RootState = store.getState();

      const newLocalStorageState = getLocalStorageState(state);
      if (!_.isEqual(localStorageState, newLocalStorageState)) {
         localStorage.setItem(storageKey, JSON.stringify(newLocalStorageState));
         localStorageState = newLocalStorageState;
      }

      const newSessionStorageState = getSessionStorageState(state);
      if (!_.isEqual(sessionStorageState, newSessionStorageState)) {
         sessionStorage.setItem(storageKey, JSON.stringify(newSessionStorageState));
         sessionStorageState = newSessionStorageState;
      }
   });
}

/**
 * Load the persisted state from the browser storage
 * @param initialState if only parts of reducers are persisted, the full initial state of these must be provided here, as the
 * reducer does not use initialState if the state is partially there which leads to unexpected states
 */
export function loadState(initialState: Partial<RootState>): Partial<RootState> {
   const localStorageValue = localStorage.getItem(storageKey);
   const sessionStorageValue = sessionStorage.getItem(storageKey);

   if (localStorageValue === null && sessionStorageValue === null) {
      return {};
   }

   const stored = new Array<Partial<RootState>>();
   if (localStorageValue !== null) {
      const localStorageState = JSON.parse(localStorageValue);
      if (localStorageState) stored.push(localStorageState);
   }

   if (sessionStorageValue !== null) {
      const sessionStorageState = JSON.parse(sessionStorageValue);
      if (sessionStorageState) stored.push(sessionStorageState);
   }

   const state = _.merge({}, initialState, ...stored) as Partial<RootState>;
   return state;
}
