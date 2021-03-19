import { Participant } from './../../../src/lib/participant';
import * as loopbackManager from '../../../src/lib/rooms/loopback-manager';
import * as Room from '../../../src/lib/rooms/room';
import { RoomManager } from '../../../src/lib/rooms/room-manager';
import { ConferenceRepository } from '../../../src/lib/synchronization/conference-repository';
import { ConferenceInfo } from '../../../src/lib/types';

type MockRoom = {
   join: jest.Mock;
   leave: jest.Mock;
   updateParticipant: jest.Mock;
   participants: Map<string, any>;
};

const mockRoom = () => {
   const rooms: MockRoom[] = [];

   spyOn(Room, 'default').and.callFake(() => {
      const participants = new Map<string, any>();
      const mock = {
         join: jest.fn().mockImplementation((p: Participant) => participants.set(p.participantId, 'joined')),
         leave: jest.fn().mockImplementation((p: Participant) => participants.delete(p.participantId)),
         updateParticipant: jest.fn(),
         participants,
      };
      rooms.push(mock);
      return mock;
   });

   return rooms;
};

const createConferenceRepoMock = (rooms: {
   [id: string]: string;
}): [ConferenceRepository, (rooms: { [id: string]: string }) => void] => {
   const info: ConferenceInfo = {
      participantPermissions: new Map(),
      participantToRoom: new Map(Object.entries(rooms)),
   };

   const getConference = jest.fn().mockReturnValue(info);
   const changeConference = (rooms: { [id: string]: string }) =>
      getConference.mockReturnValueOnce({
         participantPermissions: new Map(),
         participantToRoom: new Map(Object.entries(rooms)),
      });

   return [{ getConference } as any, changeConference];
};

const mockLoopbackManager = () => {
   const updateParticipant = jest.fn();
   spyOn(loopbackManager, 'LoopbackManager').and.returnValue({ updateParticipant });
   return updateParticipant;
};

const createParticipant = (participantId: string) => {
   return { participantId } as Participant;
};

test('updateParticipant() | participant is not mapped to a room | dont add participant to room but update in loopback manager', async () => {
   const rooms = mockRoom();
   const [conferenceRepo] = createConferenceRepoMock({});
   const loopback = mockLoopbackManager();

   const roomManager = new RoomManager('', undefined as any, undefined as any, conferenceRepo);

   const participant = createParticipant('123');
   await roomManager.updateParticipant(participant);

   expect(loopback.mock.calls.length).toEqual(1);
   expect(rooms.length).toEqual(0);
});

test('updateParticipant() | two participants mapped to same room | add both to same room', async () => {
   const rooms = mockRoom();
   const [conferenceRepo] = createConferenceRepoMock({ p1: 'room1', p2: 'room1' });
   mockLoopbackManager();

   const roomManager = new RoomManager('', undefined as any, undefined as any, conferenceRepo);

   const participant1 = createParticipant('p1');
   await roomManager.updateParticipant(participant1);

   const participant2 = createParticipant('p2');
   await roomManager.updateParticipant(participant2);

   expect(rooms.length).toEqual(1);
   expect(rooms[0].join.mock.calls.length).toEqual(2);
});

test('updateParticipant() | participant change room | remove old room, add to new room', async () => {
   const rooms = mockRoom();
   const [conferenceRepo, changeValue] = createConferenceRepoMock({ p1: 'room1' });
   mockLoopbackManager();

   const roomManager = new RoomManager('', undefined as any, undefined as any, conferenceRepo);

   const participant = createParticipant('p1');
   await roomManager.updateParticipant(participant);

   changeValue({ p1: 'room2' });
   await roomManager.updateParticipant(participant);

   expect(rooms.length).toEqual(2);
   expect(rooms[0].join.mock.calls.length).toEqual(1);
   expect(rooms[0].leave.mock.calls.length).toEqual(1);
   expect(rooms[1].join.mock.calls.length).toEqual(1);
});

test('updateParticipant() | participant unset room | remove from room', async () => {
   const rooms = mockRoom();
   const [conferenceRepo, changeValue] = createConferenceRepoMock({ p1: 'room1' });
   mockLoopbackManager();

   const roomManager = new RoomManager('', undefined as any, undefined as any, conferenceRepo);

   const participant = createParticipant('p1');
   await roomManager.updateParticipant(participant);

   changeValue({});
   await roomManager.updateParticipant(participant);

   expect(rooms.length).toEqual(1);
   expect(rooms[0].join.mock.calls.length).toEqual(1);
   expect(rooms[0].leave.mock.calls.length).toEqual(1);
});
