using System;

namespace Hillinworks.TiledImage.Controls.Overlays
{

    /// <remarks>
    /// This enum only serves for <see cref="PointQuadTree"/>. The <see cref="North"/>, <see cref="South"/>,
    /// <see cref="East"/> and <see cref="West"/> values can only be used to construct <see cref="Northwest"/>,
    /// <see cref="Northeast"/>, <see cref="Southwest"/> and <see cref="Southeast"/>.
    /// Use <see cref="DirectionExtensions.ToDirectionString"/> instead of <see cref="Enum.ToString()"/>.
    /// </remarks>
    [Flags]
    public enum Direction
    {
        North = 0 << 1,
        South = 1 << 1,
        West = 0,
        East = 1,

        Northwest = North | West,
        Northeast = North | East,
        Southwest = South | West,
        Southeast = South | East
    }
}