import {
   Button,
   ClickAwayListener,
   Grow,
   List,
   makeStyles,
   MenuList,
   Paper,
   Popper,
   Typography,
} from '@material-ui/core';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import React, { useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_SET_SCENE } from 'src/permissions';
import { setAppliedScene } from '../reducer';
import scenePresenters from '../scene-presenter-registry';
import { selectAvailableScenesViewModels, selectServerScene } from '../selectors';
import { Scene } from '../types';
import * as coreHub from 'src/core-hub';
import { selectParticipantRoom } from 'src/features/rooms/selectors';
import _ from 'lodash';
import { useTranslation } from 'react-i18next';

const sceneDisplayOrder: Scene['type'][] = ['autonomous', 'grid', 'activeSpeaker', 'screenShare', 'breakoutRoom'];

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(1, 2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

export default function SceneManagement() {
   const dispatch = useDispatch();
   const classes = useStyles();
   const { t } = useTranslation();

   const [actionPopper, setActionPopper] = useState(false);
   const actionButton = useRef<HTMLButtonElement>(null);
   const handleClose = () => setActionPopper(false);
   const handleOpen = () => setActionPopper(true);

   const availableScenes = useSelector(selectAvailableScenesViewModels);

   const serverScene = useSelector(selectServerScene);
   const canSetScene = usePermission(SCENES_CAN_SET_SCENE);
   const myRoomId = useSelector(selectParticipantRoom);

   const canChangeScene = canSetScene || !serverScene?.isControlled;

   const handleChangeScene = (scene: Scene) => {
      if (canSetScene) {
         dispatch(
            coreHub.setScene({
               roomId: myRoomId as string,
               active: { scene, config: serverScene?.config ?? {}, isControlled: true },
            }),
         );
      } else if (canChangeScene) {
         dispatch(setAppliedScene(scene));
      }
   };

   const availableScenePresenters = _.orderBy(
      availableScenes.map((viewModel) => {
         // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
         const presenter = scenePresenters.find((x) => x.type === viewModel.scene.type)!;
         if (!presenter) console.error('Presenter not found', viewModel.scene);

         return {
            viewModel,
            presenter,
         };
      }),
      (x) => sceneDisplayOrder.indexOf(x.viewModel.scene.type),
   );

   if (!canChangeScene) return null;

   return (
      <div>
         <List dense disablePadding>
            <li style={{ paddingLeft: 16, marginTop: 16 }}>
               <Typography variant="subtitle2" color="textSecondary">
                  {t('glossary:scene_plural')}
               </Typography>
            </li>
            {availableScenePresenters.map(({ presenter, viewModel }) => (
               <presenter.ListItem
                  key={viewModel.id}
                  applied={viewModel.isApplied}
                  current={viewModel.isCurrent}
                  scene={viewModel.scene}
                  onChangeScene={handleChangeScene}
               />
            ))}
         </List>
         <Paper elevation={4} className={classes.root}>
            <Button
               variant="contained"
               color="primary"
               size="small"
               fullWidth
               ref={actionButton}
               onClick={handleOpen}
               aria-controls={actionPopper ? 'scene-action-list' : undefined}
               aria-expanded={actionPopper ? 'true' : undefined}
               aria-label={t('conference.scenes.actions_description')}
               aria-haspopup="menu"
            >
               {t('conference.scenes.actions')} <ArrowDropDownIcon />
            </Button>
         </Paper>
         <Popper open={actionPopper} anchorEl={actionButton.current} role={undefined} transition>
            {({ TransitionProps }) => (
               <Grow
                  {...TransitionProps}
                  style={{
                     transformOrigin: 'center bottom',
                  }}
               >
                  <Paper>
                     <ClickAwayListener onClickAway={handleClose}>
                        <MenuList id="scene-action-list">
                           {scenePresenters.map(({ type, OpenMenuItem }) => {
                              if (!OpenMenuItem) return null;
                              return <OpenMenuItem key={type} onClose={handleClose} />;
                           })}
                        </MenuList>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
         {scenePresenters.map(({ AlwaysRender, type }) => AlwaysRender && <AlwaysRender key={type} />)}
      </div>
   );
}
