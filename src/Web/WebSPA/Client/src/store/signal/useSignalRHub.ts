import { HubConnection } from '@microsoft/signalr';
import { useEffect, useState } from 'react';
import appHubConn from './app-hub-connection';

export default function useSignalRHub(): HubConnection | undefined {
   const [instance, setInstance] = useState<HubConnection | undefined>(appHubConn.current);
   useEffect(() => {
      const updateHandler = () => {
         setInstance(appHubConn.current);
      };

      appHubConn.on('update', updateHandler);

      return () => {
         appHubConn.off('update', updateHandler);
      };
   }, []);

   return instance;
}
