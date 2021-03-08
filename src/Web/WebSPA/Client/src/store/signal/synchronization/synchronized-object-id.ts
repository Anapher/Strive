export function parseSynchronizedObjectId(s: string) {
   const split = s.split('?', 2);
   const id = split[0];
   const query = split.length > 1 ? split[1] : '';

   const parameters = parseQueryString(query);
   return { id, parameters };
}

export function synchronizedObjectIdToString(syncObjId: SynchronizedObjectId): string {
   let result = syncObjId.id;

   const parameters = Object.entries(syncObjId.parameters);
   if (parameters.length > 0) {
      result += '?' + buildQueryString(syncObjId.parameters);
   }

   return result;
}

function parseQueryString(s: string): { [key: string]: string } {
   const parts = s
      .split('&')
      .filter((x) => !!x)
      .map((x) => x.split('='));
   return Object.fromEntries(parts);
}

function buildQueryString(parameters: { [key: string]: string }) {
   return Object.entries(parameters)
      .map((x) => `${x[0]}=${x[1]}`)
      .join('&');
}

export type SynchronizedObjectId = { id: string; parameters: { [key: string]: string } };
