import { useTheme } from '@material-ui/core';
import { motion } from 'framer-motion';
import React, { useEffect, useRef } from 'react';

const Path = (props: React.ComponentProps<typeof motion.path>) => (
   <motion.path strokeWidth="2" {...props} fill="transparent" />
);

let maskCounter = 0;

export type AnimatedIconProps = {
   activated?: boolean;
   color?: string;
   disabledColor?: string;
} & React.ComponentProps<typeof motion.svg>;

export default function AnimatedIconBase({
   activated,
   color,
   disabledColor,
   path,
   ...motionSvg
}: AnimatedIconProps & { path: string }) {
   const animate = activated ? 'activated' : 'deactivated';
   const theme = useTheme();
   const maskId = useRef<string>();

   useEffect(() => {
      maskId.current = `animated-icon-mask-${maskCounter++}`;
   }, []);

   color = color ?? theme.palette.text.primary;
   disabledColor = disabledColor ?? color;

   return (
      <motion.svg
         viewBox="0 0 24 24"
         fill={activated ? color : disabledColor}
         width="18px"
         height="18px"
         initial={animate}
         animate={animate}
         {...motionSvg}
      >
         <defs>
            <mask id={maskId.current}>
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
         <motion.path d={path} mask={`url(#${maskId.current})`} />
         <Path
            stroke={disabledColor}
            variants={{
               activated: { d: 'M 0,0 0,0' },
               deactivated: { d: 'M 3.9573736,2.7985301 20.99842,20.241506' },
            }}
         />
      </motion.svg>
   );
}
