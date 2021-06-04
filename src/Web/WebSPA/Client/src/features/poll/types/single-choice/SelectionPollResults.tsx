import { useTheme } from '@material-ui/core';
import { ResponsiveBar } from '@nivo/bar';
import React from 'react';
import { PollResultsProps } from '../types';

export default function SelectionPollResults({ viewModel }: PollResultsProps) {
   const theme = useTheme();
   if (viewModel.results?.results.type !== 'selection') return null;

   const maxAnswers = Math.ceil(Math.max(viewModel.results.participantsAnswered, 5) / 5) * 5;

   return (
      <div style={{ height: '100%' }}>
         <ResponsiveBar
            data={Object.entries(viewModel.results.results.options).map(([option, answers]) => ({
               option,
               count: answers.length,
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
            }}
            axisBottom={{
               tickSize: 5,
               tickPadding: 5,
               tickRotation: 0,
               legend: `${viewModel.results.participantsAnswered} votes`,
               legendPosition: 'middle',
               legendOffset: 40,
            }}
            axisLeft={{
               tickSize: 5,
               tickPadding: 5,
               tickRotation: 0,
               legend: 'Antworten',
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
