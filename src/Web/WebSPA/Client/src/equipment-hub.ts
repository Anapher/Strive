import { connectSignal } from './store/signal/actions';
import appSettings from './config';
import { createHubFn } from './core-hub';
import { InitializeEquipmentDto, EquipmentStatus } from './equipment-hub.types';
import { DomainError } from './communication-types';

export const joinConference = (conferenceId: string, participantId: string, token: string, defaultEvents: string[]) =>
   connectSignal(appSettings.equipmentSignalrHubUrl, { conferenceId, participantId, token }, defaultEvents, {
      conferenceId,
   });

export const initialize = createHubFn<InitializeEquipmentDto>('Initialize');
export const updateStatus = createHubFn<EquipmentStatus>('UpdateStatus');
export const errorOccurred = createHubFn<DomainError>('ErrorOccurred');

export const events = {
   onConnectionError: 'OnConnectionError',
   onRequestDisconnect: 'OnRequestDisconnect',
   onEquipmentCommand: 'OnEquipmentCommand',
};
