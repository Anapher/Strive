import Axios from 'axios';
import { StartConferenceRequestDto, StartConferenceResponseDto } from 'MyModels';

export async function create(dto: StartConferenceRequestDto): Promise<StartConferenceResponseDto> {
   const response = await Axios.post<StartConferenceResponseDto>('/api/v1/conference', dto);
   return response.data;
}
