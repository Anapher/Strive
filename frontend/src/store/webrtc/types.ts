export type ChangeStreamDto = {
   id: string;
   type: 'producer' | 'consumer';
   action: 'pause' | 'resume' | 'close';
};

export type ProducerSource = 'mic' | 'webcam' | 'screen';
