import { fabric } from 'fabric';

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
