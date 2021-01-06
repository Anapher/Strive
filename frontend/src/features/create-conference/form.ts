import { DateTime } from 'luxon';
import { ConferenceData } from './types';

export type UserInfo = {
   name: string;
   id: string;
};

export type ConferenceDataForm = ConferenceData & {
   additionalFormData: {
      enableSchedule: boolean;
      enableStartTime: boolean;
   };
};

export const mapDataToForm: (data: ConferenceData) => ConferenceDataForm = (data) => ({
   ...data,
   configuration: {
      ...data.configuration,
      scheduleCron: data.configuration.scheduleCron || '0 0 15 ? * MON *',
      startTime: data.configuration.startTime || DateTime.local().plus({ days: 1 }).toFormat("yyyy-MM-dd'T'HH:mm"),
   },
   additionalFormData: {
      enableSchedule: !!data.configuration.scheduleCron,
      enableStartTime: !!data.configuration.startTime,
   },
});

export const mapFormToData: (form: ConferenceDataForm) => ConferenceData = ({
   configuration,
   permissions,
   additionalFormData,
}) => ({
   configuration: {
      ...configuration,
      startTime: additionalFormData.enableStartTime ? configuration.startTime : undefined,
      scheduleCron: additionalFormData.enableSchedule ? configuration.scheduleCron : undefined,
   },
   permissions,
});
