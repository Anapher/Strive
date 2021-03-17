import { Channel } from 'amqplib';
import { ConferenceRepository } from '../../../src/lib/synchronization/conference-repository';
import { ConferenceInfoUpdateDto } from '../../../src/lib/types';
import { RabbitChannel } from '../../../src/rabbitmq/rabbit-mq-conn';
import { ConferenceInfo } from './../../../src/lib/types';

class TestEnvironment {
   private rabbitChannel: RabbitChannel | undefined;
   private consumeFn: jest.Mock | undefined;
   private resetHandler: undefined | (() => Promise<void>);

   constructor(public apiConference: ConferenceInfo) {
      this.createRabbitConn();
   }

   private createRabbitConn() {
      const sub = this.createChannelMock();
      const pub = this.createChannelMock();

      this.consumeFn = sub.consume as any;

      this.rabbitChannel = {
         sub,
         pub,
      };
   }

   private createChannelMock(): Channel {
      const assertExchange = jest.fn();
      const assertQueue = jest.fn();
      const bindQueue = jest.fn();
      const consume = jest.fn();

      assertQueue.mockReturnValue({ queue: 'test' });
      return { assertExchange, assertQueue, bindQueue, consume } as any;
   }

   get repository(): ConferenceRepository {
      const client = {
         fetchConference: () => this.apiConference,
      } as any;

      const rabbit = {
         getChannel: () => Promise.resolve(this.rabbitChannel),
         on: (ev: string, handler: () => any) => {
            if (ev === 'reset') this.resetHandler = handler;
         },
      } as any;
      return new ConferenceRepository(client, rabbit);
   }

   public callUpdate(update: ConferenceInfoUpdateDto) {
      const call = this.consumeFn!.mock.calls[0][1] as any;

      const message = JSON.stringify({
         message: {
            update,
         },
      });

      call({ content: message });
   }

   public async resetConnection(): Promise<void> {
      this.createRabbitConn();
      if (this.resetHandler) await this.resetHandler();
   }
}

test('should initialize and return the conference on getConference()', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', '123'),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   const result = await env.repository.getConference('123');

   expect(result).toBeTruthy();
   expect(result.participantToRoom).toEqual(new Map().set('p1', '123'));
});

test('should request api if rabbit connection changed', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', '123'),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   await env.repository.getConference('123');

   env.apiConference = { participantPermissions: new Map(), participantToRoom: new Map() };
   env.resetConnection();

   const result2 = await env.repository.getConference('123');
   expect(result2.participantToRoom).toEqual(new Map());
});

test('should emit message event if rabbit message arrives', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map(),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   let received = false;
   await env.repository.addMessageHandler('123', () => {
      received = true;
   });

   env.callUpdate({ participantPermissions: {}, participantToRoom: {}, removedParticipants: [] });
   expect(received).toEqual(true);
});

test('should emit message event after rabbit mq connection changed if rabbit message arrives', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map(),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   let received = false;
   await env.repository.addMessageHandler('123', () => {
      received = true;
   });

   await env.resetConnection();

   env.callUpdate({ participantPermissions: {}, participantToRoom: {}, removedParticipants: [] });
   expect(received).toEqual(true);
});

test('should emit message event after rabbit mq connection changed if rabbit message arrives', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map(),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   let received = false;
   await env.repository.addMessageHandler('123', () => {
      received = true;
   });

   await env.resetConnection();

   env.callUpdate({ participantPermissions: {}, participantToRoom: {}, removedParticipants: [] });
   expect(received).toEqual(true);
});

test('should emit message event after rabbit mq connection changed with new api obj', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map(),
      participantPermissions: new Map(),
   };
   const env = new TestEnvironment(conference);

   let received = false;
   await env.repository.addMessageHandler('123', (update) => {
      received = true;
      expect(update.removedParticipants).toEqual([]);
      expect(update.participantToRoom).toEqual(new Map().set('p1', 'room1'));
      expect(update.participantPermissions).toEqual(new Map());
   });

   env.apiConference = { participantToRoom: new Map().set('p1', 'room1'), participantPermissions: new Map() };

   await env.resetConnection();
   expect(received).toEqual(true);
});

test('should emit message event after rabbit mq connection changed with removed participants', async () => {
   const conference: ConferenceInfo = {
      participantToRoom: new Map().set('p1', 'room1'),
      participantPermissions: new Map().set('p2', { audio: false }),
   };
   const env = new TestEnvironment(conference);

   let received = false;
   await env.repository.addMessageHandler('123', (update) => {
      received = true;
      expect(update.removedParticipants.sort()).toEqual(['p1', 'p2'].sort());
      expect(update.participantToRoom).toEqual(new Map());
      expect(update.participantPermissions).toEqual(new Map());
   });

   env.apiConference = { participantToRoom: new Map(), participantPermissions: new Map() };

   await env.resetConnection();
   expect(received).toEqual(true);
});
