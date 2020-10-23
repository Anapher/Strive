import { useContext } from 'react';
import { SoupManagerContext } from './SoupManagerProvider';

const useSoupManager = () => useContext(SoupManagerContext);

export default useSoupManager;
