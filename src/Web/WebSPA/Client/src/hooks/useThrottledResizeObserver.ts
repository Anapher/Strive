import { useState, useMemo } from 'react';
import useResizeObserver from 'use-resize-observer';
import _ from 'lodash';
import { Size } from 'src/types';

export type UseThrottledResizeObserverHook = [React.RefObject<any>, Size | undefined];

function observedSizeToSize(setSize: (size: Size | undefined) => void): (size: Partial<Size>) => void {
   return (size) => {
      if (typeof size.width === 'number' && typeof size.height === 'number') {
         setSize(size as Size);
      } else {
         setSize(undefined);
      }
   };
}

const useThrottledResizeObserver: (wait: number) => UseThrottledResizeObserverHook = (wait) => {
   const [size, setSize] = useState<Size | undefined>(undefined);
   const onResize = useMemo(() => _.throttle(observedSizeToSize(setSize), wait), [wait]);
   const { ref } = useResizeObserver({ onResize });

   return [ref as any, size];
};

export default useThrottledResizeObserver;
