export const rtpCapabilitiesKey = createChannelName('::routerRtpCapabilities');
export const onSendMessageToConnection = createChannelName('::sendMessageToConnection');
export const onClientDisconnected = createChannelName('::clientDisconnected');

export const newConferencesKey = 'newConferences';

export const channels = {
   newConferenceCreated: 'newConferenceCreated',
   request: {
      initializeConnection: createChannelName('/req::initializeConnection'),
      createTransport: createChannelName('/req::createTransport'),
      connectTransport: createChannelName('/req::connectTransport'),
      transportProduce: createChannelName('/req::transportProduce'),
      transportProduceData: createChannelName('/req::transportProduceData'),
   },
};

function createChannelName(postFix: string): ChannelName {
   return {
      match: (s) => s.endsWith(postFix),
      getName: (id) => id + postFix,
   };
}

type ChannelName = {
   match: (s: string) => boolean;
   getName: (conferenceId: string) => string;
};
