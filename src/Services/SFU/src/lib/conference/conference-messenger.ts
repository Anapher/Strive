import {
   ConferenceParticipantStreamInfo,
   ConsumerArgs,
   ConsumerCreatedArgs,
   ConsumerLayersChanged,
   ConsumerScoreArgs,
   ProducerChangedEventArgs,
   ProducerScoreInfo,
} from './pub-types';

export interface ConferenceMessenger {
   notifyProducerChanged(connectionId: string, args: ProducerChangedEventArgs): Promise<void>;
   notifyProducerScore(connectionId: string, args: ProducerScoreInfo): Promise<void>;

   notifyConsumerClosed(connectionId: string, args: ConsumerArgs): Promise<void>;
   notifyConsumerPaused(connectionId: string, args: ConsumerArgs): Promise<void>;
   notifyConsumerResumed(connectionId: string, args: ConsumerArgs): Promise<void>;
   notifyConsumerScore(connectionId: string, args: ConsumerScoreArgs): Promise<void>;
   notifyConsumerCreated(connectionId: string, args: ConsumerCreatedArgs): Promise<void>;
   notifyConsumerLayersChanged(connectionId: string, args: ConsumerLayersChanged): Promise<void>;

   updateStreams(arg: ConferenceParticipantStreamInfo, conferenceId: string): Promise<void>;
}
