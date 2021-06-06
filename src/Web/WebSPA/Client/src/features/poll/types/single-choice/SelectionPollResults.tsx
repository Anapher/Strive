import { useTheme } from '@material-ui/core';
import { ResponsiveBar } from '@nivo/bar';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { NivoTooltipContent } from '../NivoTooltip';
import { PollResultsProps } from '../types';

export default function SelectionPollResults({ viewModel: { results, poll } }: PollResultsProps) {
   const theme = useTheme();
   const { t } = useTranslation();

   if (results?.results.type !== 'selection') return null;

   const maxAnswers = Math.ceil(Math.max(results.participantsAnswered, 5) / 5) * 5;

   return (
      <div style={{ height: '100%' }}>
         <ResponsiveBar
            data={Object.entries(results.results.options).map(([option, answers]) => ({
               option,
               count: answers.length,
               tokens: answers,
            }))}
            keys={['count']}
            indexBy="option"
            animate={true}
            margin={{ bottom: 50, left: 50, top: 20 }}
            motionStiffness={90}
            motionDamping={15}
            valueScale={{ type: 'linear' }}
            indexScale={{ type: 'band', round: true }}
            colors={{ scheme: 'nivo' }}
            labelTextColor={{ from: 'color', modifiers: [['darker', 1.6]] }}
            borderColor={{ from: 'color', modifiers: [['darker', 1.6]] }}
            axisTop={null}
            axisRight={null}
            tooltip={({ data: { option, count, tokens } }) => (
               <NivoTooltipContent
                  header={
                     <span>
                        {option}: <strong>{count}</strong>
                     </span>
                  }
                  participantTokens={tokens as any}
                  pollId={poll.id}
               />
            )}
            padding={0.3}
            theme={{
               textColor: theme.palette.text.secondary,
               grid: { line: { stroke: theme.palette.divider } },
               axis: {
                  ticks: {
                     line: {
                        stroke: theme.palette.text.secondary,
                     },
                     text: {
                        fill: theme.palette.text.primary,
                     },
                  },
               },
               tooltip: {
                  container: {
                     backgroundColor: theme.palette.background.paper,
                  },
               },
            }}
            axisBottom={{
               tickSize: 5,
               tickPadding: 5,
               tickRotation: 0,
               legend: t('conference.poll.chart_legend', { count: results.participantsAnswered }),
               legendPosition: 'middle',
               legendOffset: 40,
            }}
            axisLeft={{
               tickSize: 5,
               tickPadding: 5,
               tickRotation: 0,
               legend: t('conference.poll.types.single_choice.axis_legend'),
               legendPosition: 'middle',
               legendOffset: -40,
               tickValues: 5,
            }}
            maxValue={maxAnswers}
            gridYValues={5}
         />
      </div>
   );
}
