import { Box, makeStyles, Paper, Tab, Tabs, TextField } from '@material-ui/core';
import { TabContext, TabPanel } from '@material-ui/lab';
import React, { useState } from 'react';
import { UseFormMethods } from 'react-hook-form';
import { ConferenceData } from '../types';
import TabCommon from './TabCommon';

const checkBoxWidth = 150;

const useStyles = makeStyles((theme) => ({
   header: {
      backgroundColor: theme.palette.grey[800],
   },
   fab: {
      position: 'absolute',
      bottom: theme.spacing(2),
      right: theme.spacing(2),
   },
}));

type Props = {
   form: UseFormMethods<ConferenceData>;
};

export default function CreateConferenceForm({ form }: Props) {
   const [currentTab, setCurrentTab] = useState('1');

   const handleChange = (_: unknown, newValue: string) => {
      setCurrentTab(newValue);
   };

   const { register } = form;
   const classes = useStyles();

   return (
      <Box display="flex" flexDirection="column">
         <Box mb={2} px={3}>
            <TextField fullWidth label="Name" name="name" inputRef={register} placeholder="Unnamed conference" />
         </Box>
         <TabContext value={currentTab}>
            <Paper square className={classes.header}>
               <Tabs
                  value={currentTab}
                  indicatorColor="primary"
                  textColor="primary"
                  onChange={handleChange}
                  aria-label="options tabs"
               >
                  <Tab label="Common" value="1" />
                  <Tab label="Moderators" value="2" />
                  <Tab label="Permissions" value="3" />
               </Tabs>
            </Paper>
            <Box height={300} position="relative">
               <TabPanel value="1">
                  <TabCommon form={form} />
               </TabPanel>
            </Box>
         </TabContext>
      </Box>
   );
}
