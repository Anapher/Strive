import { useEffect, useState } from 'react';
import { AsyncFunction, GetPayload, PromiseListener, SetPayload } from 'redux-promise-listener';
import { promiseListener } from 'src/store';
import {
   ActionCreator,
   ActionCreatorBuilder,
   ActionCreatorTypeMetadata,
   getType,
   TypeConstant,
} from 'typesafe-actions';

/**
 * Create an async function from redux actions (e. g. for Formik)
 * @param request the request action that starts the process
 * @param success the success action of the process that resolves the promise
 * @param failure the failure action of the process that rejects the promise
 */
export default function <T1 extends TypeConstant, T2 extends TypeConstant, T3 extends TypeConstant, P, R>(
   request: ActionCreatorBuilder<T1, P>,
   success: ActionCreatorBuilder<T2, R>,
   failure: ActionCreator<T3> & ActionCreatorTypeMetadata<T3>,
): ((payload: P) => Promise<R>) | null {
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
