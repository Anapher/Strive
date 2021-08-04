import { useState, useMemo, RefObject, RefCallback } from 'react';
import useResizeObserver from 'use-resize-observer';
import _ from 'lodash';
import { Size } from 'src/types';

export type UseThrottledResizeObserverHook<T extends HTMLElement> = [Size | undefined, RefCallback<T>];

function observedSizeToSize(setSize: (size: Size | undefined) => void): (size: Partial<Size>) => void {
   return (size) => {
      if (typeof size.width === 'number' && typeof size.height === 'number') {
         setSize(size as Size);
      } else {
         setSize(undefined);
      }
   };
}

function useThrottledResizeObserver<T extends HTMLElement>(
   wait: number,
   opts?: {
      ref?: RefObject<T> | T | null | undefined;
      onResize?: (size: Size | undefined) => void;
   },
): UseThrottledResizeObserverHook<T> {
   const [size, setSize] = useState<Size | undefined>(undefined);

   const onSizeChanged = (size: Size | undefined) => {
      if (opts?.onResize) {
         opts.onResize(size);
      } else {
         setSize(size);
      }
   };

   const onThrottledResize = useMemo(() => _.throttle(observedSizeToSize(onSizeChanged), wait), [wait, opts?.onResize]);
   const { ref } = useResizeObserver<T>({ onResize: onThrottledResize, ref: opts?.ref });

   return [size, ref];
}

export default useThrottledResizeObserver;
