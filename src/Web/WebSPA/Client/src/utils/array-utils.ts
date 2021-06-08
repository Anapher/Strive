import { hashCode } from './string-utils';

export function getArrayEntryByHashCode<T>(arr: T[], index: string): T {
   return arr[Math.abs(hashCode(index)) % arr.length];
}
