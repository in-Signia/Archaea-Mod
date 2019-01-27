﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Tiles
{
    public class sky_lightbrick : ModTile
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            return false;
        }
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            drop = mod.ItemType("sky_lightbrick");
            AddMapEntry(Color.AntiqueWhite);
            soundStyle = 0;
            soundType = 21;
        }
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = mod.DustType<Merged.Dusts.c_silver_dust>();
            makeDust = true;
            color = Color.White;
        }
    }
}
