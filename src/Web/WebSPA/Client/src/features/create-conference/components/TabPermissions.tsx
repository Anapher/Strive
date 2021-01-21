import { Box, Divider, FormControlLabel, Radio, RadioGroup } from '@material-ui/core';
import React, { useState } from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import { ConferenceDataForm } from '../form';
import { PermissionType } from '../types';
import PermissionList from './PermissionList';

type Props = {
   form: UseFormMethods<ConferenceDataForm>;
};

type PermissionTypeButton = {
   label: string;
   type: PermissionType;
};

const permissionTypeButtons: PermissionTypeButton[] = [
   { label: 'All', type: 'conference' },
   { label: 'Moderators', type: 'moderator' },
   { label: 'Breakout Room', type: 'breakoutRoom' },
];

export default function TabPermissions({ form: { control } }: Props) {
   const [permissionType, setPermissionType] = useState<PermissionType>('conference');

   const handleChangePermissionType = (event: React.ChangeEvent<HTMLInputElement>) => {
      setPermissionType(event.target.value as PermissionType);
   };

   return (
      <Box display="flex" flexDirection="column" height="100%">
         <Box display="flex" justifyContent="space-between" mx={3} mt={3}>
            <RadioGroup row value={permissionType} onChange={handleChangePermissionType}>
               {permissionTypeButtons.map(({ label, type }) => (
                  <FormControlLabel key={type} value={type} control={<Radio />} label={label} />
               ))}
            </RadioGroup>
         </Box>
         <Divider style={{ marginTop: 8 }} />
         <Controller
            control={control}
            name="permissions"
            render={({ value, onChange }) => (
               <PermissionList
                  inherited={permissionType === 'conference' ? {} : value['conference']}
                  value={value[permissionType]}
                  onChange={(newValue) => onChange({ ...value, [permissionType]: newValue })}
               />
            )}
         />
      </Box>
   );
}
