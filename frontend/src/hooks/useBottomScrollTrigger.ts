import { useScrollTrigger } from '@material-ui/core';
import { UseScrollTriggerOptions } from '@material-ui/core/useScrollTrigger/useScrollTrigger';

export default function useBottomScrollTrigger(props: UseScrollTriggerOptions) {
   return useScrollTrigger({
      ...props,
      getTrigger: (store: any, options: any) => {
         const { disableHysteresis = false, threshold = 100, target } = options;
         const previous = store.current;

         if (target) {
            // Get vertical scroll
            store.current = target.scrollHeight - target.clientHeight - target.scrollTop;
            console.log(store.current);
         }

         if (!disableHysteresis && previous !== undefined) {
            if (store.current < previous) {
               return false;
            }
         }

         return store.current > threshold;
      },
   } as any);
}
