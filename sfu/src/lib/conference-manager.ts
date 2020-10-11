import { Conference } from './conference';

export default class ConferenceManager {
   private conferences: Map<string, Conference> = new Map();

   createConference(conference: Conference): void {
      this.conferences.set(conference.conferenceId, conference);
   }

   getConference(id: string): Conference {
      const conference = this.conferences.get(id);
      if (!conference) throw new Error('Conference not registered');

      return conference;
   }
}
