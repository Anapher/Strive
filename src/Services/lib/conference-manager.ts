import { Conference } from './conference';
import * as requests from './pader-conference/requests';

/**
 * Manages the local conferences this SFU handles
 */
export default class ConferenceManager {
   private conferences: Map<string, Conference> = new Map();

   async getConference(id: string): Promise<Conference | undefined> {
      let localConference = this.conferences.get(id);
      if (localConference) return localConference;

      const data = await requests.requestConferenceInfo(id);
      localConference = new Conference();
   }

   hasConference(id: string): boolean {
      return this.conferences.has(id);
   }
}
