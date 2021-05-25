import { ListItem, ListItemIcon, ListItemText, makeStyles } from '@material-ui/core';
import _ from 'lodash';
import React from 'react';
import { AvailableSceneListItemProps } from '../types';

export type Props = AvailableSceneListItemProps & {
   title: string;
   icon: React.ReactNode;

   children?: React.ReactNode;
};

const useStyles = makeStyles((theme) => ({
   root: {
      paddingRight: 8,
   },
   indicator: {
      width: 4,
      alignSelf: 'stretch',
      marginRight: 12,
   },
   indicatorIsCurrent: {
      backgroundColor: theme.palette.primary.main,
   },
   icon: {
      minWidth: 32,
   },
   listItemText: {
      marginRight: 36,
   },
}));

export default React.forwardRef<HTMLDivElement, Props>(function SceneListItem(
   { scene, stack, title, onChangeScene, icon, children },
   ref,
) {
   const classes = useStyles();
   const applied = _.isEqual(stack[stack.length - 1], scene);

   const handleToggle = () => {
      if (!applied) {
         onChangeScene(scene);
      }
   };

   return (
      <ListItem button className={classes.root} onClick={handleToggle} selected={applied} ref={ref}>
         <ListItemIcon className={classes.icon}>{icon}</ListItemIcon>
         <ListItemText primary={title} className={classes.listItemText} primaryTypographyProps={{ noWrap: true }} />
         {children}
      </ListItem>
   );
});
