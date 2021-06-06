import { makeStyles, Typography } from '@material-ui/core';
import { scaleOrdinal } from 'd3-scale';
import { schemeDark2 } from 'd3-scale-chromatic';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';
import ReactWordcloud from 'react-wordcloud';
import { hashCode } from 'src/utils/string-utils';
import 'tippy.js/animations/scale.css';
import 'tippy.js/dist/tippy.css';
import { PollResultsProps } from '../types';

const useStyles = makeStyles((theme) => ({
   container: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
   },
   cloud: {
      flex: 1,
      minHeight: 0,
   },
   description: {
      margin: theme.spacing(1, 0),
   },
}));

function getDefaultColors() {
   return Array.from({ length: 23 /** prime */ })
      .map((_, i) => i.toString())
      .map(scaleOrdinal(schemeDark2));
}

const defaultColors = getDefaultColors();

export default function TagCloudResults({ viewModel }: PollResultsProps) {
   const classes = useStyles();
   const { t } = useTranslation();

   if (viewModel.results?.results.type !== 'tagCloud') return null;

   return (
      <div className={classes.container}>
         <div className={classes.cloud}>
            <ReactWordcloud
               words={_.orderBy(
                  Object.entries(viewModel.results.results.tags).map(([text, value]) => ({
                     text,
                     value: value.length,
                  })),
                  (x) => x.text,
               )}
               options={{
                  deterministic: true,
                  rotationAngles: [-45, 45],
                  scale: 'linear',
                  spiral: 'rectangular',
                  fontFamily: 'Impact',
                  fontSizes: [20, 30],
               }}
               callbacks={{
                  getWordColor: (x) => defaultColors[Math.abs(hashCode(x.text)) % defaultColors.length],
               }}
            />
         </div>
         <Typography align="center" color="textSecondary" variant="caption" className={classes.description}>
            {viewModel.results.participantsAnswered}{' '}
            {t('common:participant', { count: viewModel.results.participantsAnswered })}{' '}
            {t('conference.poll.types.tag_cloud.cloud_description', {
               count: _.sumBy(Object.values(viewModel.results.results.tags), (x) => x.length),
            })}
         </Typography>
      </div>
   );
}
