export type ChangeStreamDto = {
   id: string;
   type: 'producer' | 'consumer';
   action: 'pause' | 'resume' | 'close';
};

export type ChangeProducerSourceDto = {
   participantId: string;
   source: ProducerSource;
   action: 'pause' | 'resume' | 'close';
};

export type ProducerDevice = 'mic' | 'webcam' | 'screen';
export type ProducerSource = ProducerDevice | 'loopback-mic' | 'loopback-webcam' | 'loopback-screen';

export const ProducerDevices: ProducerDevice[] = ['mic', 'webcam', 'screen'];

export function isProducerDevice(source: ProducerSource): source is ProducerDevice {
   return ProducerDevices.includes(source as any);
}
