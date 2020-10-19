import {
   DtlsParameters,
   IceCandidate,
   IceParameters,
   ProducerOptions,
   RtpCapabilities,
   SctpCapabilities,
   SctpParameters,
} from 'mediasoup/lib/types';

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

export type CallbackResponse<TPayload> = {
   error?: boolean;
   errorMesage?: boolean;
   payload?: TPayload;
};

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
