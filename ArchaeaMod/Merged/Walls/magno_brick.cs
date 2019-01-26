﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Walls
{
    public class magno_brick : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("magno_brickwall");
            AddMapEntry(new Color(80, 10, 10));
        }
    }
}
