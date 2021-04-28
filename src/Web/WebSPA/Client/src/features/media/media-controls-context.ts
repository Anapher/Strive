import React from 'react';

export type MediaControlsContextType = {
   leftControlsContainer?: React.ReactInstance | null;
};

const MediaControlsContext = React.createContext<MediaControlsContextType>({});

export default MediaControlsContext;
