import MoreVertIcon from '@material-ui/icons/MoreVert';
import { Draw } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectWhiteboard, selectWhiteboardsCount } from 'src/features/whiteboard/selectors';
import { RootState } from 'src/store';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import PollOptionsPopper from './WhiteboardOptionsPopper';

export default function WhiteboardListItem(props: AvailableSceneListItemProps) {
   const scene = props.scene;

   if (scene.type !== 'whiteboard') throw new Error('Invalid scene type');

   const whiteboard = useSelector((state: RootState) => selectWhiteboard(state, scene.id));
   const whiteboardCount = useSelector(selectWhiteboardsCount);
   const { t } = useTranslation();

   return (
      <SceneListItemWithPopper
         PopperComponent={PollOptionsPopper}
         {...props}
         icon={<Draw />}
         listItemIcon={<MoreVertIcon />}
         title={whiteboardCount === 1 || !whiteboard ? t('conference.whiteboard.whiteboard') : whiteboard.friendlyName}
      />
   );
}
