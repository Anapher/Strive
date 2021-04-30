import { ListItem, ListItemIcon, ListItemSecondaryAction, ListItemText, makeStyles, Radio } from '@material-ui/core';
import _ from 'lodash';
import React from 'react';
import { AvailableSceneListItemProps } from '../types';

export type Props = AvailableSceneListItemProps & {
   title: string;
   icon: React.ReactNode;

   onClick?: () => void;
   selected?: boolean;
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
   { scene, stack, title, onChangeScene, icon, selected, onClick },
   ref,
) {
   const classes = useStyles();
   const applied = _.isEqual(stack[stack.length - 1], scene);

   return (
      <ListItem button className={classes.root} onClick={onClick} selected={selected} ref={ref}>
         <ListItemIcon className={classes.icon}>{icon}</ListItemIcon>
         <ListItemText primary={title} className={classes.listItemText} primaryTypographyProps={{ noWrap: true }} />
         <ListItemSecondaryAction>
            <Radio
               edge="end"
               checked={applied}
               onChange={(_, checked) => {
                  if (checked) onChangeScene(scene);
               }}
            />
         </ListItemSecondaryAction>
      </ListItem>
   );
});
