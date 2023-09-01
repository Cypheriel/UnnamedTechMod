using Terraria.DataStructures;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod;

public enum FlowType
{
    Input,
    Output
}

public class IOPort
{
    public TransportType Type;
    public Point16 Position;
    public FlowType FlowType;
    
    public IOPort(TransportType type, Point16 position, FlowType flowType)
    {
        Type = type;
        Position = position;
        FlowType = flowType;
    }
}