import Axios from 'axios';
import { ConferenceData, ConferencePermissions, CreateConferenceResponse } from 'src/features/create-conference/types';

export async function create(dto: ConferenceData): Promise<CreateConferenceResponse> {
   const response = await Axios.post<CreateConferenceResponse>('/api/v1/conference', dto);
   return response.data;
}

export async function getDefaultPermissions(): Promise<ConferencePermissions> {
   const response = await Axios.get<ConferencePermissions>('/api/v1/conference/default-permissions');
   return response.data;
}
