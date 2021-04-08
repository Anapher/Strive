import Axios from 'axios';
import { Operation } from 'fast-json-patch';
import appSettings from 'src/config';
import { ConferenceData, CreateConferenceResponse } from 'src/features/create-conference/types';

export async function create(dto: ConferenceData): Promise<CreateConferenceResponse> {
   const response = await Axios.post<CreateConferenceResponse>(`${appSettings.conferenceUrl}/v1/conference`, dto);
   return response.data;
}

export async function getDefault(): Promise<ConferenceData> {
   const response = await Axios.get<ConferenceData>(`${appSettings.conferenceUrl}/v1/conference/default-data`);
   return response.data;
}

export async function get(conferenceId: string): Promise<ConferenceData> {
   const response = await Axios.get<ConferenceData>(`${appSettings.conferenceUrl}/v1/conference/${conferenceId}`);
   return response.data;
}

export async function patch(conferenceId: string, patch: Operation[]): Promise<void> {
   await Axios.patch(`${appSettings.conferenceUrl}/v1/conference/${conferenceId}`, patch);
}
