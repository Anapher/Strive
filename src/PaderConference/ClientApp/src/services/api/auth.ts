import Axios from 'axios';
import { AccessInfo } from 'MyModels';

export async function signIn(userName: string, password: string): Promise<AccessInfo> {
   const response = await Axios.post<AccessInfo>('/api/v1/auth/login', {
      userName,
      password,
   });

   return response.data;
}

export async function refreshToken(access: AccessInfo): Promise<AccessInfo> {
   const response = await Axios.post<AccessInfo>('/api/v1/auth/refreshtoken', access);
   return response.data;
}
