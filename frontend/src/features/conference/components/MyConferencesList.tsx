import { List, makeStyles, Typography } from '@material-ui/core';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(2),
   },
}));

export default function MyConferencesList() {
   return <List></List>;
}
