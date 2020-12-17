import {
   DtlsParameters,
   IceCandidate,
   IceParameters,
   ProducerOptions,
   RtpCapabilities,
   SctpCapabilities,
   SctpParameters,
} from 'mediasoup/lib/types';
import { SuccessOrError } from './communication-types';
import { ProducerSource } from './participant';

export type ConferenceInfo = {
   id: string;
};

export type ConnectionMessageMetadata = {
   conferenceId: string;
   connectionId: string;
   participantId: string;
};

export type CallbackMessage<TPayload> = {
   callbackChannel: string;
   payload: TPayload;
};

export type CallbackResponse<TPayload> = SuccessOrError<TPayload>;

export type ConnectionMessage<TPayload> = {
   meta: ConnectionMessageMetadata;
   payload: TPayload;
};

export type InitializeConnectionRequest = ConnectionMessage<{
   sctpCapabilities: SctpCapabilities;
   rtpCapabilities: RtpCapabilities;
}>;

export type CreateTransportRequest = ConnectionMessage<{
   sctpCapabilities: SctpCapabilities;
   forceTcp: boolean;
   producing: boolean;
   consuming: boolean;
}>;

export type ConnectTransportRequest = ConnectionMessage<{
   transportId: string;
   dtlsParameters: any;
}>;

export type TransportProduceRequest = ConnectionMessage<
   {
      transportId: string;
   } & ProducerOptions
>;

export type StreamType = 'producer' | 'consumer';

export type ChangeStreamRequest = ConnectionMessage<{
   id: string;
   type: 'producer' | 'consumer';
   action: 'pause' | 'resume' | 'close';
}>;

export type CreateTransportResponse = {
   id: string;
   iceParameters: IceParameters;
   iceCandidates: IceCandidate[];
   dtlsParameters: DtlsParameters;
   sctpParameters?: SctpParameters;
};

export type TransportProduceResponse = {
   id: string;
};

export type SendToConnectionDto<T> = {
   payload: T;
   connectionId: string;
   methodName: string;
};

export type ConsumerInfo = {
   paused: boolean;
   participantId: string;
   loopback?: boolean;
};

export type ProducerInfo = {
   paused: boolean;
   selected: boolean;
   kind?: ProducerSource;
};

export type ParticipantStreams = {
   consumers: {
      [key: string]: ConsumerInfo;
   };
   producers: { [key: string]: ProducerInfo };
};

export type ConferenceParticipantStreamInfo = { [key: string]: ParticipantStreams };
