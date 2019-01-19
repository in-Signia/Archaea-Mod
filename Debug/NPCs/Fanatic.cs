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

namespace ArchaeaMod_Debug.NPCs
{
    public class Fanatic : Caster
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fanatic");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 100;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 1000;
            npc.lavaImmune = true;
        }

        private float compensate
        {
            get { return (float)(npcTarget.velocity.Y * (0.017d * 5d)); }
        }
        public override bool JustSpawned()
        {
            elapse = 120;
            dustType = 6;
            var dusts = ArchaeaNPC.DustSpread(npc.Center, 1, 1, dustType, 10, 3f);
            foreach (Dust d in dusts)
                d.noGravity = true;
            return true;
        }
        public override void Teleport()
        {
            var dusts = ArchaeaNPC.DustSpread(npc.Center, 1, 1, dustType, 10, 3f);
            foreach (Dust d in dusts)
                d.noGravity = true;
        }
        public override bool PreAttack()
        {
            if (timer % elapse == 0 && timer != 0)
                return true;
            else return false;
        }
        public override void Attack()
        {
            int a = Projectile.NewProjectile(npc.Center, ArchaeaNPC.AngleToSpeed(ArchaeaNPC.AngleTo(npc, npcTarget) + compensate, 4f), ProjectileID.Fireball, 10, 1f);
            Main.projectile[a].timeLeft = 300;
            Main.projectile[a].hostile = true;
            Main.projectile[a].friendly = false;
            Main.projectile[a].tileCollide = false;
        }
    }
}
