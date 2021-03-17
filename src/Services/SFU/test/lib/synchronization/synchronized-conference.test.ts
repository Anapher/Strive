import { applyUpdate, SynchronizedConference } from '../../../src/lib/synchronization/synchronized-conference';
import { ConferenceInfo, ConferenceInfoUpdate, ConferenceInfoUpdateDto } from '../../../src/lib/types';
import { RabbitChannel } from '../../../src/rabbitmq/rabbit-mq-conn';
import { mapToObject } from '../../../src/utils/map-utils';

test('should applyUpdate() with empty update return initial value', () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', 'room1'),
      participantPermissions: new Map().set('p1', { audio: false }),
   };

   const update: ConferenceInfoUpdate = {
      participantPermissions: new Map(),
      participantToRoom: new Map(),
      removedParticipants: [],
   };

   const result = applyUpdate(conference, update);
   expect(result).not.toBe(conference);
   expect(result).toEqual(conference);
});

test('should applyUpdate() with removed participant change conference', () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', 'room1'),
      participantPermissions: new Map().set('p1', { audio: false }),
   };

   const update: ConferenceInfoUpdate = {
      participantPermissions: new Map(),
      participantToRoom: new Map(),
      removedParticipants: ['p1'],
   };

   const result = applyUpdate(conference, update);
   expect(result.participantPermissions.size).toEqual(0);
   expect(result.participantToRoom.size).toEqual(0);
});

test('should applyUpdate() with added participant change conference', () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map(),
      participantPermissions: new Map(),
   };

   const update: ConferenceInfoUpdate = {
      participantPermissions: new Map().set('p1', { audio: false }),
      participantToRoom: new Map().set('p1', 'room1'),
      removedParticipants: [],
   };

   const result = applyUpdate(conference, update);
   expect(result.participantPermissions.get('p1')).toEqual({ audio: false });
   expect(result.participantToRoom.get('p1')).toEqual('room1');
});

test('should applyUpdate() with changed participant change conference', () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', 'room1'),
      participantPermissions: new Map(),
   };

   const update: ConferenceInfoUpdate = {
      participantPermissions: new Map(),
      participantToRoom: new Map().set('p1', 'room2'),
      removedParticipants: [],
   };

   const result = applyUpdate(conference, update);
   expect(result.participantToRoom.get('p1')).toEqual('room2');
});

function mockChannel(): [RabbitChannel, jest.Mock] {
   const consumeFn = jest.fn();
   const channelMock = {
      sub: {
         consume: consumeFn,
      },
   };

   return [channelMock as any, consumeFn];
}

function callMockConsume(consumeFn: jest.Mock, update: ConferenceInfoUpdateDto) {
   const call = consumeFn.mock.calls[0][1] as any;

   const message = JSON.stringify({
      message: {
         update,
      },
   });

   call({ content: message });
}

test('should subscribe rabbitmq on start', async () => {
   const [channel, consumeFn] = mockChannel();
   const syncConference = new SynchronizedConference(channel as any, 'queue');
   await syncConference.start();

   expect(consumeFn).toBeCalledTimes(1);
});

test('should apply received messages after initialization', async () => {
   const [channel, consumeFn] = mockChannel();
   const syncConference = new SynchronizedConference(channel as any, 'queue');
   await syncConference.start();

   callMockConsume(consumeFn, {
      participantPermissions: {},
      participantToRoom: { p1: 'room1' },
      removedParticipants: [],
   });

   syncConference.initialize({ participantPermissions: new Map(), participantToRoom: new Map() });

   const result = syncConference.conferenceInfo;
   expect(result.participantToRoom).toEqual(new Map().set('p1', 'room1'));
});

test('should apply received messages in order of receiving after initialization', async () => {
   const [channel, consumeFn] = mockChannel();
   const syncConference = new SynchronizedConference(channel as any, 'queue');
   await syncConference.start();

   callMockConsume(consumeFn, {
      participantPermissions: {},
      participantToRoom: { p1: 'room1' },
      removedParticipants: [],
   });

   callMockConsume(consumeFn, {
      participantPermissions: {},
      participantToRoom: { p1: 'room2' },
      removedParticipants: [],
   });

   syncConference.initialize({ participantPermissions: new Map(), participantToRoom: new Map() });

   const result = syncConference.conferenceInfo;
   expect(result.participantToRoom).toEqual(new Map().set('p1', 'room2'));
});

test('should just set the current obj if no messages were received before initialization', async () => {
   const [channel] = mockChannel();
   const syncConference = new SynchronizedConference(channel as any, 'queue');
   await syncConference.start();

   syncConference.initialize({ participantPermissions: new Map(), participantToRoom: new Map().set('p1', 'room2') });

   const result = syncConference.conferenceInfo;
   expect(result.participantToRoom).toEqual(new Map().set('p1', 'room2'));
   expect(result.participantPermissions).toEqual(new Map());
});

test('should immediatly update current value after initialization', async () => {
   const [channel, consumeFn] = mockChannel();
   const syncConference = new SynchronizedConference(channel as any, 'queue');
   await syncConference.start();
   syncConference.initialize({ participantPermissions: new Map(), participantToRoom: new Map() });

   callMockConsume(consumeFn, {
      participantPermissions: {},
      participantToRoom: { p1: 'room1' },
      removedParticipants: [],
   });

   const result = syncConference.conferenceInfo;
   expect(result.participantToRoom).toEqual(new Map().set('p1', 'room1'));
});
