using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;


namespace ArchaeaMod_Debug
{
    public class ArchaeaPlayer : ModPlayer
    {
        private bool once;
        private bool debugDraw;
        private int previousNPC;
        public override void PreUpdate()
        {
            if (!once)
            {
                if (KeyPress(Keys.LeftShift) && KeyPress(Keys.LeftAlt))
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
                player.position = Main.MouseWorld;
                player.fallStart = (int)player.position.Y;
            }
            if (!ArchaeaMod_Debug.prompt)
            {
                Main.NewText("Typing /info and pressing Left Shift before pressing Enter shows/enters commands");
                ArchaeaMod_Debug.prompt = true;
            }
            foreach (NPC npc in Main.npc)
                if (npc.active && npc.life > 0)
                    if (npc.type >= 580)
                    {
                        Debug.BasicInfo(npc.TypeName, npc.position.ToString());
                        break;
                    }
            if (KeyPress(Keys.F2))
                debugDraw = !debugDraw;
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
                            "Monolith [boss]: " + ModNPCID.Monolith,
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
        public NPC GetNPCByName(int type)
        {
            string name = string.Empty;
            if (type == ModNPCID.Fanatic)
                name = "Fanatic";
            if (type == ModNPCID.Hatchling)
                name = "Hatchling_head";
            if (type == ModNPCID.ItchySlime)
                name = "Slime_Itchy";
            if (type == ModNPCID.Marauder)
                name = "Sky_2";
            if (type == ModNPCID.MercurialSlime)
                name = "Slime_Mercurial";
            if (type == ModNPCID.Mimic)
                name = "Mimic";
            if (type == ModNPCID.Observer)
                name = "Sky_1";
            return mod.GetNPC(name).npc;
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
        private int timer;
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (debugDraw && !Main.playerInventory)
                Debug.Draw(Main.spriteBatch);
            if (timer++ < 600)
            {
            //  string text = "Typing /info and pressing Left Shift shows/enters commands";
            //  SpriteBatch sb = Main.spriteBatch;
            //  sb.DrawString(Main.fontMouseText, text, new Vector2(Main.spawnTileX * 16 - text.Length * 2.5f, Main.spawnTileY * 16) - Main.screenPosition, Color.White);
            }
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
