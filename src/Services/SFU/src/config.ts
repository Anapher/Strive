import { RouterOptions } from 'mediasoup/lib/Router';
import { WebRtcTransportOptions } from 'mediasoup/lib/WebRtcTransport';
import { WorkerSettings } from 'mediasoup/lib/Worker';
import * as os from 'os';

const config: Config = {
   mediasoup: {
      numWorkers: Object.keys(os.cpus()).length,
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
         listenIps: [
            {
               ip: process.env.MEDIASOUP_LISTEN_IP || '127.0.0.1',
               announcedIp: process.env.MEDIASOUP_ANNOUNCED_IP,
            },
         ],
         initialAvailableOutgoingBitrate: 1000000,
         maxSctpMessageSize: 262144,
         enableUdp: true,
         enableTcp: true,
         preferUdp: true,
      },
      maxIncomingBitrate: 1500000,
   },
   http: {
      port: Number(process.env.HTTP_PORT) || 3000,
   },
   services: {
      conferenceInfoRequestUrl:
         process.env.API_CONFERENCE_MANAGEMENT || 'http://localhost:55104/v1/sfu/{conferenceId}?apiKey=testApiKey',
      rabbitMq: process.env.AMQP_CONNECTION_STRING || 'amqp://localhost:5672',
      tokenSecret: process.env.API_TOKEN_SECRET || 'ae2c687f875a47c99e3a05096badbac6',
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
   http: {
      port: number;
   };
   services: {
      conferenceInfoRequestUrl: string;
      rabbitMq: string;
      tokenSecret: string;
   };
};
