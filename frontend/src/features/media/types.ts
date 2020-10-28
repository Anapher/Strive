export type ProducerSource = 'mic' | 'webcam' | 'screen';

export type ConsumerInfo = {
   paused: boolean;
   participantId: string;
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

export type ConferenceParticipantStreamInfo = { [key: string]: ParticipantStreams | undefined };
