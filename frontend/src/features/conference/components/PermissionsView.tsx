import { Chip, Table, TableBody, TableCell, TableRow, Tooltip, Typography } from '@material-ui/core';
import _ from 'lodash';
import React, { Fragment, useMemo } from 'react';
import { ParticipantPermissionInfo, PermissionLayer, PermissionValue } from 'src/core-hub.types';

type Props = {
   permissions: ParticipantPermissionInfo;
};

export default function PermissionsView({ permissions: { layers } }: Props) {
   const viewModels = useMemo(() => mapToViewModels(layers), [layers]);

   return (
      <div>
         <Typography gutterBottom variant="body2">
            Here you can inspect the exact permissions and where they come from. A permission consists of a key and a
            value that maybe used all over the application. The permissions are grouped in layers, depending of the
            status of the participant there may be different layers. Layers at the top overwrite permissions defined at
            the bottom if with the same key. The default values are false.
         </Typography>
         <Table size="small">
            <TableBody>
               {viewModels.map((layer) => (
                  <Fragment key={layer.name}>
                     <TableRow>
                        <TableCell colSpan={2}>
                           <Typography variant="h6" style={{ marginTop: 8 }}>
                              {layer.name}
                           </Typography>
                        </TableCell>
                     </TableRow>
                     {layer.values.map((permission) =>
                        permission.overwrittenIn ? (
                           <Tooltip
                              key={permission.key}
                              title={`This property is overwritten in ${permission.overwrittenIn}`}
                           >
                              <TableRow style={{ opacity: 0.5 }}>
                                 <TableCell>
                                    <s style={{ marginLeft: 16 }}>{permission.key}</s>
                                 </TableCell>
                                 <TableCell>
                                    <Chip size="small" label={<s>{permission.value.toString()}</s>} />
                                 </TableCell>
                              </TableRow>
                           </Tooltip>
                        ) : (
                           <TableRow key={permission.key}>
                              <TableCell>
                                 <span style={{ marginLeft: 16 }}>{permission.key}</span>
                              </TableCell>
                              <TableCell>
                                 <Chip size="small" label={permission.value.toString()} />
                              </TableCell>
                           </TableRow>
                        ),
                     )}
                  </Fragment>
               ))}
            </TableBody>
         </Table>
      </div>
   );
}

type PermissionLayerViewModel = {
   name: string;
   values: PermissionValueViewModel[];
};

type PermissionValueViewModel = {
   key: string;
   value: PermissionValue;
   overwrittenIn?: string;
};

function mapToViewModels(layers: PermissionLayer[]): PermissionLayerViewModel[] {
   const appliedValues = new Map<string, string>();
   const result = new Array<PermissionLayerViewModel>();

   for (const layer of _(layers)
      .orderBy((x) => x.order, 'desc')
      .value()) {
      const values = new Array<PermissionValueViewModel>();
      for (const [key, value] of Object.entries(layer.permissions)) {
         const overwrittenIn = appliedValues.get(key);
         if (!overwrittenIn) {
            appliedValues.set(key, layer.name);
         }

         values.push({ key, value, overwrittenIn });
      }

      if (values.length > 0) result.push({ name: layer.name, values: _.orderBy(values, (x) => x.key) });
   }

   return result;
}
