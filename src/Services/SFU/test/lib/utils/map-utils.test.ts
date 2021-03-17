import { mapToObject, objectToMap } from '../../../src/utils/map-utils';

const testMap = new Map().set('hello', 'world').set('test1', 'test2');
const testObj = { hello: 'world', test1: 'test2' };

test('should convert object to map', () => {
   const map = objectToMap(testObj);
   expect(map).toEqual(testMap);
});

test('should convert map to object', () => {
   const map = mapToObject(testMap);
   expect(map).toEqual(testObj);
});
