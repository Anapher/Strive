import { Size } from 'src/types';

export function expandToBox(content: Size, boundingBox: Size): Size {
   let widthScale = 0;
   let heightScale = 0;

   if (content.width != 0) widthScale = boundingBox.width / content.width;
   if (content.height != 0) heightScale = boundingBox.height / content.height;

   const scale = Math.min(widthScale, heightScale);
   return { width: content.width * scale, height: content.height * scale };
}

export function maxWidth(size: Size, maxWidth: number): Size {
   if (size.width > maxWidth) {
      return { width: maxWidth, height: (size.height / size.width) * maxWidth };
   }

   return size;
}

type ComputedGrid = {
   containerWidth: number;
   containerHeight: number;
   rows: number;
   itemsPerRow: number;
   itemSize: Size;
};

export function generateGrid(
   itemsCount: number,
   itemMaxWidth: number,
   boundingBox: Size,
   spacing: number,
): ComputedGrid {
   // maximize width of all items
   let result: ComputedGrid | undefined;

   for (let rows = 1; rows < itemsCount + 1; rows++) {
      const itemsPerRow = Math.ceil(itemsCount / rows);

      // the spacing between the tiles
      const spacingX = spacing * (itemsPerRow - 1);
      const spacingY = spacing * (rows - 1);

      const bouncingBoxWithoutSpacing: Size = {
         width: boundingBox.width - spacingX,
         height: boundingBox.height - spacingY,
      };

      const contentSize: Size = {
         width: 16 * itemsPerRow,
         height: 9 * rows,
      };

      const rowMaxWidth = itemMaxWidth * itemsPerRow;
      const gridSize = maxWidth(expandToBox(contentSize, bouncingBoxWithoutSpacing), rowMaxWidth);

      const itemWidth = gridSize.width / itemsPerRow;
      const itemHeight = gridSize.height / rows;

      if (!result) {
         result = {
            containerWidth: gridSize.width + spacingX,
            containerHeight: gridSize.height + spacingY,
            itemSize: { width: itemWidth, height: itemHeight },
            itemsPerRow,
            rows,
         };
      } else {
         if (itemWidth > result.itemSize.width) {
            result = {
               containerWidth: gridSize.width + spacingX,
               containerHeight: gridSize.height + spacingY,
               itemSize: { width: itemWidth, height: itemHeight },
               itemsPerRow,
               rows,
            };
         }
      }
   }

   if (!result) {
      throw new Error('itemsCount must be greater than 0');
   }

   return result;
}
