import { useState, useMemo } from 'react';
import useResizeObserver from 'use-resize-observer';
import _ from 'lodash';
import { Size } from 'src/types';

export type UseThrottledResizeObserverHook = [React.RefObject<any>, Partial<Size> | null];

const useThrottledResizeObserver: (wait: number) => UseThrottledResizeObserverHook = (wait) => {
   const [size, setSize] = useState<Partial<Size>>({} as Partial<Size>);
   const onResize = useMemo(() => _.throttle(setSize, wait), [wait]);
   const { ref } = useResizeObserver({ onResize });

   return [ref, size];
};

export default useThrottledResizeObserver;
