using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UnnamedTechMod;

public class UnnamedTechMod : Mod
{
    public static Player DummyPlayer = new Player();
    public static readonly List<TransportNetwork> TransportNetworks = new();
}
