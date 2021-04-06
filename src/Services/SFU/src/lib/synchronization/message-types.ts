import { ChangeParticipantProducerDto, ConferenceInfoUpdateDto } from '../types';

type SfuMessageMeta = { conferenceId: string };

export type ReceivedSfuMessage = MediaStateChanged | ChangeParticipantProducer | ParticipantLeft;

export type MediaStateChanged = { type: 'Update'; payload: ConferenceInfoUpdateDto } & SfuMessageMeta;
export type ChangeParticipantProducer = {
   type: 'ChangeProducer';
   payload: ChangeParticipantProducerDto;
} & SfuMessageMeta;

export type ParticipantLeft = { type: 'ParticipantLeft'; payload: string } & SfuMessageMeta;
