import React from 'react';
import { WebRtcManager } from './WebRtcManager';

export const WebRtcContext = React.createContext<WebRtcManager>(new WebRtcManager());
