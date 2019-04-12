﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hillinworks.TiledImage.Imaging.Sources;
using Hillinworks.TiledImage.Utilities;

namespace Hillinworks.TiledImage.Imaging
{
    public static partial class Clipper
    {
        private static unsafe void ClearBackBuffer(this WriteableBitmap bitmap)
        {
            var length = bitmap.PixelHeight * bitmap.BackBufferStride;
            var pBuffer = (byte*)bitmap.BackBuffer;
            for (var i = 0; i < length; ++i, ++pBuffer)
            {
                *pBuffer = 0xff;
            }
        }

        public static async Task<BitmapSource> ClipAsync(
            this IImageSource imageSource,
            Int32Rect bounds,
            int? layer = null,
            int? lodLevel = null,
            CancellationToken cancellationToken = default)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (imageSource is INativeClippable nativeClippable)
            {
                return await nativeClippable.ClipAsync(bounds, layer, lodLevel, cancellationToken);
            }

            var finalLayer = layer ?? imageSource.Dimensions.MinimumLayerIndex;
            var finalLodLevel = lodLevel ?? imageSource.LOD.MinLODLevel;

            var dimensions = imageSource.GetLODDimensions(finalLodLevel);

            var tileWidth = dimensions.TileWidth;
            var tileHeight = dimensions.TileHeight;
            var tileIndexLeft = bounds.X / tileWidth;
            var tileIndexTop = bounds.Y / tileHeight;
            var tileIndexRight = (bounds.X + bounds.Width - 1) / tileWidth;
            var tileIndexBottom = (bounds.Y + bounds.Height - 1) / tileHeight;

            var copyPixelRequests = new List<CopyPixelRequest>();

            for (var row = tileIndexTop; row <= tileIndexBottom; ++row)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tileTop = row * tileHeight;
                var tileBottom = (row + 1) * tileHeight;

                var copyMetricsY = ImageCopyMetrics.Calculate(tileTop, tileBottom, bounds.Y, bounds.Height);

                for (var column = tileIndexLeft; column <= tileIndexRight; ++column)
                {
                    var tileLeft = column * tileWidth;
                    var tileRight = (column + 1) * tileWidth;

                    var index = new TileIndex.Full(column, row, finalLayer, finalLodLevel);
                    var tileImage = await imageSource.LoadTileAsync(index, cancellationToken: cancellationToken);

                    var copyMetricsX = ImageCopyMetrics.Calculate(tileLeft, tileRight, bounds.X, bounds.Width);

                    var sourceRect = new Int32Rect(
                        copyMetricsX.Source,
                        copyMetricsY.Source,
                        copyMetricsX.Size,
                        copyMetricsY.Size);

                    var destinationRect = new Int32Rect(
                        copyMetricsX.Destination,
                        copyMetricsY.Destination,
                        copyMetricsX.Size,
                        copyMetricsY.Size);

                    copyPixelRequests.Add(new CopyPixelRequest(tileImage, sourceRect, destinationRect));
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            var image = new WriteableBitmap(bounds.Width, bounds.Height, 96, 96, PixelFormats.Bgr24, null);
            image.ClearBackBuffer();

            foreach (var request in copyPixelRequests)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var source = request.TileImage.Format == PixelFormats.Bgr24
                    ? request.TileImage
                    : new FormatConvertedBitmap(request.TileImage, PixelFormats.Bgr24, null, 0);

                source.CopyPixels(
                    request.SourceRect,
                    image.BackBuffer + request.DestinationRect.Y * image.BackBufferStride +
                    request.DestinationRect.X * 3,
                    image.PixelHeight * image.BackBufferStride,
                    image.BackBufferStride);
            }

            image.Freeze();
            return image;
        }
    }
}
