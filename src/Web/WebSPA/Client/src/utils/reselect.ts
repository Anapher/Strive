import { isEqual } from 'lodash';
import { createSelectorCreator, defaultMemoize } from 'reselect';
import { compareArrays } from './array-utils';

export const createDeepEqualSelector = createSelectorCreator(defaultMemoize, isEqual);

const compareObjectsWithArraySupport = (a: any, b: any) => {
   if (Array.isArray(a) && Array.isArray(b)) {
      return compareArrays(a, b);
   }

   return a === b;
};
export const createArrayEqualSelector = createSelectorCreator(defaultMemoize, compareObjectsWithArraySupport);
