import { useRef } from 'react';
import { useSelector } from 'react-redux';

export default function useSelectorFactory<TState, TSelector = any, TSelected = any>(
   factoryFn: () => TSelector,
   selectFunction: (state: TState, selector: TSelector) => TSelected,
) {
   const selector = useRef(factoryFn()).current;
   return useSelector((state: TState) => selectFunction(state, selector));
}
