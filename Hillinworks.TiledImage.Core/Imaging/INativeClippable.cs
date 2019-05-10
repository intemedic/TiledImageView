using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Hillinworks.TiledImage.Imaging
{
    public interface INativeClippable
    {
        Task<BitmapSource> ClipAsync(
            Int32Rect bounds,
            int layer,
            int lodLevel,
            CancellationToken cancellationToken = default);
    }
}
