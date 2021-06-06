import Connection from '../../../src/lib/connection';
import * as mediasoupMixer from '../../../src/lib/media-soup/mediasoup-mixer';
import { Participant } from '../../../src/lib/participant';
import { MEDIA_CAN_SHARE_AUDIO, MEDIA_CAN_SHARE_WEBCAM, Permission } from '../../../src/lib/permissions';
import Room from '../../../src/lib/rooms/room';
import { ConferenceRepository } from '../../../src/lib/synchronization/conference-repository';
import { ConferenceInfo, ProducerLink } from '../../../src/lib/types';
import fromEntries from 'object.fromentries';

const createConferenceRepoMock = (conference: ConferenceInfo): ConferenceRepository => {
   return { getConference: jest.fn().mockReturnValue(conference) } as any;
};

const createMediasoupMixerMock = () => {
   const removeReceiveTransport = jest.fn();
   const addReceiveTransport = jest.fn();
   const removeProducer = jest.fn();
   const addProducer = jest.fn();

   const instance = { removeReceiveTransport, addReceiveTransport, removeProducer, addProducer };

   jest.spyOn(mediasoupMixer, 'MediasoupMixer').mockReturnValue(instance as any);

   return instance;
};

const createConn = (connectionId: string) => ({ connectionId } as Connection);
const createProducer = (id: string) => ({ producer: { id } } as any as ProducerLink);

const conferenceWithPermissions = (participantId: string, ...permissions: Permission<boolean>[]): ConferenceInfo => ({
   ...emptyConference,
   participantPermissions: new Map().set(participantId, fromEntries(permissions.map((x) => [x.key, true]))),
});

const emptyConference: ConferenceInfo = { participantPermissions: new Map(), participantToRoom: new Map() };
const roomId = '123';
const conferenceId = '1';

test('join() | active receive connection without permissions | add to mixer', async () => {
   const conferenceRepo = createConferenceRepoMock(emptyConference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant = {
      connections: [],
      participantId: '1',
      producers: {},
      receiveConnection: createConn('34'),
   };

   await room.join(participant);

   expect(room.getIsParticipantJoined(participant.participantId)).toEqual(true);
   expect(mixer.addReceiveTransport.mock.calls.length).toEqual(1);
   expect(mixer.addReceiveTransport.mock.calls[0][0]).toEqual(participant.receiveConnection);
});

test('join() | active producer but no permissions | dont add producer to mixer', async () => {
   const conferenceRepo = createConferenceRepoMock(emptyConference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   expect(room.getIsParticipantJoined(participant.participantId)).toEqual(true);
   expect(mixer.addProducer.mock.calls.length).toEqual(0);
});

test('join() | active producer with permissions | add producer to mixer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   expect(room.getIsParticipantJoined(participant.participantId)).toEqual(true);
   expect(mixer.addProducer.mock.calls.length).toEqual(1);
   expect(mixer.addProducer.mock.calls[0][0]).toEqual({
      participantId: participant.participantId,
      producer: participant.producers.mic?.producer,
   });
});

test('updateParticipant() | equal participant | do nothing', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);
   await room.updateParticipant(participant);

   expect(room.getIsParticipantJoined(participant.participantId)).toEqual(true);
   expect(mixer.addProducer.mock.calls.length).toEqual(1);
   expect(mixer.removeProducer.mock.calls.length).toEqual(0);
});

test('updateParticipant() | new producer | add producer to mixer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO, MEDIA_CAN_SHARE_WEBCAM);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic', 'webcam']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   const webcamProducer = createProducer('6');
   await room.updateParticipant({
      ...participant,
      producers: {
         ...participant.producers,
         webcam: webcamProducer,
      },
   });

   expect(room.getIsParticipantJoined(participant.participantId)).toEqual(true);
   expect(mixer.addProducer.mock.calls.length).toEqual(2);
   expect(mixer.addProducer.mock.calls[1][0]).toEqual({
      participantId: participant.participantId,
      producer: webcamProducer.producer,
   });
});

test('updateParticipant() | new producer but no permissions | dont add producer to mixer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic', 'webcam']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   const webcamProducer = createProducer('6');
   await room.updateParticipant({
      ...participant,
      producers: {
         ...participant.producers,
         webcam: webcamProducer,
      },
   });

   expect(mixer.addProducer.mock.calls.length).toEqual(1);
});

test('updateParticipant() | removed producer | remove producer from mixer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   await room.updateParticipant({
      ...participant,
      producers: {},
   });

   expect(mixer.addProducer.mock.calls.length).toEqual(1);
   expect(mixer.removeProducer.mock.calls.length).toEqual(1);
   expect(mixer.removeProducer.mock.calls[0][0]).toEqual(participant.producers.mic!.producer.id);
});

test('updateParticipant() | removed permissions | remove producer from mixer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);

   const conferenceWithoutPermissions = conferenceWithPermissions('1');
   (conferenceRepo.getConference as any).mockReturnValueOnce(conferenceWithoutPermissions);

   await room.updateParticipant(participant);

   expect(mixer.removeProducer.mock.calls.length).toEqual(1);
   expect(mixer.removeProducer.mock.calls[0][0]).toEqual(participant.producers.mic!.producer.id);
});

test('updateParticipant() | added permissions | add to mixer', async () => {
   const conference = conferenceWithPermissions('1');
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);
   expect(mixer.addProducer.mock.calls.length).toEqual(0);

   const newConferenceInfo = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   (conferenceRepo.getConference as any).mockReturnValueOnce(newConferenceInfo);

   await room.updateParticipant(participant);

   expect(mixer.addProducer.mock.calls.length).toEqual(1);
});

test('updateParticipant() | new receive transport | update old transport and subscribe new one', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);

   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('5'),
      },
      receiveConnection: createConn('old'),
   };

   await room.join(participant);

   const newParticipant: Participant = {
      ...participant,
      receiveConnection: createConn('receiveConn'),
   };
   await room.updateParticipant(newParticipant);

   expect(mixer.addReceiveTransport.mock.calls.length).toEqual(2);
   expect(mixer.removeReceiveTransport.mock.calls.length).toEqual(1);
   expect(mixer.addReceiveTransport.mock.calls[0][0]).toEqual(participant.receiveConnection);
   expect(mixer.addReceiveTransport.mock.calls[1][0]).toEqual(newParticipant.receiveConnection);
   expect(mixer.removeReceiveTransport.mock.calls[0][0]).toEqual('old');
});

test('leave() | not joined | no error', async () => {
   const conferenceRepo = createConferenceRepoMock(emptyConference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {},
      receiveConnection: undefined,
   };

   await room.leave(participant);
});

test('leave() | joined with receive connection | remove receive connection', async () => {
   const conferenceRepo = createConferenceRepoMock(emptyConference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {},
      receiveConnection: createConn('receiveConnId'),
   };

   await room.join(participant);
   await room.leave(participant);

   expect(mixer.removeReceiveTransport.mock.calls[0][0]).toEqual('receiveConnId');
});

test('leave() | joined with producer | remove producer', async () => {
   const conference = conferenceWithPermissions('1', MEDIA_CAN_SHARE_AUDIO);
   const conferenceRepo = createConferenceRepoMock(conference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('producerId'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);
   await room.leave(participant);

   expect(mixer.removeProducer.mock.calls[0][0]).toEqual('producerId');
});

test('leave() | has producer but not activated | dont remove producer', async () => {
   const conference = conferenceWithPermissions('1');
   const conferenceRepo = createConferenceRepoMock(conference);
   const mixer = createMediasoupMixerMock();

   const room = new Room(roomId, undefined as any, undefined as any, conferenceRepo, conferenceId, ['mic']);
   const participant: Participant = {
      connections: [],
      participantId: '1',
      producers: {
         mic: createProducer('producerId'),
      },
      receiveConnection: undefined,
   };

   await room.join(participant);
   await room.leave(participant);

   expect(mixer.removeProducer.mock.calls.length).toEqual(0);
});

// test('getIsParticipantJoined() should return false if participant is not joined', () => {
//    const conferenceRepo = createConferenceRepoMock(emptyConference);
//    const mixer = createMediasoupMixerMock();

//    const room = new Room('123', undefined as any, undefined as any, conferenceRepo as any, '123', ['mic']);
//    expect(room.getIsParticipantJoined('123')).toEqual(false);
// });
