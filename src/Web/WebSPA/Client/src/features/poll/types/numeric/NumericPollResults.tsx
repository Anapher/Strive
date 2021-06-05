import { Theme, useTheme } from '@material-ui/core';
import { Theme as NivoTheme } from '@nivo/core';
import { ResponsiveSwarmPlot } from '@nivo/swarmplot';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';
import NivoTooltip from '../NivoTooltip';
import { PollResultsProps } from '../types';

const getNivoTheme: (theme: Theme) => NivoTheme = (theme) => ({
   textColor: theme.palette.text.secondary,
   axis: {
      ticks: {
         line: {
            stroke: theme.palette.text.primary,
         },
      },
   },
   grid: {
      line: {
         stroke: theme.palette.divider,
      },
   },
});

export default function NumericPollResults({ viewModel: { poll, results } }: PollResultsProps) {
   if (results?.results.type !== 'numeric') return null;
   if (poll.instruction.type !== 'numeric') throw new Error('Invalid instruction type');

   const theme = useTheme();
   const { t } = useTranslation();

   const data = _(Object.entries(results.results.answers))
      .groupBy((x) => x[1])
      .map((value, key) => ({
         key: Number(key),
         count: value.length,
         tokens: value.map((x) => x[0]),
         group: 'A',
      }))
      .value();

   const maxCount = _(data)
      .orderBy((x) => x.count)
      .last()?.count;

   return (
      <div style={{ height: '100%' }}>
         <ResponsiveSwarmPlot
            data={data}
            groups={['A']}
            value="key"
            identity="key"
            valueScale={{
               type: 'linear',
               min: poll.instruction.min ?? undefined,
               max: poll.instruction.max ?? undefined,
               reverse: false,
            }}
            tooltip={({
               node: {
                  data: { key, count, tokens },
               },
            }: any) => (
               <NivoTooltip
                  header={
                     <span>
                        {key}: <strong>{count}</strong>
                     </span>
                  }
                  participantTokens={tokens}
                  pollId={poll.id}
               />
            )}
            layout="horizontal"
            enableGridY={false}
            axisTop={null}
            axisRight={null}
            axisLeft={null}
            axisBottom={{
               legend: t('conference.poll.chart_legend', { count: results.participantsAnswered }),
               legendPosition: 'middle',
               legendOffset: 50,
            }}
            theme={getNivoTheme(theme)}
            margin={{
               top: 30,
               right: 60,
               bottom: 80,
               left: 60,
            }}
            size={{ key: 'count', values: [0, (maxCount ?? 1) + 5], sizes: [6, 50] }}
         />
      </div>
   );
}
