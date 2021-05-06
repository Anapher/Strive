import { ListSubheader, MenuItem, Select } from '@material-ui/core';
import React from 'react';
import { isMobile } from 'react-device-detect';

type MenuItemData = { label: string; value: string; key?: string };
type MenuItemDataGroup = { label?: string; key?: string; children?: MenuItemData[] };

function isMenuItem(obj: MenuItemData | MenuItemDataGroup): obj is MenuItemData {
   return 'value' in obj;
}

const mapMobileMenuItems = ({ key, value, label }: MenuItemData) => (
   <option key={key ?? value} value={value}>
      {label}
   </option>
);

const mapDesktopMenuItems = ({ key, value, label }: MenuItemData) => (
   <MenuItem value={value} key={key ?? value}>
      {label}
   </MenuItem>
);

const mapMobileGroup = ({ label, key, children }: MenuItemDataGroup) =>
   label ? (
      <optgroup key={key ?? label} label={label}>
         {children?.map(mapMobileMenuItems)}
      </optgroup>
   ) : (
      children?.map(mapMobileMenuItems)
   );

const mapDesktopGroup = ({ label, key, children }: MenuItemDataGroup) =>
   label
      ? ([<ListSubheader key={key ?? label}>{label}</ListSubheader>, children?.map(mapDesktopMenuItems)] as any)
      : children?.map(mapDesktopMenuItems);

type Props = Omit<React.ComponentProps<typeof Select>, 'children'> & {
   children?: (MenuItemData | MenuItemDataGroup)[];
};

export default function MobileAwareSelect({ children, ...props }: Props) {
   const itemMap = isMobile ? mapMobileMenuItems : mapDesktopMenuItems;
   const groupMap = isMobile ? mapMobileGroup : mapDesktopGroup;

   return (
      <Select native={isMobile} {...props}>
         {children?.map((x) => (isMenuItem(x) ? itemMap(x) : groupMap(x)))}
      </Select>
   );
}
