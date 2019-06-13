using HexTools;

namespace BetaApartUranus
{
    public static class SchemaExtensions
    {
        public static AxialCoordinate ToAxial(this GridCoordinate coord)
        {
            // TODO: Find a better way to handler numeric conversions instead of just
            // downcasting from long to int.
            return new AxialCoordinate((int)coord.Col, (int)coord.Row);
        }
    }
}
