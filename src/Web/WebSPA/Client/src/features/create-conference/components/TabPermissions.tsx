import { Box, Divider, FormControlLabel, Radio, RadioGroup } from '@material-ui/core';
import { TFunction } from 'i18next';
import React, { useState } from 'react';
import { Controller, UseFormReturn } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ConferenceDataForm } from '../form';
import { PermissionType } from '../types';
import PermissionList from './PermissionList';

type Props = {
   form: UseFormReturn<ConferenceDataForm>;
};

type PermissionTypeButton = {
   label: string;
   type: PermissionType;
};

const permissionTypeButtons: (t: TFunction) => PermissionTypeButton[] = (t) => [
   { label: t('dialog_create_conference.tabs.permissions.all'), type: 'conference' },
   { label: t('common:moderator_plural'), type: 'moderator' },
   { label: t('dialog_create_conference.tabs.permissions.breakout_room'), type: 'breakoutRoom' },
];

export default function TabPermissions({ form: { control } }: Props) {
   const [permissionType, setPermissionType] = useState<PermissionType>('conference');
   const { t } = useTranslation();

   const handleChangePermissionType = (event: React.ChangeEvent<HTMLInputElement>) => {
      setPermissionType(event.target.value as PermissionType);
   };

   return (
      <Box display="flex" flexDirection="column" height="100%">
         <Box display="flex" justifyContent="space-between" mx={3} mt={3}>
            <RadioGroup row value={permissionType} onChange={handleChangePermissionType}>
               {permissionTypeButtons(t).map(({ label, type }) => (
                  <FormControlLabel key={type} value={type} control={<Radio />} label={label} />
               ))}
            </RadioGroup>
         </Box>
         <Divider style={{ marginTop: 8 }} />
         <Controller
            control={control}
            name="permissions"
            render={({ field: { value, onChange } }) => (
               <PermissionList
                  inherited={permissionType === 'conference' ? {} : value['conference']!}
                  value={value[permissionType]!}
                  onChange={(newValue) => onChange({ ...value, [permissionType]: newValue })}
               />
            )}
         />
      </Box>
   );
}
