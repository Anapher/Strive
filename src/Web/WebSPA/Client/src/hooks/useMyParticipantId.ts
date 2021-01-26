import { useReactOidc } from '@axa-fr/react-oidc-context';

export default function useMyParticipantId() {
   const { oidcUser } = useReactOidc();
   return oidcUser.profile.sub;
}
