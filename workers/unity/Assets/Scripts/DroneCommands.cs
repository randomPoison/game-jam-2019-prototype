using System;
using HexTools;
using Improbable.Gdk.Core;

namespace BetaApartUranus.DroneCommands
{
    [Serializable]
    public struct MoveToPosition
    {
        public AxialCoordinate Target;
    }
    [Serializable]
    public struct HarvestResourceNode
    {
        public long Target;
    }
}
