import { Accordion, AccordionDetails, AccordionSummary, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';

const useStyles = makeStyles((theme) => ({
   heading: {
      fontSize: theme.typography.pxToRem(15),
      flex: 1,
   },
   accordionSummary: {
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'center',
      flex: 1,
   },
}));

type Props = {
   expanded: boolean;
   onChange: (isExpanded: boolean) => void;

   title: string;

   renderStatus?: () => React.ReactChild;
   children?: React.ReactNode;
};

export default function TroubleshootAccordion({ expanded, onChange, title, renderStatus, children }: Props) {
   const classes = useStyles();

   const handleChange = (_: React.ChangeEvent<unknown>, isExpanded: boolean) => {
      onChange(isExpanded);
   };

   return (
      <Accordion expanded={expanded} onChange={handleChange}>
         <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls={`troubleshoot-${title.toLowerCase()}-content`}
            id={`troubleshoot-${title.toLowerCase()}-header`}
         >
            <div className={classes.accordionSummary}>
               <Typography className={classes.heading}>{title}</Typography>
               {renderStatus && renderStatus()}
            </div>
         </AccordionSummary>
         <AccordionDetails>{children}</AccordionDetails>
      </Accordion>
   );
}
