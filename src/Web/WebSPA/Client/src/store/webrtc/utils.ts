import _ from 'lodash';
import { RtpEncodingParameters } from 'mediasoup-client/lib/RtpParameters';

export function fetchEncodings(sourceHeight: number, layerResolutions: number[], parameters: RtpEncodingParameters[]) {
   const maxLayer = layerResolutions.indexOf(_.orderBy(layerResolutions, (x) => Math.abs(x - sourceHeight))[0]);

   const layerEncodings = parameters.slice(0, maxLayer + 1);
   const scaledLayers = layerEncodings.map((encoding, i) => ({
      ...encoding,
      scaleResolutionDownBy: Math.max((layerEncodings.length - 1 - i) * 2, 1),
   }));

   return scaledLayers;
}
