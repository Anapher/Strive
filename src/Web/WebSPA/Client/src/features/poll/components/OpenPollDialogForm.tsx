import {
   Box,
   Button,
   Checkbox,
   Chip,
   Collapse,
   DialogActions,
   DialogContent,
   Divider,
   FormControl,
   FormControlLabel,
   Grid,
   InputLabel,
   makeStyles,
   Switch,
   TextField,
   Typography,
} from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { Control, Controller, FieldPath, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import MobileAwareSelect from 'src/components/MobileAwareSelect';
import { CreatePollDto } from 'src/core-hub.types';
import { selectParticipantRoom } from 'src/features/rooms/selectors';
import { wrapForInputRef } from 'src/utils/reat-hook-form-utils';
import getPresets from '../poll-presets';
import pollTypes from '../types/register';

const useStyles = makeStyles((theme) => ({
   divider: {
      margin: theme.spacing(1, 0),
   },
   advancedOptionsHeader: {
      marginTop: theme.spacing(3),
   },
}));

const mapDtoToForm: (dto: CreatePollDto) => CreatePollDto = (dto) => ({
   ...dto,
   instruction: {
      ...dto.instruction,
      options:
         dto.instruction.type === 'singleChoice' || dto.instruction.type === 'multipleChoice'
            ? (dto.instruction.options.join('\n') as any)
            : undefined,
   },
});

const mapFormToDto: (form: CreatePollDto, myRoomId: string | undefined) => CreatePollDto = (form, myRoomId) => {
   const result = {
      ...form,
      instruction: Object.fromEntries(
         Object.entries(form.instruction).filter(
            ([k, v]) =>
               !Number.isNaN(v) &&
               (k !== 'options' ||
                  form.instruction.type === 'singleChoice' ||
                  form.instruction.type === 'multipleChoice'),
         ),
      ) as any,
   };
   if (result.instruction.options) {
      result.instruction.options = (result.instruction.options as string).split(/\r?\n/).filter((x) => x.length > 0);
   }

   if (result.roomId) {
      result.roomId = undefined;
   } else {
      result.roomId = myRoomId;
   }

   if (result.config.question === '') result.config.question = undefined;

   return result;
};

type AdvancedOptionProps<TName extends FieldPath<CreatePollDto>> = {
   label: string;
   helperText: string;
   control: Control<CreatePollDto>;
   name: TName;
};
function AdvancedOption<TName extends FieldPath<CreatePollDto>>({
   label,
   helperText,
   control,
   name,
}: AdvancedOptionProps<TName>) {
   return (
      <>
         <div>
            <FormControlLabel
               control={
                  <Controller
                     render={({ field: { onChange, value } }) => (
                        <Checkbox
                           name={name}
                           onChange={(e) => onChange(e.target.checked)}
                           checked={(value as boolean) || false}
                        />
                     )}
                     control={control}
                     name={name}
                  />
               }
               label={label}
            />
         </div>
         <Typography variant="caption">{helperText}</Typography>
      </>
   );
}

const defaultValues: CreatePollDto = {
   config: { isAnonymous: true, isAnswerFinal: true },
   instruction: { type: 'singleChoice' } as any,
   initialState: { isOpen: true, resultsPublished: false },
};

type Props = {
   open: boolean;
   onSubmit: (dto: CreatePollDto) => void;
};

export default function OpenPollDialogForm({ open, onSubmit }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();
   const presets = getPresets(t);

   const myRoomId = useSelector(selectParticipantRoom);

   const [showAdvanced, setShowAdvanced] = useState(false);

   const form = useForm<CreatePollDto>({ defaultValues });

   useEffect(() => {
      if (open) {
         form.reset(defaultValues);
      }
   }, [open]);

   const { register, control, handleSubmit, watch } = form;

   const instructionType = watch('instruction.type');
   const pollPresenter = pollTypes.find((x) => x.instructionType === instructionType);
   if (!pollPresenter) {
      console.error('Poll presenter not found for ' + instructionType);
      return null;
   }

   const { InstructionForm } = pollPresenter;

   const handleChangeShowAdvanced = (event: React.ChangeEvent<HTMLInputElement>) => {
      setShowAdvanced(event.target.checked);
   };

   const handleApplyPreset = (preset: CreatePollDto) => {
      form.reset(mapDtoToForm(preset));
   };

   const onFormSubmit = (data: CreatePollDto) => {
      const dto = mapFormToDto(data, myRoomId);
      onSubmit(dto);
   };

   return (
      <>
         <DialogContent>
            <Typography variant="caption">{t('conference.poll.create_dialog.presets.title')}:</Typography>
            <Grid container spacing={1}>
               {presets.map((x) => (
                  <Grid item key={x.label}>
                     <Chip label={x.label} size="small" onClick={() => handleApplyPreset(x.data)} />
                  </Grid>
               ))}
            </Grid>
         </DialogContent>
         <Divider className={classes.divider} />
         <DialogContent>
            <form id="open-poll-form" onSubmit={handleSubmit(onFormSubmit)}>
               <TextField
                  label={t('conference.poll.create_dialog.question')}
                  fullWidth
                  {...wrapForInputRef(register('config.question'))}
               />
               <Box mt={3}>
                  <Grid container spacing={4} style={{ width: '100%', margin: 0 }}>
                     <Grid item xs={4}>
                        <Box>
                           <FormControl fullWidth>
                              <InputLabel id="poll-dialog-select-mode-label">
                                 {t('conference.poll.create_dialog.poll_type')}
                              </InputLabel>
                              <Controller
                                 render={({ field: { onChange, value } }) => (
                                    <MobileAwareSelect
                                       labelId="poll-dialog-select-mode-label"
                                       id="poll-dialog-select-mode"
                                       value={value}
                                       onChange={onChange}
                                    >
                                       {pollTypes.map((x) => ({
                                          label: t<string>(x.labelTranslationKey),
                                          value: x.instructionType,
                                       }))}
                                    </MobileAwareSelect>
                                 )}
                                 control={control}
                                 name="instruction.type"
                              />
                           </FormControl>
                        </Box>
                     </Grid>
                     <Grid item xs={8}>
                        <InstructionForm form={form} showAdvanced={showAdvanced} />
                     </Grid>
                  </Grid>

                  <Collapse in={showAdvanced}>
                     <Typography gutterBottom className={classes.advancedOptionsHeader}>
                        {t('conference.poll.create_dialog.advanced_options')}
                     </Typography>
                     <Grid container spacing={1}>
                        <Grid item xs={12} sm={6}>
                           <AdvancedOption
                              control={control}
                              label={t('conference.poll.create_dialog.advanced.anonymous_label')}
                              helperText={t('conference.poll.create_dialog.advanced.anonymous_helper')}
                              name="config.isAnonymous"
                           />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                           <AdvancedOption
                              control={control}
                              label={t('conference.poll.create_dialog.advanced.anwser_final_label')}
                              helperText={t('conference.poll.create_dialog.advanced.anwser_final_helper')}
                              name="config.isAnswerFinal"
                           />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                           <AdvancedOption
                              control={control}
                              label={t('conference.poll.create_dialog.advanced.open_label')}
                              helperText={t('conference.poll.create_dialog.advanced.open_helper')}
                              name="initialState.isOpen"
                           />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                           <AdvancedOption
                              control={control}
                              label={t('conference.poll.create_dialog.advanced.publish_results_label')}
                              helperText={t('conference.poll.create_dialog.advanced.publish_results_helper')}
                              name="initialState.resultsPublished"
                           />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                           <AdvancedOption
                              control={control}
                              label={t('conference.poll.create_dialog.advanced.global_label')}
                              helperText={t('conference.poll.create_dialog.advanced.global_helper')}
                              name="roomId"
                           />
                        </Grid>
                     </Grid>
                  </Collapse>
               </Box>
            </form>
         </DialogContent>
         <Divider />
         <DialogActions style={{ justifyContent: 'space-between' }}>
            <FormControlLabel
               control={<Switch checked={showAdvanced} onChange={handleChangeShowAdvanced} />}
               label={t('conference.poll.create_dialog.advanced_options')}
            />
            <Button type="submit" form="open-poll-form" color="primary">
               {t('conference.poll.create_dialog.submit')}
            </Button>
         </DialogActions>
      </>
   );
}
