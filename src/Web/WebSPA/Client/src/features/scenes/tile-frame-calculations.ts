import { Size } from 'src/types';
import { expandToBox, maxWidth, resizeMaintainAspectRatio } from './calculations';

type Margin = { top: number; left: number; bottom: number; right: number };

type TilesSceneBarState = {
   width: number;
   tileSpaceBetween: number;
   tileSize: Size;
   tileMinWidth: number;
};

export type TilesSceneBarInstructions = {
   tileAmount: number;
   tileSize: Size;
   tileSpaceBetween: number;
};

export function computeTilesSceneBar({
   width,
   tileSpaceBetween,
   tileSize,
   tileMinWidth,
}: TilesSceneBarState): TilesSceneBarInstructions {
   const tileAmount = Math.floor((width + tileSpaceBetween) / (tileMinWidth + tileSpaceBetween));

   // substract the space between for all tiles and compute the max width for each tile
   const singleTileWidth = (width - tileSpaceBetween * tileAmount + tileSpaceBetween) / tileAmount;
   const newTileSize = resizeMaintainAspectRatio(tileSize, singleTileWidth);

   return { tileAmount, tileSize: newTileSize, tileSpaceBetween };
}

export type GridTopInstructions = {
   contentDimensions: Size;
   tileSpaceBetween: number;
   tileAmount: number;
   tileSize: Size;
   tilesMargin: Margin;
   contentMargin: Margin;
};

export type TileFrameState = {
   dimensions: Size;
   contentRatio: Size;
   maxContentWidth?: number;
   tileSize: Size;
   tileMinWidth: number;
   tileSpaceBetween: number;
   tilesMargin: Margin;
   contentMargin: Margin;
};

export function computeGridTop({
   dimensions,
   contentRatio,
   maxContentWidth,
   tileSize,
   tileMinWidth,
   tileSpaceBetween,
   tilesMargin,
   contentMargin,
}: TileFrameState): GridTopInstructions {
   const tilesContainerWidth = dimensions.width - tilesMargin.left - tilesMargin.right; // margin left and right
   const dimensionWidthFixed = tilesContainerWidth + tileSpaceBetween; // because the first tile doesn't need the space between
   const tileAmount = Math.floor(dimensionWidthFixed / (tileMinWidth + tileSpaceBetween));

   // substract the space between for all tiles and compute the max width for each tile
   const singleTileWidth = (tilesContainerWidth - tileSpaceBetween * tileAmount) / tileAmount;
   const newTileSize = resizeMaintainAspectRatio(tileSize, singleTileWidth);

   const contentArea: Size = {
      width: dimensions.width - contentMargin.left - contentMargin.right,
      height:
         dimensions.height -
         newTileSize.height -
         tilesMargin.top -
         tilesMargin.bottom -
         contentMargin.top -
         contentMargin.bottom,
   };
   let contentDimensions = expandToBox(contentRatio, contentArea);
   if (maxContentWidth) contentDimensions = maxWidth(contentDimensions, maxContentWidth);

   return { contentDimensions, tileSpaceBetween, tileSize: newTileSize, tileAmount, tilesMargin, contentMargin };
}
