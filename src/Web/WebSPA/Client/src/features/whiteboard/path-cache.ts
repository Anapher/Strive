import { uncompressPathData } from './path-compression';
import { CanvasObject, VersionedCanvasObject, WhiteboardCanvas } from './types';

export default class PathCache {
   private uncompressedPaths = new Map<string, CanvasObject>();

   public preprocess(canvas: WhiteboardCanvas): WhiteboardCanvas {
      const requiredPathIds = Object.fromEntries(Array.from(this.uncompressedPaths.keys()).map((x) => [x, false]));

      const uncompressPath = (obj: VersionedCanvasObject) => {
         requiredPathIds[obj.id] = true;

         let cached: any = this.uncompressedPaths.get(obj.id);
         if (!cached) {
            cached = uncompressPathData(obj.data);
            this.uncompressedPaths.set(obj.id, cached);
         }

         return { ...obj, data: { ...obj.data, path: cached } };
      };

      const result = {
         ...canvas,
         objects: canvas.objects.map((x) => (x.data.type === 'path' ? uncompressPath(x) : x)),
      };

      for (const [id, used] of Object.entries(requiredPathIds)) {
         if (!used) {
            this.uncompressedPaths.delete(id);
         }
      }

      return result;
   }
}
