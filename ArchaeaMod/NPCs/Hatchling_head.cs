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
    public class Hatchling_head : Digger
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hatchling");
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 0;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            bodyType = mod.NPCType<Hatchling_body>();
            tailType = mod.NPCType<Hatchling_tail>();
        }
        public override bool moveThroughAir
        {
            get { return ai == 3; }
        }
        public override float leadSpeed
        {
            get { return 5.5f; }
        }
        public override float followSpeed
        {
            get { return leadSpeed * 4f; }
        }
        private Player newTarget
        {
            get
            {
                if (target() == null || !target().active || target().dead)
                    return ArchaeaNPC.FindClosest(npc, true);
                else return target();
            }
        }
        public override void PostAI()
        {
            if (npc.Distance(newTarget.position) > maxDistance)
            {
                ai = Idle;
                if (npc.timeLeft-- <= 0)
                    npc.active = false;
            }
        }
    }
}
