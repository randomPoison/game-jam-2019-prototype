using System;
using HexTools;

namespace BetaApartUranus.DroneCommands
{
    [Serializable]
    public struct MoveToPosition
    {
        public AxialCoordinate Target;
    }
}
