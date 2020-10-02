import { useEffect, useState } from 'react';
import { AsyncFunction, GetPayload, PromiseListener, SetPayload } from 'redux-promise-listener';
import { promiseListener } from 'src/store';
import { getType } from 'typesafe-actions';
import { ActionBuilderConstructor } from 'typesafe-actions/dist/type-helpers';

/**
 * Create an async function from redux actions (e. g. for Formik)
 * @param request the request action that starts the process
 * @param success the success action of the process that resolves the promise
 * @param failure the failure action of the process that rejects the promise
 */
export default function<T1 extends string, T2 extends string, T3 extends string, P1, P2, P3>(
   request: ActionBuilderConstructor<T1, P1>,
   success: ActionBuilderConstructor<T2, P2>,
   failure: ActionBuilderConstructor<T3, P3>,
): ((payload: P1) => Promise<P2>) | null {
   const requestType = getType(request);
   const successType = getType(success);
   const failureType = getType(failure);

   return useAsyncFunction(promiseListener, requestType, successType, failureType);
}

function useAsyncFunction(
   listener: PromiseListener,
   start: string,
   resolve: string,
   reject: string,
   setPayload?: SetPayload,
   getPayload?: GetPayload,
   getError?: GetPayload,
): ((...args: any[]) => Promise<any>) | null {
   const [asyncFunction, setAsyncFunction] = useState<AsyncFunction | null>(null);

   useEffect(() => {
      if (asyncFunction !== null) {
         asyncFunction.unsubscribe();
      }

      const fnc = listener.createAsyncFunction({
         start,
         resolve,
         reject,
         setPayload,
         getPayload,
         getError,
      });

      setAsyncFunction(fnc);

      return () => {
         if (asyncFunction !== null) {
            asyncFunction.unsubscribe();
         }
      };
   }, [listener, start, resolve, reject]);

   return asyncFunction && asyncFunction.asyncFunction;
}
