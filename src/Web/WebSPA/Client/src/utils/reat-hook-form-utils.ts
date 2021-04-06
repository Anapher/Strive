import { UseFormRegisterReturn } from 'react-hook-form';
import React from 'react';

type UseFormRegisterReturnInputRef = Omit<UseFormRegisterReturn, 'ref'> & {
   inputRef: React.Ref<any>;
};

export function wrapForInputRef({ ref, ...rest }: UseFormRegisterReturn): UseFormRegisterReturnInputRef {
   return { inputRef: ref, ...rest };
}
