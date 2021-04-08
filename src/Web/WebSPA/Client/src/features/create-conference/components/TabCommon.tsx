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
import React, { useEffect, useRef, useState } from 'react';
import { Controller, UseFormReturn } from 'react-hook-form';
import ScheduleIcon from '@material-ui/icons/Schedule';
import { ConferenceDataForm } from '../form';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import NativeIsoDateInput from 'src/components/NativeIsoDateInput';

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
   form: UseFormReturn<ConferenceDataForm>;
};

export default function TabCommon({
   form: {
      control,
      watch,
      register,
      formState: { errors },
      getValues,
   },
}: Props) {
   const classes = useStyles();
   const startTime: boolean = watch('additionalFormData.enableStartTime');
   const scheduleCron: boolean = watch('additionalFormData.enableSchedule');

   const [cronDesc, setCronDesc] = useState<string | undefined>();
   const currentTimeIso = useRef(DateTime.local().toISO());

   const validateSchedulerCron = (s: string | undefined | null) => {
      if (!scheduleCron) return true;
      if (!s) {
         return 'Cannot be empty.';
      }

      try {
         const desc = cronstrue.toString(s);
         setCronDesc(desc);
         return true;
      } catch (error) {
         return error.toString() as string;
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
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="additionalFormData.enableStartTime"
                     />
                  }
                  label="Start"
               />
               <Controller
                  control={control}
                  name="configuration.startTime"
                  render={({ field }) => (
                     <NativeIsoDateInput
                        disabled={!startTime}
                        id="meeting-time"
                        {...field}
                        min={currentTimeIso.current}
                     />
                  )}
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
                           render={({ field: { onChange, value } }) => (
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
                     error={!!errors.configuration?.scheduleCron}
                     {...wrapForInputRef(
                        register('configuration.scheduleCron', {
                           validate: validateSchedulerCron,
                        }),
                     )}
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
            <Box mt={4}>
               <Typography variant="h6">Chat</Typography>
               <Box>
                  <FormControlLabel
                     control={
                        <Controller
                           render={({ field: { onChange, value } }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="configuration.chat.isGlobalChatEnabled"
                        />
                     }
                     label="Global Chat"
                  />
                  <FormControlLabel
                     control={
                        <Controller
                           render={({ field: { onChange, value } }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="configuration.chat.isRoomChatEnabled"
                        />
                     }
                     label="Room Chat"
                  />
                  <FormControlLabel
                     control={
                        <Controller
                           render={({ field: { onChange, value } }) => (
                              <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                           )}
                           control={control}
                           name="configuration.chat.isPrivateChatEnabled"
                        />
                     }
                     label="Private Chat"
                  />
               </Box>
               <FormControlLabel
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="configuration.chat.isDefaultRoomChatDisabled"
                     />
                  }
                  label="Disable room chat for master room"
               />
               <FormControlLabel
                  control={
                     <Controller
                        render={({ field: { onChange, value } }) => (
                           <Checkbox onChange={(e) => onChange(e.target.checked)} checked={value} />
                        )}
                        control={control}
                        name="configuration.chat.showTyping"
                     />
                  }
                  label="Show participants that are currently typing below chat"
               />
            </Box>
         </Grid>
      </Grid>
   );
}
