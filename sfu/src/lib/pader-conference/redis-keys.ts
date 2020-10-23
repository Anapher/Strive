export const participantPermissions = (participantId: string): string => `participantPermissions::${participantId}`;

export const participantToRoom = (conferenceId: string): string => `${conferenceId}::participantToRoom`;

export const newConferences = 'newConferences';

export const conferenceStreams = (conferenceId: string): string => `${conferenceId}::streams`;

export const openConferences = 'openConferences';
