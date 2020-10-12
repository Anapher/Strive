import { Redis } from 'ioredis';
import { onSendMessageToConnection } from '../redis-communication';
import { SendToConnectionDto } from './types';

export interface ISignalWrapper {
   sendToConnection<T>(connectionId: string, methodName: string, payload: T): Promise<void>;
}

export class SignalWrapper implements ISignalWrapper {
   constructor(private redis: Redis, private conferenceId: string) {}

   async sendToConnection<T>(connectionId: string, methodName: string, payload: T): Promise<void> {
      const message: SendToConnectionDto<T> = {
         connectionId,
         methodName,
         payload,
      };

      await this.redis.publish(onSendMessageToConnection.getName(this.conferenceId), JSON.stringify(message));
   }
}
