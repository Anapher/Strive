import { RouterOptions } from 'mediasoup/lib/Router';
import { WebRtcTransportOptions } from 'mediasoup/lib/WebRtcTransport';
import { WorkerSettings } from 'mediasoup/lib/Worker';

const config: Config = {
   mediasoup: {
      numWorkers: 10,
      workerSettings: {
         logLevel: 'debug',
         logTags: ['info', 'ice', 'dtls', 'rtp', 'srtp', 'rtcp', 'rtx', 'bwe', 'score', 'simulcast', 'svc', 'sctp'],
         rtcMinPort: Number(process.env.MEDIASOUP_MIN_PORT) || 40000,
         rtcMaxPort: Number(process.env.MEDIASOUP_MAX_PORT) || 49999,
      },
   },
   router: {
      mediaCodecs: [
         {
            kind: 'audio',
            mimeType: 'audio/opus',
            clockRate: 48000,
            channels: 2,
         },
         {
            kind: 'video',
            mimeType: 'video/VP8',
            clockRate: 90000,
            parameters: {
               'x-google-start-bitrate': 1000,
            },
         },
      ],
   },
   webRtcTransport: {
      options: {
         initialAvailableOutgoingBitrate: 1000000,
         listenIps: [
            {
               ip: process.env.MEDIASOUP_LISTEN_IP || '127.0.0.1',
               announcedIp: process.env.MEDIASOUP_ANNOUNCED_IP,
            },
         ],
         maxSctpMessageSize: 262144,
         enableUdp: true,
         enableTcp: true,
         preferUdp: true,
      },
      maxIncomingBitrate: 1500000,
   },
};

export default config;

type Config = {
   mediasoup: {
      numWorkers: number;
      workerSettings: WorkerSettings;
   };
   router: RouterOptions;
   webRtcTransport: {
      options: WebRtcTransportOptions;
      maxIncomingBitrate?: number;
   };
};
