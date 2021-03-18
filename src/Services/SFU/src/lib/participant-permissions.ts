import { Permission } from './permissions';
import { ConferenceInfo } from './types';

export class ParticipantPermissions {
   constructor(private participantId: string, private conferenceInfo: ConferenceInfo) {}

   public get<T>(perm: Permission<T>): T | undefined {
      const value = this.conferenceInfo.participantPermissions.get(this.participantId);
      if (!value) return undefined;

      return value[perm.key];
   }
}
