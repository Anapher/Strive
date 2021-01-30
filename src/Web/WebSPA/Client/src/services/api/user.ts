import Axios from 'axios';
import appSettings from 'src/config';
import { UserInfo } from 'src/features/create-conference/types';

export async function getUserInfo(ids: string[]): Promise<UserInfo[]> {
   const response = await Axios.post<UserInfo[]>(`${appSettings.identityUrl}/api/v1/user/list`, ids);
   return response.data;
}
