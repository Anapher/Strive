import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ConferenceInfo } from '../types';
import ClassConference from './ClassConference';
import ConferenceNotOpen from './ConferenceNotOpen';
import RequestUserInteractionView from './RequestUserInteractionView';

type Props = {
   conference: ConferenceInfo;
};
export default function index({ conference }: Props) {
   const userInteractionMade = useSelector((state: RootState) => state.media.userInteractionMade);

   if (!conference.isOpen) {
      return <ConferenceNotOpen conferenceInfo={conference} />;
   }

   if (!userInteractionMade) {
      return <RequestUserInteractionView />;
   }

   switch (conference.conferenceType) {
      default:
         return <ClassConference />;
   }
}
