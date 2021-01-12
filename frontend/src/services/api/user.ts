import Axios from 'axios';
import { UserInfo } from 'src/features/create-conference/types';

export async function getUserInfo(ids: string[]): Promise<UserInfo[]> {
   const response = await Axios.post<UserInfo[]>('/api/v1/user/list', ids);
   return response.data;
}
