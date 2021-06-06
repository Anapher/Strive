import { LoopbackManager } from './../../../src/lib/rooms/loopback-manager';
import * as Room from '../../../src/lib/rooms/room';
import { ProducerLink } from '../../../src/lib/types';

const mockRoom = () => {
   const mock = {
      join: jest.fn(),
      leave: jest.fn(),
      updateParticipant: jest.fn(),
   };
   jest.spyOn(Room, 'default').mockReturnValue(mock as any);
   return mock;
};

const createLoopbackManager = () =>
   new LoopbackManager(undefined as any, undefined as any, undefined as any, undefined as any);

const createProducer = (id: string) => ({ producer: { id } } as any as ProducerLink);

test('updateParticipant() | no loopback producers | do nothing', async () => {
   const room = mockRoom();
   const loopbackManager = createLoopbackManager();

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: {},
      receiveConnection: undefined,
   });

   expect(room.join.mock.calls.length).toEqual(0);
   expect(room.leave.mock.calls.length).toEqual(0);
   expect(room.updateParticipant.mock.calls.length).toEqual(0);
});

test('updateParticipant() | has producer but not loopback | do nothing', async () => {
   const room = mockRoom();
   const loopbackManager = createLoopbackManager();
   const mic = createProducer('m1');

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { mic },
      receiveConnection: undefined,
   });

   expect(room.join.mock.calls.length).toEqual(0);
});

test('updateParticipant() | has loopback producer | add participant to room', async () => {
   const room = mockRoom();
   const loopbackManager = createLoopbackManager();
   const mic = createProducer('m1');

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { 'loopback-mic': mic },
      receiveConnection: undefined,
   });

   expect(room.join.mock.calls.length).toEqual(1);
});

test('updateParticipant() | already added and new producer | update participant in room', async () => {
   const room = mockRoom();
   const loopbackManager = createLoopbackManager();
   const mic = createProducer('m1');

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { 'loopback-mic': mic },
      receiveConnection: undefined,
   });

   const webcam = createProducer('m2');

   room.join.mockClear();
   room.updateParticipant.mockClear();

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { 'loopback-mic': mic, 'loopback-webcam': webcam },
      receiveConnection: undefined,
   });

   expect(room.join.mock.calls.length).toEqual(0);
   expect(room.updateParticipant.mock.calls.length).toEqual(1);
});

test('updateParticipant() | cleared loopback producers | leave room', async () => {
   const room = mockRoom();
   const loopbackManager = createLoopbackManager();
   const mic = createProducer('m1');

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { 'loopback-mic': mic },
      receiveConnection: undefined,
   });

   await loopbackManager.updateParticipant({
      connections: [],
      participantId: '1',
      producers: { mic },
      receiveConnection: undefined,
   });

   expect(room.leave.mock.calls.length).toEqual(1);
});
