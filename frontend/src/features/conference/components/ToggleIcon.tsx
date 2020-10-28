import React from 'react';
import { AnimatePresence, motion } from 'framer-motion';
import { OverridableComponent } from '@material-ui/core/OverridableComponent';
import { SvgIconTypeMap } from '@material-ui/core';

const variants = {
   enter: {
      scale: 0,
      opacity: 0,
   },
   center: {
      scale: 1,
      opacity: 1,
   },
   exit: {
      scale: 0,
      opacity: 0,
   },
};

type Props = {
   IconEnable: OverridableComponent<SvgIconTypeMap>;
   IconDisable: OverridableComponent<SvgIconTypeMap>;
   enabled: boolean | undefined;
};

const iconStyle = {
   width: 14,
   height: 14,
   originX: 0.5,
   originY: 0.5,
};

export default function ToggleIcon({ IconEnable, IconDisable, enabled }: Props) {
   return (
      <AnimatePresence>
         {enabled === true && (
            <IconDisable
               component={motion.svg}
               variants={variants}
               initial="enter"
               animate="center"
               exit="exit"
               style={iconStyle}
            />
         )}
         {enabled === false && (
            <IconEnable
               component={motion.svg}
               variants={variants}
               initial="enter"
               animate="center"
               exit="exit"
               style={iconStyle}
            />
         )}
      </AnimatePresence>
   );
}
