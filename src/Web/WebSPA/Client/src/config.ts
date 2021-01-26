import { UserManagerSettings } from 'oidc-client';

export type AppSettings = {
   identityUrl: string;
   conferenceUrl: string;
   signalrHubUrl: string;
   frontendUrl: string;
};

// injected by ASP.Net Core, normalize urls. All urls don't have a trailing slash
const appSettings: AppSettings = Object.fromEntries(
   Object.entries((window as any).ENV).map(([name, url]) => [name, (url as string).replace(/\/$/, '')]),
) as AppSettings;

export default appSettings;

export const ocidConfig: UserManagerSettings = {
   client_id: 'interactive.public.short',
   redirect_uri: `${appSettings.frontendUrl}/authentication/callback`,
   response_type: 'code',
   post_logout_redirect_uri: `${appSettings.frontendUrl}/`,
   scope: 'openid profile email api offline_access',
   authority: 'https://demo.identityserver.io',
   silent_redirect_uri: 'http://localhost:3000/authentication/silent_callback',
   automaticSilentRenew: true,
   loadUserInfo: true,
};
