import { Box, Checkbox, FormControlLabel, Grid, makeStyles } from '@material-ui/core';
import { DateTimePicker } from '@material-ui/pickers';
import { DateTime } from 'luxon';
import React from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import { ConferenceData } from '../types';

const checkBoxWidth = 150;
const useStyles = makeStyles(() => ({
   checkBoxLabel: {
      margin: 0,
      width: 150,
   },
}));

type Props = {
   form: UseFormMethods<ConferenceData>;
};

export default function TabCommon({ form: { control, errors, watch } }: Props) {
   const classes = useStyles();
   const state = watch(['scheduleCron', 'conferenceType', 'enableStartTime', 'schedule']);

   return (
      <Grid container>
         <Grid item xs={12}>
            <Box display="flex" flexDirection="row" alignItems="center">
               <FormControlLabel
                  className={classes.checkBoxLabel}
                  control={
                     <Controller
                        render={({ onChange, value }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="enableStartTime"
                     />
                  }
                  label="Start"
               />
               <Controller
                  name="startTime"
                  control={control}
                  rules={{ validate: (x: string) => x }}
                  render={({ onChange, value }) => (
                     <DateTimePicker
                        error={!!errors.configuration?.startTime}
                        disabled={!state.configuration?.startTime}
                        disablePast
                        onChange={(x) => onChange(x?.toISO())}
                        value={value && DateTime.local()}
                        ampm={false}
                     />
                  )}
               />
            </Box>
         </Grid>
      </Grid>
   );
}
