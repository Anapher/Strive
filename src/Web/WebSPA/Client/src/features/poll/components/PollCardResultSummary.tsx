import { Box, Chip } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { PollViewModel } from '../types';

type Props = {
   viewModel: PollViewModel;
};

export default function PollCardResultSummary({ viewModel }: Props) {
   const { t } = useTranslation();
   return (
      <Box display="flex" justifyContent="center">
         <Chip
            size="small"
            label={
               t('conference.poll.desc_poll_open_not') +
               (viewModel.results &&
                  ' | ' + t('conference.poll.chart_legend', { count: viewModel.results.participantsAnswered }))
            }
         />
      </Box>
   );
}
