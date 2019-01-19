using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod_Debug.Merged.Items.Materials
{
    public class magno_core : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac Core");
        }
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 32;
            item.scale = 1f;
            item.maxStack = 99;
            item.value = 3500;
            item.rare = 3;
        }
    }
}
