import { MediaKind, RtpCapabilities, RtpParameters } from 'mediasoup-client/lib/RtpParameters';
import { SctpCapabilities, SctpParameters } from 'mediasoup-client/lib/SctpParameters';
import { DtlsParameters, IceCandidate, IceParameters } from 'mediasoup-client/lib/Transport';

export type ChangeStreamRequest = {
   id: string;
   type: 'producer' | 'consumer';
   action: 'pause' | 'resume' | 'close';
};

export type ChangeProducerSourceRequest = {
   source: ProducerSource;
   action: 'pause' | 'resume' | 'close';
};

export type ProducerDevice = 'mic' | 'webcam' | 'screen';
export type ProducerSource = ProducerDevice | 'loopback-mic' | 'loopback-webcam' | 'loopback-screen';

export const ProducerDevices: ProducerDevice[] = ['mic', 'webcam', 'screen'];

export function isProducerDevice(source: ProducerSource): source is ProducerDevice {
   return ProducerDevices.includes(source as any);
}

export type InitializeConnectionRequest = {
   sctpCapabilities: SctpCapabilities;
   rtpCapabilities: RtpCapabilities;
};

export type CreateTransportRequest = {
   sctpCapabilities?: SctpCapabilities;
   forceTcp?: boolean;
   producing: boolean;
   consuming: boolean;
};

export type CreateTransportResponse = {
   id: string;
   iceParameters: IceParameters;
   iceCandidates: IceCandidate[];
   dtlsParameters: DtlsParameters;
   sctpParameters?: SctpParameters;
};

export type ConnectTransportRequest = {
   transportId: string;
   dtlsParameters: any;
};

export type TransportProduceRequest = {
   transportId: string;

   // producer options
   /**
    * Producer id (just for Router.pipeToRouter() method).
    */
   id?: string;

   /**
    * Media kind ('audio' or 'video').
    */
   kind: MediaKind;

   /**
    * RTP parameters defining what the endpoint is sending.
    */
   rtpParameters: RtpParameters;

   /**
    * Whether the producer must start in paused mode. Default false.
    */
   paused?: boolean;

   /**
    * Just for video. Time (in ms) before asking the sender for a new key frame
    * after having asked a previous one. Default 0.
    */
   keyFrameRequestDelay?: number;

   /**
    * Custom application data.
    */
   appData?: any;
};

export type TransportProduceResponse = {
   id: string;
};
