import { ConferenceParticipantStreamInfo } from '../../../src/lib/conference/pub-types';
import { Consumer } from 'mediasoup/lib/Consumer';
import { ConferenceMessenger } from '../../../src/lib/conference/conference-messenger';
import { StreamInfoRepo } from '../../../src/lib/conference/stream-info-repo';
import { Participant } from '../../../src/lib/participant';

const mockMessenger: () => [ConferenceMessenger, jest.Mock] = () => {
   const updateStreams = jest.fn();
   const messenger = ({ updateStreams } as any) as ConferenceMessenger;

   return [messenger, updateStreams];
};

test('should update streams with empty participants array', async () => {
   const [messenger, fn] = mockMessenger();

   const repo = new StreamInfoRepo(messenger, '123');
   const participants: Participant[] = [];

   await repo.updateStreams(participants.values());

   expect(fn.mock.calls.length).toEqual(1);
   expect(fn.mock.calls[0][1]).toEqual('123');

   const result = fn.mock.calls[0][0] as ConferenceParticipantStreamInfo;
   expect(Object.entries(result).length).toEqual(0);
});

test('should update streams', async () => {
   const [messenger, fn] = mockMessenger();

   const repo = new StreamInfoRepo(messenger, '123');
   const participants: Participant[] = [
      {
         participantId: '123',
         receiveConnection: undefined,
         producers: { mic: { connectionId: '34', producer: { paused: false } as any } },
         connections: [
            {
               connectionId: '34',
               consumers: new Map<string, Consumer>().set('43', {
                  id: '43',
                  paused: true,
                  appData: { participantId: '56', source: 'mic' },
               } as any),
               ...({} as any),
            },
         ],
      },
   ];

   await repo.updateStreams(participants.values());

   expect(fn.mock.calls.length).toEqual(1);

   const result = fn.mock.calls[0][0] as ConferenceParticipantStreamInfo;
   const expected: ConferenceParticipantStreamInfo = {
      '123': {
         consumers: {
            '43': { paused: true, participantId: '56', source: 'mic' },
         },
         producers: {
            mic: {
               paused: false,
            },
         },
      },
   };
   expect(result).toEqual(expected);
});

test('should not update streams when frozen and update once when unfrozen', async () => {
   const [messenger, fn] = mockMessenger();

   const repo = new StreamInfoRepo(messenger, '123');
   const participants: Participant[] = [
      { participantId: '123', receiveConnection: undefined, producers: {}, connections: [] },
   ];

   const unfreeze = repo.freeze();

   await repo.updateStreams([].values());
   await repo.updateStreams(participants.values());

   expect(fn.mock.calls.length).toEqual(0);

   unfreeze();

   expect(fn.mock.calls.length).toEqual(1);

   const result = fn.mock.calls[0][0] as ConferenceParticipantStreamInfo;
   expect(Object.entries(result).length).toEqual(1);
});
