import { makeStyles, Tab, Tabs, useMediaQuery, useTheme } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import About from './About';
import AudioSettings from './AudioSettings';
import CommonSettings from './CommonSettings';
import EquipmentSettings from './EquipmentSettings';
import ScreenSettings from './ScreenSettings';
import WebcamSettings from './WebcamSettings';

interface TabPanelProps {
   children?: React.ReactNode;
   index: any;
   value: any;
   className?: string;
}

function TabPanel(props: TabPanelProps) {
   const { children, value, index, ...other } = props;

   return (
      <div
         role="tabpanel"
         hidden={value !== index}
         id={`settings-tabpanel-${index}`}
         aria-labelledby={`settings-tab-${index}`}
         {...other}
      >
         {value === index && children}
      </div>
   );
}

function a11yProps(index: any) {
   return {
      id: `settings-tab-${index}`,
      'aria-controls': `settings-tabpanel-${index}`,
   };
}

const useStyles = makeStyles((theme) => ({
   root: {
      flexGrow: 1,
      backgroundColor: theme.palette.background.paper,
      display: 'flex',

      [theme.breakpoints.up('md')]: {
         flexDirection: 'row',
         height: 400,
      },
      [theme.breakpoints.down('sm')]: {
         flexDirection: 'column',
      },
   },
   tabs: {
      [theme.breakpoints.up('md')]: {
         borderRight: `1px solid ${theme.palette.divider}`,
      },
      [theme.breakpoints.down('sm')]: {
         borderBottom: `1px solid ${theme.palette.divider}`,
      },
   },
   tab: {
      [theme.breakpoints.down('sm')]: {
         marginTop: theme.spacing(2),
      },
   },
   tabPanels: {
      flex: 1,
      minHeight: 0,
      overflowY: 'auto',
   },
}));

export default function Settings() {
   const classes = useStyles();
   const [value, setValue] = React.useState(0);
   const { t } = useTranslation();

   const handleChange = (_: React.ChangeEvent<unknown>, newValue: number) => {
      setValue(newValue);
   };

   const theme = useTheme();
   const horizontalTabs = useMediaQuery(theme.breakpoints.down('sm'));

   return (
      <div className={classes.root}>
         <Tabs
            orientation={horizontalTabs ? 'horizontal' : 'vertical'}
            variant="scrollable"
            aria-label="settings tabs"
            className={classes.tabs}
            value={value}
            onChange={handleChange}
         >
            <Tab label={t('common:common')} {...a11yProps(0)} />
            <Tab label={t('conference.settings.audio.title')} {...a11yProps(1)} />
            <Tab label={t('conference.settings.webcam.title')} {...a11yProps(2)} />
            <Tab label={t('conference.settings.screen.title')} {...a11yProps(3)} />
            <Tab label={t('conference.settings.equipment.title')} {...a11yProps(4)} />
            <Tab label={t('conference.settings.about.title')} {...a11yProps(6)} />
         </Tabs>
         <div className={classes.tabPanels}>
            <TabPanel value={value} index={0} className={classes.tab}>
               <CommonSettings />
            </TabPanel>
            <TabPanel value={value} index={1} className={classes.tab}>
               <AudioSettings />
            </TabPanel>
            <TabPanel value={value} index={2} className={classes.tab}>
               <WebcamSettings />
            </TabPanel>
            <TabPanel value={value} index={3} className={classes.tab}>
               <ScreenSettings />
            </TabPanel>
            <TabPanel value={value} index={4} className={classes.tab}>
               <EquipmentSettings />
            </TabPanel>
            <TabPanel value={value} index={5} className={classes.tab}>
               <About />
            </TabPanel>
         </div>
      </div>
   );
}
