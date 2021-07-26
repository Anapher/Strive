import { makeStyles } from '@material-ui/core';
import React, { useContext } from 'react';
import { Participant } from 'src/features/conference/types';
import LayoutChildSizeContext from '../layout-child-size-context';
import SceneLayout from './SceneLayout';

const useStyles = makeStyles({
   centered: {
      width: '100%',
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
   },
});

type Props = {
   children?: React.ReactNode;
   width: number;
   height: number;
   participant?: Participant;
   className?: string;
   center?: boolean;
};

export default function AutoSceneLayout({ children, center, ...props }: Props) {
   return (
      <SceneLayout type="tiles-bar" {...props}>
         {center ? <CenteredLayout>{children}</CenteredLayout> : children}
      </SceneLayout>
   );
}

type CenteredLayoutProps = {
   children?: React.ReactNode;
};
function CenteredLayout({ children }: CenteredLayoutProps) {
   const classes = useStyles();
   const size = useContext(LayoutChildSizeContext);

   const offsetTop = size.topOffset ?? 0;

   return (
      <div className={classes.centered} style={{ marginTop: -offsetTop, height: `calc(100% + ${offsetTop}px)` }}>
         <div style={{ flex: 1, minHeight: offsetTop }} />
         {children}
         <div style={{ flex: 1 }} />
      </div>
   );
}
