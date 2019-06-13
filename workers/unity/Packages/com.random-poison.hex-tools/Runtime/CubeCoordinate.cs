using UnityEngine;

namespace HexTools
{
    public struct CubeCoordinate
    {
        public int Q;
        public int R;
        public int S;

        public CubeCoordinate(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }
    }

    public struct FractionalCubeCoordinate
    {
        public float Q;
        public float R;
        public float S;

        public FractionalCubeCoordinate(float q, float r, float s)
        {
            Q = q;
            R = r;
            S = s;
        }

        /// <summary>
        /// Calculates the coordinates of the grid cell that contains the given
        /// hex position.
        /// </summary>
        ///
        /// <param name="fractionalPos">
        /// A position on the hex grid, represented in axial coordinates.
        /// </param>
        public CubeCoordinate Round()
        {
            var rx = Mathf.Round(Q);
            var ry = Mathf.Round(R);
            var rz = Mathf.Round(S);

            var x_diff = Mathf.Abs(rx - Q);
            var y_diff = Mathf.Abs(ry - R);
            var z_diff = Mathf.Abs(rz - S);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if (y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new CubeCoordinate(
                Mathf.RoundToInt(rx),
                Mathf.RoundToInt(ry),
                Mathf.RoundToInt(rz));
        }

        public static implicit operator FractionalCubeCoordinate(FractionalAxialCoordinate axial)
        {
            var q = axial.Q;
            var r = axial.R;
            var s = -q - r;
            return new FractionalCubeCoordinate(q, r, s);
        }
    }
}
