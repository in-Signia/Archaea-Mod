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
    public class c_crystalsmall : ModTile
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
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            drop = mod.ItemType("cinnabar_crystal");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cinnabar Crystal");
            AddMapEntry(new Color(210, 110, 110), name);
            disableSmartCursor = true;
            mineResist = 1.2f;
            minPick = 55;
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
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("cinnabar_crystal"), 1, true, 0, true, false);
        }

        bool tileCheckFlip = false;
        int x, y;
        float rotation;
        Texture2D texture;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {   
            SpriteEffects effects = SpriteEffects.None;
            Tile tile = Main.tile[i, j];

            if (Main.tile[i, j - 1].slope() != 0 ||
                Main.tile[i, j + 1].slope() != 0 ||
                Main.tile[i - 1, j].slope() != 0 ||
                Main.tile[i + 1, j].slope() != 0)
            {
                tile.type = 0;
                tile.active(false);
                //  WorldGen.KillTile(i, j, false, false, true);
            }

            texture = Main.tileTexture[Type];
            
            if (!tileCheckFlip && Main.rand.NextFloat() >= 0.50f)
            {
                effects = SpriteEffects.FlipHorizontally;
                tileCheckFlip = true;
            }
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(texture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(0, 0, 16, 16),
                Lighting.GetColor(i, j), 0f, default(Vector2), 1f, effects, 0f);
            return false;
        }
    }
}
