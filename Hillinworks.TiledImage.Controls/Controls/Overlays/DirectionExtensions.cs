using System;

namespace Hillinworks.TiledImage.Controls.Overlays
{
    public static class DirectionExtensions
    {
        public static string ToDirectionString(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Northwest:
                    return "Northwest";
                case Direction.Northeast:
                    return "Northeast";
                case Direction.Southwest:
                    return "Southwest";
                case Direction.Southeast:
                    return "Southeast";
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}