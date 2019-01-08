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

namespace ArchaeaMod.NPCs
{
    public class Sky_1 : Sky_air
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky_1");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 4000;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
        }
        public override bool PreDefaultMove()
        {
            move = ArchaeaNPC.FindEmptyRegion(target(), targetRange);
            if (move != Vector2.Zero)
                return true;
            else return false;
        }
        public override bool BeginActive()
        {
            if (amount < 1f)
            {
                amount += 0.0125f;
                degree = 5d * amount;
                npc.position.Y = Vector2.Lerp(idle, upper, amount).Y;
                npc.position.X += (float)Math.Cos(degree);
                return false;
            }
            else return true;
        }
        public override bool PreFadeOut()
        {
            Vector2 v = ArchaeaNPC.FindEmptyRegion(target(), targetRange);
            if (v != Vector2.Zero)
            {
                move = v;
                return true;
            }
            else return false;
        }
    }
}
