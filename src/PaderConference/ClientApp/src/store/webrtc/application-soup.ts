import { EventEmitter } from 'events';
import { SoupManager } from './SoupManager';

class ApplicationSoup {
   private soupManager: SoupManager | undefined;

   public eventEmitter = new EventEmitter();

   get getSoupManager() {
      return this.soupManager;
   }

   public registerSoupManager(soupManager: SoupManager) {
      this.soupManager = soupManager;
      this.eventEmitter.emit('update');
   }

   public uregisterSoupManager() {
      this.soupManager = undefined;
      this.eventEmitter.emit('update');
   }
}

const app = new ApplicationSoup();

export default app;
