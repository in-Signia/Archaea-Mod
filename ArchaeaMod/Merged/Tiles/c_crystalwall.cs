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
    public class c_crystalwall : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileNoSunLight[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(2);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            drop = mod.ItemType("cinnabar_crystal");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cinnabar Crystal");
            AddMapEntry(new Color(210, 110, 110), name);
            disableSmartCursor = true;
            torch = false;
            soundStyle = 27;
            soundType = 2;
            mineResist = 1f;
            minPick = 45;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.604f;
            g = 0.161f;
            b = 0.161f;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
             return false;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        int x, y;
        int frame = 1;
        float rotation;
        Texture2D texture;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            if (Main.tile[i, j - 1].slope() != 0 ||
                Main.tile[i, j + 1].slope() != 0 ||
                Main.tile[i - 1, j].slope() != 0 ||
                Main.tile[i + 1, j].slope() != 0)
            {
                Main.tile[i, j - 1].slope(0);
                Main.tile[i, j + 1].slope(0);
                Main.tile[i - 1, j].slope(0);
                Main.tile[i + 1, j].slope(0);
            }
            if (Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].active())
            {
            //  tile.type = 0;
            //  tile.active(false);
            }

            SpriteEffects effects = SpriteEffects.None;
            texture = Main.tileTexture[Type];
            effects = SpriteEffects.None;
            
            /// rotation
            if (Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].active() && Main.tile[i, j + 1].type != 0)
                frame = 3;
            if (Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].active() && Main.tile[i, j - 1].type != 0)
                frame = 0;
            if (Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].active() && Main.tile[i + 1, j].type != 0)
                frame = 2;
            if (Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].active() && Main.tile[i - 1, j].type != 0)
                frame = 1;
            ///

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(texture, 
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(0, tile.frameY + frame * 18, 16, 16), 
                Lighting.GetColor(i, j), 0f, default(Vector2), 1f, effects, 0f);
            return false;
        }
    }
}
