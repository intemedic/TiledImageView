using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Hillinworks.TiledImage.Imaging.Sources
{
    public interface IImageSource : IDisposable
    {
        /// <summary>
        ///     Get the dimensions of this image.
        /// </summary>
        Dimensions Dimensions { get; }

        /// <summary>
        ///     Get the Level of Detail information of this image.
        /// </summary>
        LODInfo LOD { get; }

        /// <summary>
        ///     Load a tile asynchronously.
        /// </summary>
        Task<BitmapSource> LoadTileAsync(
            TileIndex.Full index,
            IProgress<double> progress = null,
            CancellationToken cancellationToken = default);

        Task<BitmapSource> LoadNamedImageAsync(
            string name,
            IProgress<double> progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a thumbnail of the whole image.
        /// </summary>
        /// <remarks>
        /// The thumbnail will have the smallest possible size larger than <paramref name="minWidth"/> and <paramref name="minHeight"/>.
        /// </remarks>
        Task<BitmapSource> CreateThumbnailAsync(
            double minWidth,
            double minHeight,
            CancellationToken cancellationToken = default);
    }
}