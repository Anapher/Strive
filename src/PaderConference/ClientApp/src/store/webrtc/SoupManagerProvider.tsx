import React, { useEffect, useState } from 'react';
import app from './application-soup';
import { SoupManager } from './SoupManager';

export const SoupManagerContext = React.createContext<SoupManager | null>(null);

type Props = {
   children?: any;
};

export default function SoupManagerProvider({ children }: Props) {
   const [soupManager, setSoupManager] = useState<SoupManager | null>(null);

   useEffect(() => {
      const onUpdate = () => setSoupManager(app.getSoupManager || null);

      app.eventEmitter.addListener('update', onUpdate);

      return () => {
         app.eventEmitter.removeListener('update', onUpdate);
      };
   }, []);

   return <SoupManagerContext.Provider value={soupManager}>{children}</SoupManagerContext.Provider>;
}
