import {
   Button,
   ClickAwayListener,
   Divider,
   Grow,
   List,
   ListItem,
   ListItemIcon,
   ListItemText,
   ListSubheader,
   makeStyles,
   MenuList,
   Paper,
   Popper,
   Typography,
} from '@material-ui/core';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import CloseIcon from '@material-ui/icons/Close';
import _ from 'lodash';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_OVERWRITE_CONTENT_SCENE, SCENES_CAN_SET_SCENE } from 'src/permissions';
import { RootState } from 'src/store';
import { default as presenters, default as scenePresenters } from '../scene-presenter-registry';
import { Scene } from '../types';
import SceneManagementModeSelectionDialog from './SceneManagementModeSelectionDialog';

const sceneDisplayOrder: Scene['type'][] = ['autonomous', 'grid', 'activeSpeaker', 'screenShare', 'breakoutRoom'];

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(1, 2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      borderTopLeftRadius: 0,
      borderTopRightRadius: 0,
   },
   modeButton: {
      textAlign: 'left',
      textOverflow: 'ellipsis',
      width: '100%',
   },
   modeButtonContainer: {
      padding: theme.spacing(0, 1, 1, 1),
      width: '100%',
   },
   divider: {
      marginLeft: theme.spacing(2),
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

   const [modeSelectionOpen, setModeSelectionOpen] = useState(false);
   const handleOpenModeSelection = () => setModeSelectionOpen(true);
   const handleCloseModeSelection = () => setModeSelectionOpen(false);

   const canSetScene = usePermission(SCENES_CAN_SET_SCENE);
   const canOverwriteScene = usePermission(SCENES_CAN_OVERWRITE_CONTENT_SCENE);

   // getIsActionListItemVisible() may contain hooks
   const visibleActionItemPresenter = presenters.filter(
      (x) => x.ActionListItem && (!x.getIsActionListItemVisible || x.getIsActionListItemVisible()),
   );

   const synchronized = useSelector((state: RootState) => state.scenes.synchronized);
   if (synchronized === null) return null;

   const { availableScenes, sceneStack, selectedScene, overwrittenContent } = synchronized;

   const handleChangeScene = (scene: Scene) => {
      if (canSetScene) {
         dispatch(coreHub.setScene(scene));
         setModeSelectionOpen(false);
      }
   };

   const handleOverwriteScene = (scene: Scene | null) => {
      if (canOverwriteScene) {
         dispatch(coreHub.setOverwrittenScene(scene));
      }
   };

   const availableScenePresenters = _.orderBy(
      availableScenes.map((scene) => {
         // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
         const presenter = scenePresenters.find((x) => x.type === scene.type)!;
         if (!presenter) console.error('Presenter not found', scene);

         return {
            scene,
            presenter,
         };
      }),
      (x) => sceneDisplayOrder.indexOf(x.scene.type),
   );

   return (
      <div>
         {(canSetScene || canOverwriteScene) && (
            <>
               <Divider className={classes.divider} />
               <List dense disablePadding id="scene-management-selection-list">
                  <ListSubheader>{t('glossary:scene_plural')}</ListSubheader>
                  {canSetScene && (
                     <div className={classes.modeButtonContainer}>
                        <Button
                           id="scene-management-mode"
                           size="small"
                           className={classes.modeButton}
                           onClick={handleOpenModeSelection}
                           disabled={!canSetScene}
                           variant="outlined"
                        >
                           <Typography variant="body2" noWrap>
                              <ActiveSceneDescriptor scene={selectedScene} />
                           </Typography>
                        </Button>
                     </div>
                  )}
                  {availableScenePresenters.map(
                     ({ presenter, scene }) =>
                        presenter.AvailableSceneListItem && (
                           <presenter.AvailableSceneListItem
                              key={presenter.getSceneId ? presenter.getSceneId(scene) : scene.type}
                              scene={scene}
                              stack={sceneStack}
                              onChangeScene={handleOverwriteScene}
                           />
                        ),
                  )}
                  {overwrittenContent && (
                     <ListItem
                        button
                        onClick={() => handleOverwriteScene(null)}
                        title={t('conference.scenes.remove_overwrite')}
                     >
                        <ListItemIcon style={{ minWidth: 32 }}>
                           <CloseIcon />
                        </ListItemIcon>
                        <ListItemText
                           primaryTypographyProps={{ noWrap: true }}
                           primary={t('conference.scenes.remove_overwrite')}
                        />
                     </ListItem>
                  )}
               </List>
            </>
         )}
         {visibleActionItemPresenter.length > 0 && (
            <Paper elevation={4} className={classes.root}>
               <Button
                  id="scene-management-actions"
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
         )}
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
                           {visibleActionItemPresenter.map(({ type, ActionListItem }) => {
                              if (!ActionListItem) return null;
                              return <ActionListItem key={type} onClose={handleClose} />;
                           })}
                        </MenuList>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
         {scenePresenters.map(({ AlwaysRender, type }) => AlwaysRender && <AlwaysRender key={type} />)}
         <SceneManagementModeSelectionDialog
            open={modeSelectionOpen}
            onClose={handleCloseModeSelection}
            availableScenes={availableScenes}
            selectedScene={selectedScene}
            onChangeScene={handleChangeScene}
         />
      </div>
   );
}

function ActiveSceneDescriptor({ scene }: { scene?: Scene }) {
   const presenter = presenters.find((x) => x.type === scene?.type);
   if (presenter?.ActiveDescriptor) return <presenter.ActiveDescriptor scene={scene} />;

   return <span>{scene?.type}</span>;
}
