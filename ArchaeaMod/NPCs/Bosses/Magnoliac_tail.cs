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

namespace ArchaeaMod.NPCs.Bosses
{
    public class Magnoliac_tail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnoliac");
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
        }

        private float acc = 1f;
        private NPC part
        {
            get { return Main.npc[(int)npc.ai[0]]; }
        }
        private NPC lead
        {
            get { return Main.npc[(int)npc.ai[1]]; }
        }
        public override bool PreAI()
        {
            npc.realLife = lead.whoAmI;
            Digger.DiggerPartsAI(npc, part, mod.GetNPC<Magnoliac_head>().followSpeed, ref acc);
            return true;
        }
        public override bool CheckActive()
        {
            return false;
        }
    }
}
