import { makeStyles } from '@material-ui/core';
import React, { useContext } from 'react';
import { useSelector } from 'react-redux';
import { Participant } from 'src/features/conference/types';
import { SceneLayoutTypeWithAuto } from 'src/features/create-conference/types';
import LayoutChildSizeContext from '../layout-child-size-context';
import { hasAnyParticipantInCurrentRoomWebcamActicated, selectSceneLayoutType } from '../selectors';
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

export default function AutoSceneLayout({ ...props }: Props) {
   const type = useSelector(selectSceneLayoutType) || 'auto';
   return <RenderSceneLayoutByType type={type} {...props} />;
}

export function RenderSceneLayoutByType({
   type,
   children,
   center,
   ...props
}: Props & { type: SceneLayoutTypeWithAuto }) {
   const hasAnyParticipantWebcam = useSelector(hasAnyParticipantInCurrentRoomWebcamActicated);
   let layoutType = type;
   if (layoutType === 'auto') {
      layoutType = hasAnyParticipantWebcam ? 'tiles' : 'chipsWithPresenter';
   }

   console.log('chips', layoutType);

   return (
      <SceneLayout type={layoutType} {...props}>
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
