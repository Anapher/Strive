import { ListItem, ListItemIcon, ListItemSecondaryAction, ListItemText, makeStyles, Radio } from '@material-ui/core';
import React from 'react';
import { SceneListItemProps } from '../types';
import clsx from 'classnames';

export type Props = SceneListItemProps & {
   title: string;
   icon: React.ReactNode;

   onClick?: () => void;
   selected?: boolean;
};

const useStyles = makeStyles((theme) => ({
   root: {
      paddingRight: 8,
      paddingLeft: 0,
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
}));

export default React.forwardRef<HTMLDivElement, Props>(function SceneListItem(
   { applied, current, scene, title, onChangeScene, icon, selected, onClick },
   ref,
) {
   const classes = useStyles();

   return (
      <ListItem button className={classes.root} onClick={onClick} selected={selected} ref={ref}>
         <div className={clsx(classes.indicator, current && classes.indicatorIsCurrent)} />
         <ListItemIcon className={classes.icon}>{icon}</ListItemIcon>
         <ListItemText primary={title} />
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
