import _ from 'lodash';
import { useState, useEffect, useRef } from 'react';

type SetValueFunc = (value: boolean) => void;

export default function useBooleanThrottle(value: boolean, applyValue: SetValueFunc): SetValueFunc {
   const [eventResetSwitch, setEventResetSwitch] = useState(false);

   const applyValueThrottled = useRef<() => void>();

   useEffect(() => {
      const result = _.throttle(() => {
         applyValue(true);
      }, 10000);

      applyValueThrottled.current = result;

      return () => result.cancel();
   }, [applyValue, eventResetSwitch]);

   useEffect(() => {
      if (!value) setEventResetSwitch((x) => !x);
   }, [value]);

   return (newValue) => {
      if (newValue) {
         if (!applyValueThrottled.current)
            throw new Error('Something went wrong, applyValueThrottled must always be set in the effect');

         applyValueThrottled.current();
      } else {
         if (value) {
            applyValue(false);
         }
      }
   };
}
