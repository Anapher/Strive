export function numberToColor(value: number, blueBoost = true) {
   const hue = Math.floor(value) % 341; // between 0 and 340
   let saturation = 100;
   let lightness = 50;

   // color adjustment for blue:
   if (blueBoost && hue > 215 && hue < 265) {
      const gain = 20;
      const blueness = 1 - Math.abs(hue - 240) / 25;
      const change = Math.floor(gain * blueness);
      lightness += change;
      saturation -= change;
   }

   return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}
