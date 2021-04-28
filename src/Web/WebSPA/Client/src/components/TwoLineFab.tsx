import { Fab, Typography } from '@material-ui/core';
import React from 'react';

type TwoLineFabProps = React.ComponentProps<typeof Fab> & {
   subtitle?: any;
};

export default function TwoLineFab({ children, subtitle, variant = 'extended', ...props }: TwoLineFabProps) {
   return (
      <Fab variant={variant} {...props}>
         <div style={{ display: 'flex', flexDirection: 'column' }}>
            <Typography component="span">{children}</Typography>
            <Typography variant="caption" style={{ marginTop: -4, fontSize: 10 }}>
               {subtitle}
            </Typography>
         </div>
      </Fab>
   );
}
