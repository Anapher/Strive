declare module 'MyModels' {
   export type AccessInfo = Readonly<{
      accessToken: string;
      refreshToken: string;
   }>;

   export interface SignInResponse {
      accessInfo: AccessInfo;
      rememberMe: boolean;
   }

   export type AccessToken = {
      nameid: string;
      unique_name: string;
      role: 'mod' | 'usr';
   };

   export type SignInRequest = {
      userName: string;
      password: string;
      rememberMe: boolean;
   };
}
