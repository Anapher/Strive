import { TFunction } from 'react-i18next';
import { WebRtcHealth } from './useWebRtcHealth';

const webRtcNamespace = 'conference.troubleshooting.webrtc.status';

export const getStatusMessage: (health: WebRtcHealth, t: TFunction<'translation'>) => string = (health, t) => {
   if (health.connection?.status === 'ok' && health.connector.status === 'ok')
      return t(`${webRtcNamespace}.connected.message`);

   if (health.connector.status !== 'ok') {
      if (health.connector.webRtcStatus === 'connecting') return t(`${webRtcNamespace}.connector_connecting.message`);
      if (health.connector.webRtcStatus === 'uninitialized') return t(`${webRtcNamespace}.uninitialized.message`);

      return 'Connected but status not ok'; // should not happen
   }

   if (!health.connection) {
      return 'Connection not found'; // should not happen
   }

   if (
      health.connection.receiveTransport.transportState !== 'connected' &&
      health.connection.sendTransport.transportState === health.connection.receiveTransport.transportState
   )
      return health.connection.receiveTransport.transportState;

   if (health.connection.receiveTransport.transportState !== 'connected') {
      return t(`${webRtcNamespace}.transport_${health.connection.receiveTransport.transportState}.message`, {
         transport: 'receive-transport',
      });
   }
   if (health.connection.sendTransport.transportState !== 'connected') {
      return t(`${webRtcNamespace}.transport_${health.connection.sendTransport.transportState}.message`, {
         transport: 'send-transport',
      });
   }

   return 'ok';
};
