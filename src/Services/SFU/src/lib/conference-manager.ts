import { Conference } from './conference';

export default class ConferenceManager {
   private conferences: Map<string, Conference> = new Map();

   createConference(conference: Conference): void {
      this.conferences.set(conference.conferenceId, conference);
   }

   getConference(id: string): Conference | undefined {
      return this.conferences.get(id);
   }

   hasConference(id: string): boolean {
      return this.conferences.has(id);
   }
}
