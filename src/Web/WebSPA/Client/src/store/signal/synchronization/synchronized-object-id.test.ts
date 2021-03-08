import { parseSynchronizedObjectId, synchronizedObjectIdToString } from './synchronized-object-id';

test.each`
   s                                  | expected
   ${'test'}                          | ${{ id: 'test', parameters: {} }}
   ${'hello_world'}                   | ${{ id: 'hello_world', parameters: {} }}
   ${'hello_world?participantId=123'} | ${{ id: 'hello_world', parameters: { participantId: '123' } }}
   ${'test?a=1&b=2'}                  | ${{ id: 'test', parameters: { a: '1', b: '2' } }}
`('parse "$s", expect to return $expected', ({ s, expected }) => {
   const result = parseSynchronizedObjectId(s);
   expect(result.id).toBe(expected.id);
   expect(result.parameters).toEqual(expected.parameters);
});

test.each`
   expected                           | syncId
   ${'test'}                          | ${{ id: 'test', parameters: {} }}
   ${'hello_world'}                   | ${{ id: 'hello_world', parameters: {} }}
   ${'hello_world?participantId=123'} | ${{ id: 'hello_world', parameters: { participantId: '123' } }}
   ${'test?a=1&b=2'}                  | ${{ id: 'test', parameters: { a: '1', b: '2' } }}
`('parse "$syncId", expect to return $expected', ({ expected, syncId }) => {
   const result = synchronizedObjectIdToString(syncId);
   expect(result).toEqual(expected);
});
