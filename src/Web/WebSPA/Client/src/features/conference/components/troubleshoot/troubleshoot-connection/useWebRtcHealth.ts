import { ConnectionState, Transport } from 'mediasoup-client/lib/types';
import { useContext, useEffect, useState } from 'react';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import { WebRtcContext } from 'src/store/webrtc/WebRtcContext';
import { WebRtcStatus } from 'src/store/webrtc/WebRtcManager';
import { ComponentHealth, HealthStatus, mergeHealth } from '../utils';

export type SfuConnectorHealth = ComponentHealth & { webRtcStatus: WebRtcStatus; latestError: any | undefined };

export function useSfuConnectorHealth(): SfuConnectorHealth {
   const context = useContext(WebRtcContext);
   const [status, setStatus] = useState<WebRtcStatus>(context.status);
   const [latestError, setLatestError] = useState<any | undefined>();

   useEffect(() => {
      const onUpdateStatus = () => setStatus(context.status);
      const onUpdateError = () => setLatestError(context.latestError);

      onUpdateStatus();
      onUpdateError();

      context.on('statuschanged', onUpdateStatus);
      context.on('errorOccurred', onUpdateError);
      return () => {
         context.off('statuschanged', onUpdateStatus);
         context.off('errorOccurred', onUpdateError);
      };
   }, [context]);

   const webRtcStatusToHealthStatus: { [key in WebRtcStatus]: HealthStatus } = {
      connected: 'ok',
      connecting: latestError ? 'error' : 'warn',
      uninitialized: 'warn',
   };

   return { status: webRtcStatusToHealthStatus[status], webRtcStatus: status, latestError };
}

export type SfuConnectionHealth = ComponentHealth & {
   sendTransport: TransportStatus;
   receiveTransport: TransportStatus;
};
export type TransportStatus = ComponentHealth & {
   transportState: 'notInitialized' | ConnectionState;
};

const getTransportStatus: (transport: Transport | null) => TransportStatus = (transport) => ({
   transportState: !transport ? 'notInitialized' : transport.connectionState,
   status:
      transport?.connectionState === 'connected' ? 'ok' : transport?.connectionState !== 'failed' ? 'warn' : 'error',
});

export function useWebRtcConnectionHealth(): SfuConnectionHealth | undefined {
   const connection = useWebRtc();
   const [health, setHealth] = useState<SfuConnectionHealth | undefined>();

   useEffect(() => {
      if (connection) {
         const updateStatus = () => {
            const receiveTransport = getTransportStatus(connection.receiveTransport);
            const sendTransport = getTransportStatus(connection.sendTransport);

            setHealth({ receiveTransport, sendTransport, status: mergeHealth([receiveTransport, sendTransport]) });
         };

         const tokenHolder: { token: NodeJS.Timeout | undefined } = { token: undefined };

         const initialize: (me: any) => void = (me) => {
            updateStatus();

            if (connection.sendTransport && connection.receiveTransport) {
               connection.sendTransport.on('connectionstatechange', updateStatus);
               connection.receiveTransport.on('connectionstatechange', updateStatus);
               tokenHolder.token = undefined;
            } else {
               tokenHolder.token = setTimeout(me, 1000);
            }
         };

         initialize(initialize); // thats a recursive function hack

         return () => {
            if (tokenHolder.token) {
               clearTimeout(tokenHolder.token);
            }
            connection.sendTransport?.off('connectionstatechange', updateStatus);
            connection.receiveTransport?.off('connectionstatechange', updateStatus);
         };
      }
   }, [connection]);

   if (!connection) return undefined;
   return health;
}

export type WebRtcHealth = ComponentHealth & {
   connector: SfuConnectorHealth;
   connection: SfuConnectionHealth | undefined;
};

export default function useWebRtcHealth(): WebRtcHealth {
   const connector = useSfuConnectorHealth();
   const connection = useWebRtcConnectionHealth();

   return { connector, connection, status: mergeHealth([connector, connection]) };
}
