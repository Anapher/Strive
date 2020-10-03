declare module 'MyModels' {
   export type ConferenceSettings = {
      allowUsersToUnmute: boolean;
   };

   export type StartConferenceRequestDto = {
      settings?: ConferenceSettings;
   };

   export type StartConferenceResponseDto = {
      conferenceId?: string;
   };
}
