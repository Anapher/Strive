import {
   List,
   ListSubheader,
   ListItem,
   ListItemIcon,
   Checkbox,
   ListItemText,
   makeStyles,
   ListItemSecondaryAction,
   Switch,
} from '@material-ui/core';
import _ from 'lodash';
import React from 'react';
import { Permissions } from 'src/core-hub.types';
import * as allPermissions from 'src/permissions';

const PERMISSION_DELIMITER = '/';

const useStyles = makeStyles((theme) => ({
   root: {
      backgroundColor: theme.palette.background.paper,
      overflowY: 'scroll',
   },
   listSection: {
      backgroundColor: 'inherit',
   },
   ul: {
      backgroundColor: 'inherit',
      padding: 0,
   },
}));

type Props = {
   value: Permissions;
   onChange: (newValue: Permissions) => void;
   inherited: Permissions;
};

const extractPermissionName = (s: string) => s.split(PERMISSION_DELIMITER)[1];

export default function PermissionsList({ value, onChange, inherited }: Props) {
   const classes = useStyles();

   const handleSetPermission = (key: string, newValue: boolean) => onChange({ ...value, [key]: newValue });

   const handleTogglePermissions = (key: string) => {
      const isEnabled = value[key] !== undefined;
      if (!isEnabled) {
         handleSetPermission(key, true);
      } else {
         onChange(Object.fromEntries(Object.entries(value).filter((x) => x[0] !== key)));
      }
   };

   return (
      <List subheader={<li />} className={classes.root} dense>
         {_(Object.values(allPermissions))
            .orderBy((x) => x.key)
            .groupBy((x) => x.key.split(PERMISSION_DELIMITER)[0])
            .map((list, key) => (
               <li key={key} className={classes.listSection}>
                  <ul className={classes.ul}>
                     <ListSubheader>{key}</ListSubheader>
                     {list.map((permission) => {
                        const isSet = value[permission.key] !== undefined;
                        const currentValue = !!value[permission.key];
                        const inheritedValue = inherited[permission.key] as boolean | undefined;

                        return (
                           <ListItem
                              key={permission.key}
                              button
                              onClick={() => handleTogglePermissions(permission.key)}
                           >
                              <ListItemIcon>
                                 <Checkbox edge="start" checked={isSet} tabIndex={-1} disableRipple />
                              </ListItemIcon>
                              <ListItemText
                                 primary={extractPermissionName(permission.key)}
                                 secondary={inheritedValue ? `Inherited value: ${inheritedValue}` : null}
                              />
                              <ListItemSecondaryAction>
                                 <Switch
                                    edge="end"
                                    disabled={!isSet}
                                    checked={!isSet && inheritedValue ? inheritedValue : currentValue}
                                    onChange={() => handleSetPermission(permission.key, !currentValue)}
                                    color="primary"
                                 />
                              </ListItemSecondaryAction>
                           </ListItem>
                        );
                     })}
                  </ul>
               </li>
            ))
            .value()}
      </List>
   );
}
