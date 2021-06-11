import { hashCode } from './string-utils';

export function getArrayEntryByHashCode<T>(arr: T[], index: string): T {
   return arr[Math.abs(hashCode(index)) % arr.length];
}

export function compareArrays<T>(a1?: T[], a2?: T[]) {
   if (!a1 && !a2) return true;
   if (!a1 || !a2) return false;

   if (a1.length !== a2.length) return false;

   for (let i = 0; i < a1.length; i++) {
      if (a1[i] !== a2[i]) return false;
   }

   return true;
}
