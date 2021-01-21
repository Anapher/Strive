import Axios from 'axios';
import { ConferenceData, CreateConferenceResponse } from 'src/features/create-conference/types';

export async function create(dto: ConferenceData): Promise<CreateConferenceResponse> {
   const response = await Axios.post<CreateConferenceResponse>('/api/v1/conference', dto);
   return response.data;
}

export async function getDefault(): Promise<ConferenceData> {
   const response = await Axios.get<ConferenceData>('/api/v1/conference/default-data');
   return response.data;
}
