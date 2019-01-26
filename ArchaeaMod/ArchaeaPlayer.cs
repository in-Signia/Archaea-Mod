using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ArchaeaMod
{
    public class ArchaeaPlayer : ModPlayer
    {
        #region biome
        public bool MagnoBiome;
        public bool SkyFort;
        public override void UpdateBiomes()
        {
            ArchaeaWorld modWorld = mod.GetModWorld<ArchaeaWorld>();
            MagnoBiome = modWorld.MagnoBiome;
            SkyFort = modWorld.SkyFort;
        }
        public override bool CustomBiomesMatch(Player other)
        {
            if (MagnoBiome)
                return MagnoBiome = other.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome;
            else if (SkyFort)
                return SkyFort = other.GetModPlayer<ArchaeaPlayer>(mod).SkyFort;
            return false;
        }
        public override void CopyCustomBiomesTo(Player other)
        {
            other.GetModPlayer<ArchaeaPlayer>(mod).MagnoBiome = MagnoBiome;
            other.GetModPlayer<ArchaeaPlayer>(mod).SkyFort = SkyFort;
        }
        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flag = new BitsByte();
            flag[0] = MagnoBiome;
            flag[1] = SkyFort;
            writer.Write(flag);
        }
        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flag = reader.ReadByte();
            MagnoBiome = flag[0];
            SkyFort = flag[1];
        }
        #endregion
        private bool once;
        public override void PreUpdate()
        {
            if (!once)
            {
                if (KeyPress(Keys.LeftAlt))
                {
                    for (int i = 0; i < Main.rightWorld / 16; i++)
                        for (int j = 0; j < Main.bottomWorld / 16; j++)
                        {
                            Main.Map.Update(i, j, 255);
                            Main.Map.ConsumeUpdate(i, j);
                            once = true;
                        }
                }
            }
            if (KeyHold(Keys.LeftControl) && LeftClick())
            {
                player.Teleport(Main.MouseWorld);
            }
            if (KeyPress(Keys.LeftShift))
            {
                if (Main.chatText.StartsWith("/info"))
                {
                    if (Main.chatText.Contains("npc"))
                    {
                        string[] info = new string[]
                        {
                            "Mimic: " + ModNPCID.Mimic,
                            "Fanatic: " + ModNPCID.Fanatic,
                            "Hatchling: " + ModNPCID.Hatchling,
                            "Mercury Slime: " + ModNPCID.MercurialSlime,
                            "Mango Slime: " + ModNPCID.ItchySlime,
                            "Observer: " + ModNPCID.Observer,
                            "Marauder: " + ModNPCID.Marauder,
                            "Magnoliac [boss]: " + ModNPCID.MagnoliacHead,
                            "Sky [boss]" + ModNPCID.SkyBoss
                        };
                        Main.chatText = string.Empty;
                        Main.chatRelease = true;
                        foreach (string s in info)
                            Main.NewText(s);
                        return;
                    }
                    if (Main.chatText.Contains("item"))
                    {
                        string[] info = new string[]
                        {
                            "Deflector: " + ModItemID.Deflector,
                            "Sabre: " + ModItemID.Sabre
                        };
                        Main.chatText = string.Empty;
                        Main.chatRelease = true;
                        foreach (string s in info)
                            Main.NewText(s);
                        return;
                    }
                    Main.chatText = string.Empty;
                    Main.chatRelease = true;
                    Main.NewText("/info npc, /npc [type], /item [type], /spawn, hold Left Control and click to go to mouse");
                }
                if (Main.chatText.StartsWith("/"))
                {
                    if (Main.chatText.StartsWith("/npc"))
                    {
                        string text = Main.chatText.Substring(Main.chatText.IndexOf(' ') + 1);
                        int count = 1;
                        if (text.Contains(" "))
                            int.TryParse(text.Substring(text.LastIndexOf(' ') + 1), out count);
                        int type;
                        int.TryParse(text.Substring(0, 3), out type);
                        for (int i = 0; i < count; i++)
                            NPC.NewNPC((int)player.position.X - 20 * count, (int)player.position.Y, type);
                        if (Main.chatText.Contains("strike"))
                            foreach (NPC npc in Main.npc)
                                if (npc.active && !npc.friendly && npc.life > 0)
                                    npc.StrikeNPC(npc.lifeMax, 0f, 1, true);
                    }
                    if (Main.chatText.StartsWith("/item"))
                    {
                        if (Main.chatText.Contains("name"))
                        {
                            Main.NewText(mod.ItemType(Main.chatText.Substring(Main.chatText.LastIndexOf(' ') + 1)));
                            return;
                        }
                        string text = Main.chatText;
                        int type;
                        int.TryParse(text.Substring(text.IndexOf(' ') + 1), out type);
                        Item.NewItem(player.position, type, 10);
                    }
                    if (Main.chatText.StartsWith("/spawn"))
                        player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
                    Main.chatText = string.Empty;
                    Main.chatRelease = true;
                }
            }
        }
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public static bool KeyHold(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }
    }

    public class Draw
    {
        public const float radian = 0.017f;
        public float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
}
