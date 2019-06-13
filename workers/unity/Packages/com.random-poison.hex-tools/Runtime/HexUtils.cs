using UnityEngine;

namespace HexTools
{
    public static class HexUtils
    {
        /// <summary>
        /// Calculates the fractional coordinates on the hex grid for the specified
        /// position in world space.
        /// </summary>
        public static FractionalAxialCoordinate WorldToFractional(Vector2 worldPos)
        {
            var q = worldPos.x - (1f / Mathf.Sqrt(3f)) * worldPos.y;
            var r = (2f / Mathf.Sqrt(3f)) * worldPos.y;
            return new FractionalAxialCoordinate(q, r);
        }


        /// <summary>
        /// Calculates the coordinates of the grid cell that contain the given
        /// world position.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static AxialCoordinate WorldToGrid(Vector2 worldPos)
        {
            return WorldToFractional(worldPos).Round();
        }

        /// <summary>
        /// Converts the specifid axial coordinate to world space.
        /// </summary>
        ///
        /// <remarks>
        /// Assumes a pointy-top grid orientation with a cell width of 1.
        /// </remarks>
        public static Vector2 GridToWorld(AxialCoordinate gridPos)
        {
            var Q_BASIS = new Vector2
            {
                x = 1f,
                y = 0f,
            };

            var R_BASIS = new Vector2
            {
                x = 0.5f,
                y = Mathf.Sqrt(3) / 2f,
            };


            return Q_BASIS * gridPos.Q + R_BASIS * gridPos.R;
        }
    }
}
