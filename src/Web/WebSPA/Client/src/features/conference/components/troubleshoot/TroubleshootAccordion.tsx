import { Accordion, AccordionDetails, AccordionSummary, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
      marginRight: theme.spacing(1),
   },
   accordionSummary: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      flex: 1,
      minWidth: 0,
   },
   content: {
      minWidth: 0,
   },
}));

type Props = {
   className?: string;

   expanded: boolean;
   onChange: (isExpanded: boolean) => void;

   title: string;

   renderStatus?: () => React.ReactChild;
   children?: React.ReactNode;
};

export default function TroubleshootAccordion({ expanded, onChange, title, renderStatus, children, className }: Props) {
   const classes = useStyles();

   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };

   return (
      <Accordion expanded={expanded} onChange={handleChange} className={className}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls={`troubleshoot-${title.toLowerCase()}-content`}
            id={`troubleshoot-${title.toLowerCase()}-header`}
            style={{ width: '100%', overflow: 'hidden', minWidth: 0 }}
            classes={{ content: classes.content }}
         >
            <div className={classes.accordionSummary}>
               <Typography className={classes.heading}>{title}</Typography>
               <div style={{ minWidth: 0, overflow: 'hidden' }}>{renderStatus && renderStatus()}</div>
            </div>
         </AccordionSummary>
         <AccordionDetails>{children}</AccordionDetails>
      </Accordion>
   );
}
