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
    public class Hatchling_body : ModNPC
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
        }
        private float acc = 1f;
        private Vector2 partEnd
        {
            get { return ArchaeaNPC.AngleBased(new Vector2(part.position.X + part.width, part.position.Y + part.height / 2), part.rotation, part.width); }
        }
        private Vector2 start
        {
            get { return ArchaeaNPC.AngleBased(new Vector2(npc.position.X, npc.position.Y + npc.height / 2), npc.rotation, npc.width); }
        }
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
            npc.active = part.type == mod.NPCType<Hatchling_head>() ? part.active : lead.active;
            Digger.DiggerPartsAI(npc, part, ref acc);
            return true;
        }
        public override bool CheckActive()
        {
            return false;
        }
    }
}
