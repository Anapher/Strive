import React from 'react';
import { useSelector } from 'react-redux';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import useUpdateDeviceLabelsAutomatically from 'src/hooks/useUpdateDeviceLabelsAutomatically';
import { RootState } from 'src/store';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import DesktopLayout from './layouts/DesktopLayout';
import ConferenceNotOpen from './conference-not-open/ConferenceNotOpen';
import ConferenceNotOpenModerator from './conference-not-open/ConferenceNotOpenModerator';
import RequestUserInteractionView from './RequestUserInteractionView';
import { useMediaQuery, useTheme } from '@material-ui/core';
import MobileLayout from './layouts/MobileLayout';
import ParticipantMicManager from 'src/features/media/components/ParticipantMicManager';

type Props = {
   conference: SynchronizedConferenceInfo;
};

export default function index({ conference }: Props) {
   const userInteractionMade = useSelector((state: RootState) => state.media.userInteractionMade);
   const playSoundOnConferenceOpen = useSelector((state: RootState) => state.settings.obj.conference.playSoundOnOpen);
   const theme = useTheme();
   const mobileLayout = useMediaQuery(theme.breakpoints.down('sm'));

   const myId = useMyParticipantId();
   const isModerator = conference.moderators.includes(myId);

   useUpdateDeviceLabelsAutomatically();

   if (playSoundOnConferenceOpen && !userInteractionMade) {
      return <RequestUserInteractionView />;
   }

   if (!conference.isOpen) {
      return isModerator ? (
         <ConferenceNotOpenModerator conferenceInfo={conference} />
      ) : (
         <ConferenceNotOpen conferenceInfo={conference} />
      );
   }

   if (!userInteractionMade) {
      return <RequestUserInteractionView />;
   }

   return <ParticipantMicManager>{mobileLayout ? <MobileLayout /> : <DesktopLayout />}</ParticipantMicManager>;
}
