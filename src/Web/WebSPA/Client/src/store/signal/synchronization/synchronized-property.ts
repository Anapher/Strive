import { applyPatch } from 'fast-json-patch';
import { SyncStatePayload } from 'src/core-hub.types';
import { parseSynchronizedObjectId } from './synchronized-object-id';

export type SynchronizeExactId = { type: 'exactId'; syncObjId: string; propertyName: string };
export type SynchronizeSingle = { type: 'single'; baseId: string; propertyName: string };
export type SynchronizeMultiple = { type: 'multiple'; baseId: string; propertyName: string };

export type SynchronizeProperty = SynchronizeExactId | SynchronizeSingle | SynchronizeMultiple;

export default interface ISynchronizedProperty {
   isTriggeredBy(id: string): boolean;
   applyState(payload: SyncStatePayload, state: any): void;
   applyPatch(payload: SyncStatePayload, state: any): void;
   remove(id: string, state: any): void;
}

class SynchronizedByExactId implements ISynchronizedProperty {
   constructor(private descriptor: SynchronizeExactId) {}

   isTriggeredBy(id: string): boolean {
      return id === this.descriptor.syncObjId;
   }

   applyState({ value }: SyncStatePayload, state: any): void {
      state[this.descriptor.propertyName] = value;
   }

   applyPatch({ value }: SyncStatePayload, state: any): void {
      const currentStateValue = state[this.descriptor.propertyName];
      applyPatch(currentStateValue, value);
   }

   remove(id: string, state: any): void {
      state[this.descriptor.propertyName] = undefined;
   }
}

class SynchronizedSingle implements ISynchronizedProperty {
   constructor(private descriptor: SynchronizeSingle) {}

   isTriggeredBy(id: string): boolean {
      const syncObjId = parseSynchronizedObjectId(id);
      return syncObjId.id === this.descriptor.baseId;
   }

   applyState({ value }: SyncStatePayload, state: any): void {
      state[this.descriptor.propertyName] = value;
   }

   applyPatch({ value }: SyncStatePayload, state: any): void {
      const currentStateValue = state[this.descriptor.propertyName];
      applyPatch(currentStateValue, value);
   }

   remove(): void {
      // dont remove the object as we might have a new default object with just a different id
   }
}

class SynchronizedMultiple implements ISynchronizedProperty {
   constructor(private descriptor: SynchronizeMultiple) {}

   isTriggeredBy(id: string): boolean {
      const syncObjId = parseSynchronizedObjectId(id);
      return syncObjId.id === this.descriptor.baseId;
   }

   applyState({ value, id }: SyncStatePayload, state: any): void {
      const collection = state[this.descriptor.propertyName] || {};
      state[this.descriptor.propertyName] = { ...collection, [id]: value };
   }

   applyPatch({ value, id }: SyncStatePayload, state: any): void {
      const currentStateValue = state[this.descriptor.propertyName][id];
      applyPatch(currentStateValue, value);
   }

   remove(id: string, state: any): void {
      const current = state[this.descriptor.propertyName];
      if (!current) return;

      const newEntries = Object.entries(current).filter(([key]) => key !== id);
      state[this.descriptor.propertyName] = Object.fromEntries(newEntries);
   }
}

export function synchronizedPropertyFactory(request: SynchronizeProperty): ISynchronizedProperty {
   switch (request.type) {
      case 'exactId':
         return new SynchronizedByExactId(request);
      case 'single':
         return new SynchronizedSingle(request);
      case 'multiple':
         return new SynchronizedMultiple(request);
   }
}
