import React from 'react';

export type ConferenceLayoutContextType = {
   chatContainer?: React.ReactInstance | null;
   chatWidth: number;
};

const ConferenceLayoutContext = React.createContext<ConferenceLayoutContextType>({ chatWidth: 0 });

export default ConferenceLayoutContext;
