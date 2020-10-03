declare module 'MyModels' {
   export type AccessInfo = Readonly<{
      accessToken: string;
      refreshToken: string;
   }>;

   export interface SignInRequest {
      userName: string;
      password: string;
      rememberMe: boolean;
   }

   export type AccessToken = {
      nameid: string;
      unique_name: string;
      role: 'mod' | 'usr';
   };
}
