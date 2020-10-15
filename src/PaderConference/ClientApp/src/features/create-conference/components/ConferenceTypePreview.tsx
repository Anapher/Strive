import { Box, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React from 'react';
import { ConferenceType } from '../types';

type Props = {
   type: ConferenceType;
   width: number;
   animate?: boolean;
};

const factor16to9 = 0.5625;

export default function ConferenceTypePreview({ animate, width, type }: Props) {
   const skeletonProps: React.ComponentProps<typeof Skeleton> = {
      animation: animate ? 'wave' : false,
      variant: 'rect',
   };

   return (
      <div>
         <Typography variant="caption" style={{ textTransform: 'capitalize' }} gutterBottom>
            {type}
         </Typography>
         <Box width={width} height={width * factor16to9} display="flex" flexDirection="column">
            <Skeleton {...skeletonProps} height={12} style={{ marginBottom: 4 }} />
            <MainView type={type} skeleton={skeletonProps} />
         </Box>
      </div>
   );
}

type MainViewProps = {
   type: ConferenceType;
   skeleton: React.ComponentProps<typeof Skeleton>;
};
function MainView({ type, skeleton }: MainViewProps) {
   switch (type) {
      case 'class':
         return (
            <Box display="flex" flexDirection="row" flex={1}>
               <Skeleton {...skeleton} style={{ flex: 1 }} height="100%" />
               <Box style={{ marginLeft: 4, marginRight: 4 }} display="flex" flexDirection="column" flex={3}>
                  <Skeleton {...skeleton} style={{ flex: 1 }} />
                  <Box style={{ marginTop: 2 }} display="flex" flexDirection="row" justifyContent="center">
                     <Skeleton {...skeleton} variant="circle" height={10} width={10} />
                     <Skeleton {...skeleton} variant="circle" height={10} width={10} style={{ marginLeft: 2 }} />
                  </Box>
               </Box>
               <Skeleton {...skeleton} style={{ flex: 1 }} height="100%" />
            </Box>
         );
      case 'presentation':
         return (
            <Box display="flex" flexDirection="row" flex={1}>
               <Skeleton {...skeleton} style={{ flex: 4 }} height="100%" />
               <Skeleton {...skeleton} style={{ flex: 1, marginLeft: 4 }} height="100%" />
            </Box>
         );
      default:
         return <>Preview not available</>;
   }
}
