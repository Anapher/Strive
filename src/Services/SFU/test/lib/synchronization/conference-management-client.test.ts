import axios from 'axios';
import { ConferenceManagementClient } from '../../../src/lib/synchronization/conference-management-client';

jest.mock('axios');

test('should send a get request to conference, empty response', async () => {
   const response = {
      participantToRoom: {},
      participantPermissions: {},
   };
   (axios.get as any).mockImplementationOnce(() => Promise.resolve({ data: response }));

   const client = new ConferenceManagementClient('http://localhost/api/{conferenceId}?apiKey=test');
   const result = await client.fetchConference('123');

   expect(axios.get).toHaveBeenCalledWith(`http://localhost/api/123?apiKey=test`);
   expect(result.participantPermissions).toEqual(new Map());
   expect(result.participantToRoom).toEqual(new Map());
});

test('should send a get request to conference, response with participant', async () => {
   const response = {
      participantToRoom: {
         p1: '234',
      },
      participantPermissions: {
         p1: { audio: true, webcam: false, screen: false },
      },
   };
   (axios.get as any).mockImplementationOnce(() => Promise.resolve({ data: response }));

   const client = new ConferenceManagementClient('http://localhost/api/{conferenceId}?apiKey=test');
   const result = await client.fetchConference('123');

   expect(result.participantPermissions).toEqual(new Map().set('p1', response.participantPermissions.p1));
   expect(result.participantToRoom).toEqual(new Map().set('p1', response.participantToRoom.p1));
});
