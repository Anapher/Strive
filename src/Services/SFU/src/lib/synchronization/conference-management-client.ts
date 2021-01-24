import { ConferenceInfo } from '../types';

/**
 * HTTP client to fetch conference info from the conference management microservice
 */
export class ConferenceManagementClient {
   constructor(private url: string) {}

   public async fetchConference(conferenceId: string): Promise<ConferenceInfo> {
      return { participantPermissions: new Map(), participantToRoom: new Map() };
      throw new Error('');
   }
}
