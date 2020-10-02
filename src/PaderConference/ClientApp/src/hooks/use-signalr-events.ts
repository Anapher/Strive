import { useEffect } from 'react';
import store from 'src/store';
import { addHandler, removeHandler } from '../store/signalr/actions';
import { useSignalrStatus } from './use-signalr-status';

export default function useSignalrEvents(...events: string[]) {
   const isConnected = useSignalrStatus();

   useEffect(() => {
      if (isConnected) {
         store.dispatch(addHandler(events));
      }

      return () => {
         if (isConnected) {
            store.dispatch(removeHandler(events));
         }
      };
   }, [isConnected]);
}
