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

test('should overwrite shallow', () => {
   const source = { a: 1 };
   const target = { a: 2 };

   mergeDeep(target, source);

   expect(target).toEqual({ a: 1 });
});

test('should overwrite deep', () => {
   const source = { a: { a: 1 } };
   const target = { a: { a: 2 } };

   mergeDeep(target, source);

   expect(target).toEqual({ a: { a: 1 } });
});
