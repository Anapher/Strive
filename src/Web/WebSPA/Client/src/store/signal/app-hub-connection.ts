import { HubConnection } from '@microsoft/signalr';
import { TypedEmitter } from 'tiny-typed-emitter';

interface AppHubConnectionEvents {
   update: () => void;
}

class AppHubConnection extends TypedEmitter<AppHubConnectionEvents> {
   private connection: HubConnection | undefined;

   get current() {
      return this.connection;
   }

   public register(hub: HubConnection) {
      this.connection = hub;
      this.emit('update');
   }

   public remove() {
      this.connection = undefined;
      this.emit('update');
   }
}

const appHubConn = new AppHubConnection();

export default appHubConn;
