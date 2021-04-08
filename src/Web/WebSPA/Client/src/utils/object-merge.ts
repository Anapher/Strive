/**
 * Simple object check.
 * @param item
 * @returns {boolean}
 */
export function isObject(item?: any): boolean {
   return item && typeof item === 'object' && !Array.isArray(item);
}

/**
 * Deep merge two objects.
 * @param target
 * @param ...sources
 */
export function mergeDeep(target: any, source: any): any {
   if (isObject(target) && isObject(source)) {
      for (const key in source) {
         if (isObject(source[key])) {
            if (!target[key]) {
               Object.assign(target, { [key]: {} });
            }

            mergeDeep(target[key], source[key]);
         } else {
            if (target[key] === null || target[key] === undefined) {
               Object.assign(target, { [key]: source[key] });
            }
         }
      }
   }

   return target;
}
