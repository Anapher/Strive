import Axios from 'axios';
import { ConferenceLink } from 'src/features/conference/types';

export async function fetch(): Promise<ConferenceLink[]> {
   const response = await Axios.get<ConferenceLink[]>('/api/v1/conference-link');
   return response.data;
}

export async function deleteLink(conferenceId: string): Promise<void> {
   await Axios.delete(`/api/v1/conference-link/${conferenceId}`);
}

export async function patch(conferenceId: string, patch: any): Promise<void> {
   await Axios.patch(`/api/v1/conference-link/${conferenceId}`, patch);
}
