import { Theme, useTheme } from '@material-ui/core';
import { Theme as NivoTheme } from '@nivo/core';
import { ResponsiveSwarmPlot } from '@nivo/swarmplot';
import _ from 'lodash';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { PollResultsProps } from '../types';

const getNivoTheme: (theme: Theme) => NivoTheme = (theme) => ({
   textColor: theme.palette.text.secondary,
   axis: {
      ticks: {
         line: {
            stroke: 'rgb(84, 39, 136)',
         },
      },
   },
   grid: {
      line: {
         stroke: theme.palette.primary.light,
      },
   },
});

export default function NumericPollResults({ viewModel: { poll, results } }: PollResultsProps) {
   if (results?.results.type !== 'numeric') return null;
   if (poll.instruction.type !== 'numeric') throw new Error('Invalid instruction type');

   const theme = useTheme();
   const participants = useSelector(selectParticipants);

   const data = _(Object.entries(results.results.answers))
      .groupBy((x) => x[1])
      .map((value, key) => ({
         key: Number(key),
         count: value.length,
         group: 'A',
      }))
      .value();

   const maxCount = _(data)
      .orderBy((x) => x.count)
      .last()?.count;

   console.log(data);

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
            layout="horizontal"
            enableGridY={false}
            axisTop={null}
            axisRight={null}
            axisLeft={null}
            axisBottom={{
               legend: `custom node rendering with donut charts using usePie() React hook from @nivo/pie package`,
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
            size={{ key: 'count', values: [0, maxCount ?? 1], sizes: [6, 50] }}
         />
      </div>
   );
}
