namespace HexTools
{
    public struct AxialCoordinate
    {
        /// <summary>
        /// The horizontal axis. Positive coordinates move to the right.
        /// </summary>
        public int Q;

        /// <summary>
        /// The diagonal axis. Positive coordinates move up and to the right.
        /// </summary>
        public int R;

        public AxialCoordinate(int q, int r)
        {
            Q = q;
            R = r;
        }

        public override string ToString()
        {
            return $"({Q}, {R})";
        }

        public static implicit operator AxialCoordinate(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.Q, cube.R);
        }
    }

    public struct FractionalAxialCoordinate
    {
        /// <summary>
        /// The horizontal axis. Positive coordinates move to the right.
        /// </summary>
        public float Q;

        /// <summary>
        /// The diagonal axis. Positive coordinates move up and to the right.
        /// </summary>
        public float R;

        public FractionalAxialCoordinate(float q, float r)
        {
            Q = q;
            R = r;
        }

        public AxialCoordinate Round()
        {
            var cube = (FractionalCubeCoordinate)this;
            return cube.Round();
        }

        public override string ToString()
        {
            return $"({Q}, {R})";
        }

        public static implicit operator FractionalAxialCoordinate(FractionalCubeCoordinate coord)
        {
            return new FractionalAxialCoordinate(coord.Q, coord.R);
        }
    }
}
