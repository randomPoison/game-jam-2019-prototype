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
            var q = (Mathf.Sqrt(3f) / 3f) * worldPos.x - (1f / 3f) * worldPos.y;
            var r = (2f / 3f) * worldPos.y;
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

        public static Vector2 GridToWorld(AxialCoordinate gridPos)
        {
            return new Vector2()
            {
                x = Mathf.Sqrt(3f) * gridPos.Q + Mathf.Sqrt(3f) / 2f * gridPos.R,
                y = 3f / 2f * gridPos.R,
            };
        }
    }
}
