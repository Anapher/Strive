import _ from 'lodash';
import { useRef, useState } from 'react';

type UseAutoHideControls = {
   showControls: boolean;
   setAutoHide: (value: boolean) => void;
   handleMouseMove: () => void;
};

export default function useAutoHideControls(delayMs: number): UseAutoHideControls {
   const [showControls, setShowControls] = useState(true);
   const autoHideControls = useRef<boolean | undefined>(undefined);

   const delayHideControlsFactory = () =>
      _.debounce(() => {
         if (autoHideControls.current) {
            setShowControls(false);
         }
      }, delayMs);

   const delayHideControls = useRef<_.DebouncedFunc<() => void>>(delayHideControlsFactory());

   return {
      showControls,
      setAutoHide: (value) => {
         autoHideControls.current = value;

         if (value) {
            delayHideControls.current();
         } else {
            setShowControls(true);
         }
      },
      handleMouseMove: () => {
         if (!autoHideControls.current) return;

         setShowControls(true);
         delayHideControls.current();
      },
   };
}
