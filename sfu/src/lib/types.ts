import {
   DtlsParameters,
   IceCandidate,
   IceParameters,
   SctpCapabilities,
   SctpParameters,
   RtpCapabilities,
} from 'mediasoup/lib/types';

export type ConferenceInfo = {
   id: string;
};

export type ConnectionMessageMetadata = {
   conferenceId: string;
   connectionId: string;
   participantId: string;
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
   rtpCapabilities: RtpCapabilities;
   forceTcp: boolean;
   producing: boolean;
   consuming: boolean;
}>;

export type CreateTransportResponse = ConnectionMessage<{
   id: string;
   iceParameters: IceParameters;
   iceCandidates: IceCandidate[];
   dtlsParameters: DtlsParameters;
   sctpParameters?: SctpParameters;
}>;
