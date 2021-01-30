import Axios from 'axios';
import appSettings from 'src/config';
import { ConferenceData, CreateConferenceResponse } from 'src/features/create-conference/types';

export async function create(dto: ConferenceData): Promise<CreateConferenceResponse> {
   const response = await Axios.post<CreateConferenceResponse>(`${appSettings.conferenceUrl}/api/v1/conference`, dto);
   return response.data;
}

export async function getDefault(): Promise<ConferenceData> {
   const response = await Axios.get<ConferenceData>(`${appSettings.conferenceUrl}/api/v1/conference/default-data`);
   return response.data;
}
