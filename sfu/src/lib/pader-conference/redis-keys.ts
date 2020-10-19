export const participantPermissions = (participantId: string): string => `participantPermissions::${participantId}`;

export const participantToRoom = (conferenceId: string): string => `${conferenceId}::participantToRoom`;

export const newConferences = 'newConferences';
