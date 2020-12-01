import {
   Button,
   ClickAwayListener,
   Grow,
   List,
   ListItem,
   ListItemIcon,
   ListItemSecondaryAction,
   ListItemText,
   makeStyles,
   MenuList,
   Paper,
   Popper,
   Radio,
   SvgIconProps,
   Typography,
   useTheme,
} from '@material-ui/core';
import AppsIcon from '@material-ui/icons/Apps';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import StarIcon from '@material-ui/icons/Star';
import React, { useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { ParticipantDto } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { setAppliedScene } from '../scenesSlice';
import { selectAvailableScenesViewModels, selectServerProvidedScene } from '../selectors';
import { Scene } from '../types';
import { BreakoutRoomAction, BreakoutRoomActive } from 'src/features/breakout-rooms/components/BreakoutRoomActions';
import OpenBreakoutRoomsDialog from 'src/features/breakout-rooms/components/OpenBreakoutRoomsDialog';

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(1, 2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

const getSceneTitle = (scene: Scene, participants: ParticipantDto[] | null) => {
   switch (scene.type) {
      case 'automatic':
         return 'Automatic';
      case 'grid':
         return 'Grid';
      case 'screenshare':
         return `Screen of ${participants?.find((x) => x.participantId === scene.participantId)?.displayName}`;
   }
};

const getSceneIcon = (scene: Scene, color?: string) => {
   const props: SvgIconProps = { style: { color } };

   switch (scene.type) {
      case 'grid':
         return <AppsIcon {...props} />;
      case 'screenshare':
         return <DesktopWindowsIcon {...props} />;
      case 'automatic':
         return <StarIcon {...props} />;
   }
};

export default function SceneManagement() {
   const classes = useStyles();
   const [actionPopper, setActionPopper] = useState(false);
   const actionButton = useRef<HTMLButtonElement>(null);
   const handleClose = () => setActionPopper(false);
   const handleOpen = () => setActionPopper(true);

   const participants = useSelector((state: RootState) => state.conference.participants);
   const availableScenes = useSelector(selectAvailableScenesViewModels);
   const theme = useTheme();
   const serverScene = useSelector(selectServerProvidedScene);
   const dispatch = useDispatch();

   const handleChangeScene = (checked: boolean, scene: Scene) => {
      if (checked) {
         dispatch(setAppliedScene(scene));
      }
   };

   return (
      <div>
         <List dense disablePadding>
            <li style={{ paddingLeft: 16, marginTop: 16 }}>
               <Typography variant="subtitle2" color="textSecondary">
                  Scenes
               </Typography>
            </li>
            {availableScenes.map(({ id, scene, isApplied, isCurrent }) => (
               <ListItem button style={{ paddingRight: 8, paddingLeft: 0 }} key={id}>
                  <div
                     style={{
                        backgroundColor: isCurrent ? theme.palette.primary.main : undefined,
                        width: 4,
                        alignSelf: 'stretch',
                        marginRight: 12,
                     }}
                  />
                  <ListItemIcon style={{ minWidth: 32 }}>
                     {getSceneIcon(scene, isCurrent ? theme.palette.primary.light : undefined)}
                  </ListItemIcon>
                  <ListItemText primary={getSceneTitle(scene, participants)} />
                  <ListItemSecondaryAction>
                     <Radio
                        edge="end"
                        checked={isApplied}
                        onChange={(_, checked) => handleChangeScene(checked, scene)}
                     />
                  </ListItemSecondaryAction>
               </ListItem>
            ))}

            <li style={{ paddingLeft: 16, marginTop: 16 }}>
               <Typography variant="subtitle2" color="textSecondary">
                  Active
               </Typography>
            </li>
            <BreakoutRoomActive />
         </List>
         <Paper elevation={4} className={classes.root}>
            <Button variant="contained" color="primary" size="small" fullWidth ref={actionButton} onClick={handleOpen}>
               Actions <ArrowDropDownIcon />
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
                        <MenuList id="action list">
                           <BreakoutRoomAction onClose={handleClose} />
                        </MenuList>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
         <OpenBreakoutRoomsDialog />
      </div>
   );
}
