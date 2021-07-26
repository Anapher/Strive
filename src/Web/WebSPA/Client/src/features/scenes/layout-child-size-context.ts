import React from 'react';
import { Size } from 'src/types';

const LayoutChildSizeContext = React.createContext<Size & { topOffset?: number }>({ width: 0, height: 0 });

export default LayoutChildSizeContext;
