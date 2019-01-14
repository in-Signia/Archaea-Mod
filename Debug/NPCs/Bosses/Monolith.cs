using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod_Debug.NPCs.Bosses
{
    public class Monolith : Sky_air
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monolith");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 5000;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = 45000;
            npc.alpha = 255;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
        }

        private bool flip;
        private int ai = -2;
        private int elapsed = 180;
        private float f;
        private const int
            Begin = -2,
            FadeIn = -1,
            SpawnParts = 0,
            End = 1;
        public override bool JustSpawned()
        {
            proximity = false;
            if (timer % elapsed / 24 == 0 && timer != 0)
                flip = !flip;
            if (ai < SpawnParts)
                npc.velocity.Y = flip ? 0.5f : -0.5f;
            else npc.velocity.Y = 0f;
            switch (ai)
            {
                case Begin:
                    move = ArchaeaNPC.FindEmptyRegion(target(), ArchaeaNPC.defaultBounds(target()));
                    if (move != Vector2.Zero)
                    {
                        npc.position = move;
                        npc.alpha = 250;
                        goto case FadeIn;
                    }
                    break;
                case FadeIn:
                    ai = FadeIn;
                    if (npc.alpha > 0)
                        npc.alpha -= 5;
                    else goto case 0;
                    break;
                case SpawnParts:
                    ai = SpawnParts;
                    if (timer % elapsed / 4 == 0 && timer != 0)
                    {
                        f += (float)Math.PI * 2f / 4f;
                        Vector2 around = ArchaeaNPC.AngleBased(npc.Center, f, npc.width * 8f);
                        NPC.ReleaseNPC((int)around.X, (int)around.Y, ModNPCID.MonolithOrb, 0, 255);
                    }
                    if (f == (float)Math.PI * 2f)
                        goto case End;
                    break;
                case End:
                    return true;
            }
            return false;
        }
    }
}
