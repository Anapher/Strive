import { Redis } from 'ioredis';
import { Permission } from '../../permissions';

import * as redisKeys from './redis-keys';

export class ParticipantPermissions {
   private redisKey: string;

   constructor(participantId: string, private redis: Redis) {
      this.redisKey = redisKeys.participantPermissions(participantId);
   }

   public async get<T>(perm: Permission<T>): Promise<T | undefined> {
      const value = await this.redis.hget(this.redisKey, perm.key);
      if (!value) return undefined;

      return JSON.parse(value);
   }
}
