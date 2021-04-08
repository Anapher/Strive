import { mergeDeep } from './object-merge';

test('should merge two objects shallow', () => {
   const source = { a: 1 };
   const target = { b: 2 };

   mergeDeep(target, source);

   expect(target).toEqual({ a: 1, b: 2 });
});

test('should merge two objects deep', () => {
   const source = { a: { a: 1 } };
   const target = { a: { b: 2 } };

   mergeDeep(target, source);

   expect(target).toEqual({ a: { a: 1, b: 2 } });
});

test('should not overwrite shallow', () => {
   const source = { a: 1 };
   const target = { a: 2 };

   mergeDeep(target, source);

   expect(target).toEqual({ a: 2 });
});

test('should not overwrite deep', () => {
   const source = { a: { a: 1 } };
   const target = { a: { a: 2 } };

   mergeDeep(target, source);

   expect(target).toEqual({ a: { a: 2 } });
});

test('should not merge lists', () => {
   const source = { a: ['hello'] };
   const target = { a: [] };

   mergeDeep(target, source);

   expect(target).toEqual({ a: [] });
});

test('should complete objects deep', () => {
   const source = { a: { a: 1, b: 3 } };
   const target = { a: { a: 2 } };

   mergeDeep(target, source);

   expect(target).toEqual({ a: { a: 2, b: 3 } });
});
