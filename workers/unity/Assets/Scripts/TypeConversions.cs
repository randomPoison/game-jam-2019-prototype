using HexTools;

namespace BetaApartUranus
{
    public static class TypeConversions
    {
        public static GridCoordinate ToGridCoordinate(this AxialCoordinate axial)
        {
            return new GridCoordinate(axial.Q, axial.R);
        }
    }
}
