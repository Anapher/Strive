import { Chip, Portal, TextField } from '@material-ui/core';
import { Autocomplete } from '@material-ui/lab';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import PollCardSubmitButton from '../../components/PollCardSubmitButton';
import { TagCloudAnswer, TagCloudInstruction } from '../../types';
import { PollAnswerFormProps } from '../types';

const createAnswerDto: (tags: string[]) => TagCloudAnswer = (tags) => ({
   type: 'tagCloud',
   tags,
});

export default function TagCloudAnswerForm({
   poll: { poll, answer },
   footerPortalRef,
   onSubmit,
   onDelete,
}: PollAnswerFormProps<TagCloudAnswer>) {
   const { t } = useTranslation();

   if (poll.instruction.type !== 'tagCloud') throw new Error('Tag cloud instruction required');

   const [selectedTags, setSelectedTags] = useState(
      (answer?.answer as TagCloudAnswer | undefined)?.tags ?? new Array<string>(),
   );

   const hasMaxTagsReached = (values: string[]) =>
      Boolean(
         poll.instruction.type === 'tagCloud' && poll.instruction.maxTags && values.length > poll.instruction.maxTags,
      );

   const maxTagsExceeded = hasMaxTagsReached(selectedTags);

   const handleChange = (_: React.ChangeEvent<unknown>, value: string[]) => {
      setSelectedTags(value);

      if (!poll.config.isAnswerFinal && !hasMaxTagsReached(value)) {
         if (value.length === 0) {
            onDelete();
         } else {
            onSubmit(createAnswerDto(value));
         }
      }
   };

   return (
      <>
         <Autocomplete
            multiple
            freeSolo
            disableClearable
            options={new Array<string>()}
            renderTags={(value, getTagProps) =>
               value.map((option: string, index: number) => (
                  <Chip key={option} variant="outlined" label={option} {...getTagProps({ index })} />
               ))
            }
            renderInput={(params) => (
               <TextField
                  {...params}
                  error={maxTagsExceeded}
                  helperText={
                     maxTagsExceeded
                        ? t('conference.poll.types.tag_cloud.error_max_exceeded', {
                             count: selectedTags.length - ((poll.instruction as TagCloudInstruction).maxTags ?? 0),
                          })
                        : t('conference.poll.types.tag_cloud.your_tags_helper')
                  }
                  variant="outlined"
                  label={t('conference.poll.types.tag_cloud.your_tags')}
               />
            )}
            limitTags={5}
            size="small"
            value={selectedTags}
            onChange={handleChange}
            disabled={answer && poll.config.isAnswerFinal}
         />
         {poll.config.isAnswerFinal && !answer && (
            <Portal container={footerPortalRef}>
               <PollCardSubmitButton
                  disabled={selectedTags.length === 0 || maxTagsExceeded}
                  onClick={() => onSubmit(createAnswerDto(selectedTags))}
               />
            </Portal>
         )}
      </>
   );
}
