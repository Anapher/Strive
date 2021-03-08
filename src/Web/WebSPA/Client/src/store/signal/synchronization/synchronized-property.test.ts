import { Operation } from 'fast-json-patch';
import { synchronizedPropertyFactory } from './synchronized-property';

test('[SynchronizedByExactId] isTriggeredBy() only for exact id', () => {
   const obj = synchronizedPropertyFactory({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });
   expect(obj.isTriggeredBy('test')).toBeTruthy();
   expect(obj.isTriggeredBy('test2')).toBeFalsy();
   expect(obj.isTriggeredBy('test?hello=true')).toBeFalsy();
});

test('[SynchronizedByExactId] applyState() sets the object in state', () => {
   const obj = synchronizedPropertyFactory({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: true };

   const state = {} as any;
   obj.applyState({ id: 'test', value: syncObj }, state);

   expect(state.testProp).toEqual(syncObj);
});

test('[SynchronizedByExactId] applyPatch() patches the object in state', () => {
   const obj = synchronizedPropertyFactory({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });
   const patch: Operation[] = [{ op: 'add', path: '/isOpen', value: true }];

   const state = { testProp: { isOpen: false } } as any;
   obj.applyPatch({ id: 'test', value: patch }, state);

   expect(state.testProp).toEqual({ isOpen: true });
});

test('[SynchronizedSingle] isTriggeredBy() for all objects with base id', () => {
   const obj = synchronizedPropertyFactory({ type: 'single', baseId: 'test', propertyName: 'testProp' });
   expect(obj.isTriggeredBy('test')).toBeTruthy();
   expect(obj.isTriggeredBy('test2')).toBeFalsy();
   expect(obj.isTriggeredBy('test?hello=true')).toBeTruthy();
});

test('[SynchronizedSingle] applyState() sets the object in state', () => {
   const obj = synchronizedPropertyFactory({ type: 'single', baseId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: true };

   const state = {} as any;
   obj.applyState({ id: 'test?participantId=123', value: syncObj }, state);

   expect(state.testProp).toEqual(syncObj);
});

test('[SynchronizedSingle] applyPatch() patches the object in state', () => {
   const obj = synchronizedPropertyFactory({ type: 'single', baseId: 'test', propertyName: 'testProp' });
   const patch: Operation[] = [{ op: 'add', path: '/isOpen', value: true }];

   const state = { testProp: { isOpen: false } } as any;
   obj.applyPatch({ id: 'test?pid=234', value: patch }, state);

   expect(state.testProp).toEqual({ isOpen: true });
});

test('[SynchronizedMultiple] isTriggeredBy() for all objects with base id', () => {
   const obj = synchronizedPropertyFactory({ type: 'multiple', baseId: 'test', propertyName: 'testProp' });
   expect(obj.isTriggeredBy('test')).toBeTruthy();
   expect(obj.isTriggeredBy('test2')).toBeFalsy();
   expect(obj.isTriggeredBy('test?hello=true')).toBeTruthy();
});

test('[SynchronizedMultiple] applyState() with empty state create object and add new object', () => {
   const obj = synchronizedPropertyFactory({ type: 'multiple', baseId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: true };

   const state = {} as any;
   obj.applyState({ id: 'test?participantId=123', value: syncObj }, state);

   expect(state.testProp).toEqual({ 'test?participantId=123': syncObj });
});

test('[SynchronizedMultiple] applyState() with existing state create object and add new object', () => {
   const obj = synchronizedPropertyFactory({ type: 'multiple', baseId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: true };

   const state = {} as any;
   obj.applyState({ id: 'test?participantId=123', value: syncObj }, state);
   obj.applyState({ id: 'test?participantId=124', value: syncObj }, state);

   expect(state.testProp).toEqual({ 'test?participantId=123': syncObj, 'test?participantId=124': syncObj });
});

test('[SynchronizedMultiple] applyPatch() patches the object in state', () => {
   const obj = synchronizedPropertyFactory({ type: 'multiple', baseId: 'test', propertyName: 'testProp' });
   const patch: Operation[] = [{ op: 'add', path: '/isOpen', value: true }];

   const state = { testProp: { 'test?pid=234': { isOpen: false } } } as any;
   obj.applyPatch({ id: 'test?pid=234', value: patch }, state);

   expect(state.testProp).toEqual({ 'test?pid=234': { isOpen: true } });
});

test('[SynchronizedMultiple] remove() removes only one sync obj', () => {
   const obj = synchronizedPropertyFactory({ type: 'multiple', baseId: 'test', propertyName: 'testProp' });

   const state = { testProp: { 'test?pid=234': { isOpen: false }, test2: {} } } as any;
   obj.remove('test?pid=234', state);

   expect(state.testProp).toEqual({ test2: {} });
});
