export const rtpCapabilitiesKey = (conferenceId: string) => `${conferenceId}::routerRtpCapabilities`;

export const newConferencesKey = 'newConferences';

export const channels = {
   newConferenceCreated: 'newConferenceCreated',
   request: {
      initializeConnection: createChannelName('/req::initializeConnection'),
      createTransport: createChannelName('/req::createTransport'),
   },
   response: {
      createTransport: createChannelName('/res::createTransport'),
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
