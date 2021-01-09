import {
   Box,
   Checkbox,
   Collapse,
   FormControlLabel,
   Grid,
   IconButton,
   InputAdornment,
   makeStyles,
   TextField,
   Typography,
} from '@material-ui/core';
import cronstrue from 'cronstrue';
import { DateTime } from 'luxon';
import React, { useEffect, useState } from 'react';
import { Controller, UseFormMethods } from 'react-hook-form';
import ScheduleIcon from '@material-ui/icons/Schedule';
import { ConferenceDataForm } from '../form';

const checkBoxWidth = 150;

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(3),
   },
   checkBoxLabel: {
      width: checkBoxWidth,
   },
}));

type Props = {
   form: UseFormMethods<ConferenceDataForm>;
};

export default function TabCommon({ form: { control, watch, register, errors, getValues } }: Props) {
   const classes = useStyles();
   const startTime: boolean = watch('additionalFormData.enableStartTime');
   const scheduleCron: boolean = watch('additionalFormData.enableSchedule');

   const [cronDesc, setCronDesc] = useState<string | undefined>();

   const validateSchedulerCron = (s: string) => {
      if (!scheduleCron) return true;

      try {
         const desc = cronstrue.toString(s);
         setCronDesc(desc);
         return true;
      } catch (error) {
         return error.toString();
      }
   };

   useEffect(() => {
      if (scheduleCron) validateSchedulerCron(getValues().configuration.scheduleCron ?? '');
   }, [scheduleCron]);

   return (
      <Grid container className={classes.root}>
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
                        name="additionalFormData.enableStartTime"
                     />
                  }
                  label="Start"
               />
               <input
                  ref={register}
                  disabled={!startTime}
                  type="datetime-local"
                  id="meeting-time"
                  name="configuration.startTime"
                  min={DateTime.local().toFormat("yyyy-MM-dd'T'HH:mm")}
               />
            </Box>
         </Grid>
         <Grid item xs={12}>
            <div>
               <Box display="flex" flexDirection="row" alignItems="center">
                  <FormControlLabel
                     className={classes.checkBoxLabel}
                     control={
                        <Controller
                           render={({ onChange, value }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="additionalFormData.enableSchedule"
                        />
                     }
                     label="Schedule"
                  />
                  <TextField
                     style={{ flex: 1 }}
                     disabled={!scheduleCron}
                     placeholder="Cron"
                     name="configuration.scheduleCron"
                     error={!!errors.configuration?.scheduleCron}
                     inputRef={register({ validate: validateSchedulerCron })}
                     InputProps={{
                        endAdornment: (
                           <InputAdornment position="end">
                              <IconButton
                                 disabled={!scheduleCron}
                                 aria-label="open cron expression generator"
                                 title="Cron Expression Generator"
                                 href="https://www.freeformatter.com/cron-expression-generator-quartz.html"
                                 target="_blank"
                              >
                                 <ScheduleIcon />
                              </IconButton>
                           </InputAdornment>
                        ),
                     }}
                  />
               </Box>
               <Collapse in={scheduleCron}>
                  <div style={{ marginLeft: checkBoxWidth }}>
                     <Typography
                        variant="caption"
                        color={errors.configuration?.scheduleCron ? 'error' : 'textSecondary'}
                     >
                        {errors.configuration?.scheduleCron?.message || cronDesc}
                     </Typography>
                  </div>
               </Collapse>
            </div>
         </Grid>
         <Grid item xs={12}>
            <div>
               <Box mt={4}>
                  <Typography variant="subtitle1">Chat</Typography>
                  <FormControlLabel
                     control={
                        <Controller
                           render={({ onChange, value }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="configuration.chat.showTyping"
                        />
                     }
                     label="Show participants that are currently typing below chat"
                  />
               </Box>
            </div>
         </Grid>
      </Grid>
   );
}
