import { events } from 'src/core-hub';
import { onEventOccurred } from './actions';
import { SUBSCRIPTIONS } from './synchronization/synchronized-object-ids';
import { synchronizeObjectState } from './synchronized-object';

const syncObjStateEvent = onEventOccurred(events.onSynchronizeObjectState).type;
const syncObjUpdatedEvent = onEventOccurred(events.onSynchronizedObjectUpdated).type;

test('synchronized object state, expect property to be set', () => {
   const reducer = synchronizeObjectState({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: false };

   const state = {};

   reducer[syncObjStateEvent](state, { payload: { id: 'test', value: syncObj } });

   expect(state).toEqual({ testProp: syncObj });
});

test('synchronized object state, invalid synchronized object id', () => {
   const reducer = synchronizeObjectState({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });
   const syncObj = { isOpen: false };

   const state = {};
   reducer[syncObjStateEvent](state, { payload: { id: 'test2', value: syncObj } });

   expect(state).toEqual({});
});

test('update object state, patch state', () => {
   const reducer = synchronizeObjectState({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });

   const state = { testProp: { isOpen: true } };
   reducer[syncObjUpdatedEvent](state, {
      payload: { id: 'test', value: [{ op: 'add', path: '/isOpen', value: false }] },
   });

   expect(state).toEqual({ testProp: { isOpen: false } });
});

test('update subscriptions, remove sync obj', () => {
   const reducer = synchronizeObjectState({ type: 'exactId', syncObjId: 'test', propertyName: 'testProp' });

   const state = { testProp: { isOpen: true } };
   reducer[syncObjUpdatedEvent](state, {
      payload: { id: SUBSCRIPTIONS, value: [{ op: 'remove', path: '/subscriptions/test' }] },
   });

   expect(state).toEqual({});
});
