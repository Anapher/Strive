import { HubConnection } from '@microsoft/signalr';
import { EventEmitter } from 'events';

class AppHubConnection {
   private connection: HubConnection | undefined;

   public eventEmitter = new EventEmitter();

   get current() {
      return this.connection;
   }

   public register(hub: HubConnection) {
      this.connection = hub;
      this.eventEmitter.emit('update');
   }

   public remove() {
      this.connection = undefined;
      this.eventEmitter.emit('update');
   }
}

const appHubConn = new AppHubConnection();

export default appHubConn;
