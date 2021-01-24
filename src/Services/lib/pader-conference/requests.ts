export async function requestConferenceInfo(conferenceId: string): Promise<ConferenceData> {
   return {};
}

/**
 * Conference data synchronized through integration events
 */
export interface ConferenceData {
   participantToRoom: Map<string, string>;
   participantPermissions: Map<string, { [key: string]: any }>;
}
