export function mapToObject<T>(map: Map<string, T>): { [key: string]: T } {
   const obj = Object.create(null);
   for (let [k, v] of map) {
      obj[k] = v;
   }

   return obj;
}

export function objectToMap<T>(obj: { [key: string]: T }): Map<string, T> {
   const strMap = new Map<string, T>();
   for (let k of Object.keys(obj)) {
      strMap.set(k, obj[k]);
   }

   return strMap;
}
