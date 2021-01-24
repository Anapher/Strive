import { Permission } from '../../permissions';
import { ConferenceData } from './requests';

export class ParticipantPermissions {
   constructor(private participantId: string, private data: ConferenceData) {}

   public async get<T>(perm: Permission<T>): Promise<T | undefined> {
      const value = this.data.participantPermissions.get(this.participantId);
      if (!value) return undefined;

      return value[perm.key];
   }
}
