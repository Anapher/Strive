import { fabric } from 'fabric';
import simplify from 'simplify-js';

/**
 * Compress the path by simplifying the points
 * @param data the path data
 */
export function compressPathData(data: any): void {
   const simplifiedPoints = simplify(data.path.map((x: any) => ({ x: x[1], y: x[2] })));

   const brush = new fabric.PencilBrush();
   const path = brush.convertPointsToSVGPath(simplifiedPoints.map(({ x, y }) => new fabric.Point(x, y)));

   console.log(path);

   data.path = fixPathGetInstructions(path);
}

/**
 * Return the path data for fabric js
 * @param segments the input segments, something like ["M ", 60.4315, " ", 10.205996948242188, " ", "Q ", 60.4325, ...]
 */
function fixPathGetInstructions(segments: string[]): (string | number)[][] {
   if (segments.length === 0) return [];

   const result = new Array<(string | number)[]>();
   let currentChunk = new Array<string | number>();

   const addCurrentChunk = () => {
      if (currentChunk.length === 0) return;
      if (currentChunk[currentChunk.length - 1] === ' ') currentChunk.pop();

      result.push(currentChunk);
      currentChunk = [];
   };

   for (const s of segments) {
      if (typeof s === 'number') {
         currentChunk.push(s);
         continue;
      }

      if (s[0] === 'M' || s[0] === 'Q' || s[0] === 'L') {
         addCurrentChunk();
      }
      if (s === ' ') continue;

      currentChunk.push(s.trim());
   }

   addCurrentChunk();
   return result;
}
