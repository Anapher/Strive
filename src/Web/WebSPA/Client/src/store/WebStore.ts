import { Log, StateStore } from 'oidc-client';

export default class WebStore implements StateStore {
   private store = localStorage;
   private prefix = 'oidc.';

   set(key: string, value: any): Promise<void> {
      Log.debug('WebStorageStateStore.set', key);

      key = this.prefix + key;

      this.store.setItem(key, value);

      return Promise.resolve();
   }

   get(key: string): Promise<any> {
      Log.debug('WebStorageStateStore.get', key);

      key = this.prefix + key;
      const item = this.store.getItem(key);
      return Promise.resolve(item);
   }

   remove(key: string): Promise<any> {
      Log.debug('WebStorageStateStore.remove', key);

      key = this.prefix + key;

      const item = this.store.getItem(key);
      this.store.removeItem(key);

      return Promise.resolve(item);
   }

   getAllKeys(): Promise<string[]> {
      Log.debug('WebStorageStateStore.getAllKeys');

      const keys = [];

      for (let index = 0; index < this.store.length; index++) {
         const key = this.store.key(index);

         if (key?.startsWith(this.prefix)) {
            keys.push(key.substr(this.prefix.length));
         }
      }

      return Promise.resolve(keys);
   }
}
