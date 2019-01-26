using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArchaeaMod.Merged.Tiles
{
	public class m_ore : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSpelunker[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = false;
            dustType = 1;
			drop = mod.ItemType("magno_ore");
            //  UI map tile color
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Magno Ore");
            AddMapEntry(new Color(201, 152, 115), name);
            soundStyle = 0;
            soundType = 21;
            mineResist = 1.5f;
            minPick = 35;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}