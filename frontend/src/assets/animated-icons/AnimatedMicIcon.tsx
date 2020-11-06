import React from 'react';
import { motion } from 'framer-motion';
import { useTheme } from '@material-ui/core';

const Path = (props: React.ComponentProps<typeof motion.path>) => (
   <motion.path strokeWidth="2" {...props} fill="transparent" />
);

type Props = {
   activated?: boolean;
   color?: string;
   disabledColor?: string;
} & React.ComponentProps<typeof motion.svg>;

export default function AnimatedMicIcon({ activated, color, disabledColor, ...motionSvg }: Props) {
   const animate = activated ? 'activated' : 'deactivated';
   const theme = useTheme();

   color = color ?? theme.palette.text.primary;
   disabledColor = disabledColor ?? theme.palette.error.dark;

   return (
      <motion.svg
         viewBox="0 0 24 24"
         fill={color}
         width="18px"
         height="18px"
         initial={animate}
         animate={animate}
         {...motionSvg}
      >
         <defs>
            <mask id="myMask">
               <rect x="0" y="0" width="24" height="24" fill="white" />
               <Path
                  variants={{
                     activated: { d: 'M 0,0 0,0' },
                     deactivated: { d: 'M 5.3753728,1.4209187 22.416419,18.863895' },
                  }}
                  stroke="black"
               />
            </mask>
         </defs>
         <path d="M0 0h24v24H0z" fill="none" />
         <motion.path
            d="M12 14c1.66 0 2.99-1.34 2.99-3L15 5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3zm5.3-3c0 3-2.54 5.1-5.3 5.1S6.7 14 6.7 11H5c0 3.41 2.72 6.23 6 6.72V21h2v-3.28c3.28-.48 6-3.3 6-6.72h-1.7z"
            mask="url(#myMask)"
            variants={{ activated: { fill: color }, deactivated: { fill: disabledColor } }}
         />
         <Path
            variants={{
               activated: { d: 'M 0,0 0,0', stroke: disabledColor },
               deactivated: { d: 'M 3.9573736,2.7985301 20.99842,20.241506', stroke: disabledColor },
            }}
         />
      </motion.svg>
   );
}
