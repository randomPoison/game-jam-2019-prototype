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
            var q = Mathf.Round(Q);
            var r = Mathf.Round(R);
            var s = Mathf.Round(S);

            var qDiff = Mathf.Abs(q - Q);
            var rDiff = Mathf.Abs(r - R);
            var sDiff = Mathf.Abs(s - S);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                q = -r - s;
            }
            else if (rDiff > sDiff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }

            return new CubeCoordinate(
                Mathf.RoundToInt(q),
                Mathf.RoundToInt(r),
                Mathf.RoundToInt(s));
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
