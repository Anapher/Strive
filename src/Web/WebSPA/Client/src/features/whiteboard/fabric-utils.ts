import cuid from 'cuid';
import { fabric } from 'fabric';
import { CanvasObject } from './types';

export function objectToJson(obj: fabric.Object) {
   const json = obj.toJSON();
   delete json.version;

   const absolutePos = getObjectAbsolutePosition(obj);
   if (absolutePos.left) json.left = absolutePos.left;
   if (absolutePos.top) json.top = absolutePos.top;

   return json;
}

export function setObjectAbsolutePosition(obj: fabric.Object) {
   if (obj.group) {
      const left = obj.left;
      const top = obj.top;

      const groupLeft = obj.group.left;
      const groupTop = obj.group.top;

      const groupWidth = obj.group.width;
      const groupHeight = obj.group.height;

      if (!left || !top || !groupLeft || !groupTop || !groupWidth || !groupHeight) return;

      obj.set({
         left: left - groupLeft - groupWidth / 2,
         top: top - groupTop - groupHeight / 2,
      });
   }
}

export function applyObjectConfig(obj: fabric.Object, template: CanvasObject) {
   obj.set(template);
   setObjectAbsolutePosition(obj);
   obj.setCoords();
   obj.group?.setCoords();
}

export function getObjectAbsolutePosition(obj: fabric.Object): { left?: number; top?: number } {
   if (!obj.group) return { left: obj.left, top: obj.top };

   const left = obj.left;
   const top = obj.top;

   const groupLeft = obj.group.left;
   const groupTop = obj.group.top;

   const groupWidth = obj.group.width;
   const groupHeight = obj.group.height;

   if (!left || !top || !groupLeft || !groupTop || !groupWidth || !groupHeight)
      return { left: undefined, top: undefined };

   return {
      left: groupLeft + left + groupWidth / 2,
      top: groupTop + top + groupHeight / 2,
   };
}

export function deleteObject(canvas: fabric.Canvas, target: fabric.Object) {
   const selected: fabric.Object[] = [];
   if (target.type === 'activeSelection') {
      (target as fabric.ActiveSelection).forEachObject((obj) => selected.push(obj));
   } else {
      selected.push(target);
   }

   const deletionId = cuid();
   selected.forEach((x) => {
      (x as any).deletionId = deletionId;
      (x as any).deletedWith = selected;
   });

   canvas.remove(...selected);

   canvas.discardActiveObject();
   canvas.requestRenderAll();
}

export function getId(obj: fabric.Object) {
   return (obj as any).id as string;
}

export function setId(obj: fabric.Object, id: string) {
   (obj as any).id = id;
}

export function isLiveObj(obj: fabric.Object) {
   return (obj as any).isLive;
}

export function setIsLiveObj(obj: fabric.Object) {
   (obj as any).isLive = true;
}
