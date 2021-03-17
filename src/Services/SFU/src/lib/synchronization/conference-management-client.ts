import { ConferenceInfo, ConferenceInfoDto } from '../types';
import axios from 'axios';
import { objectToMap } from '../../utils/map-utils';

/**
 * HTTP client to fetch conference info from the conference management microservice
 */
export class ConferenceManagementClient {
   constructor(private conferenceInfoRequestUrl: string) {}

   public async fetchConference(conferenceId: string): Promise<ConferenceInfo> {
      const requestUrl = this.buildRequestUrl(conferenceId);
      const response = await axios.get<ConferenceInfoDto>(requestUrl);

      const result = response.data;
      return {
         participantToRoom: objectToMap(result.participantToRoom),
         participantPermissions: objectToMap(result.participantPermissions),
      };
   }

   private buildRequestUrl(conferenceId: string): string {
      const pattern = this.conferenceInfoRequestUrl;
      return pattern.replace('{conferenceId}', conferenceId);
   }
}
