export const rtpCapabilitiesKey = createChannelName('::routerRtpCapabilities');
export const onSendMessageToConnection = createChannelName('::sendMessageToConnection');
export const onClientDisconnected = createChannelName('::clientDisconnected');
export const onRoomSwitched = createChannelName('::roomSwitched');

export const channels = {
   newConferenceCreated: 'newConferenceCreated',
   audioObserver: createChannelName('::audioObserver'),
   streamsChanged: createChannelName('::streamsChanged'),
   request: {
      initializeConnection: createChannelName('/req::initializeConnection'),
      createTransport: createChannelName('/req::createTransport'),
      connectTransport: createChannelName('/req::connectTransport'),
      transportProduce: createChannelName('/req::transportProduce'),
      changeStream: createChannelName('/req::changeStream'),
      changeProducerSource: createChannelName('/req::changeProducerSource'),
   },
};

function createChannelName(postFix: string): ChannelName {
   return {
      match: (s) => s.endsWith(postFix),
      getName: (id) => id + postFix,
   };
}

export type ChannelName = {
   match: (s: string) => boolean;
   getName: (conferenceId: string) => string;
};
