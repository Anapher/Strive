import Axios from 'axios';
import { CreateConferenceDto, CreateConferenceResponse } from 'src/features/create-conference/types';

export async function create(dto: CreateConferenceDto): Promise<CreateConferenceResponse> {
   const response = await Axios.post<CreateConferenceResponse>('/api/v1/conference', dto);
   return response.data;
}
