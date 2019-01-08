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
    public class Slime_Mercurial : Slime
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Slime");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 32;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.lavaImmune = true;
        }
        private int count;
        private float compensateY;
        public override bool PreAI()
        {
            return base.PreAI();
        }
        public override void AI()
        {
            if (FacingWall())
                if (timer % interval / 4 == 0)
                    if (count++ > 2)
                    {
                        oldLife = npc.life;
                        pattern = Pattern.Active;
                        count = 0;
                    }
            base.AI();
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.Next(4) == 0)
                target.AddBuff(BuffID.Darkness, 60);
        }
        #region slime methods
        public override bool JustSpawned()
        {
            flip = Main.rand.Next(2) == 0;
            return true;
        }
        public override void DefaultActions(int interval = 180, bool moveX = false)
        {
            if (timer % interval == 0 && timer != 0)
            {
                SlimeJump(jumpHeight(), moveX, speedX(), flip);
                flip = !flip;
            }
        }
        public override void Active(int interval = 180)
        {
            if (timer % interval == 0 && timer != 0)
                SlimeJump(jumpHeight(FacingWall()), true, speedX(), flip);
            if (count++ > 3)
            {
                flip = !flip;
                count = 0;
            }
        }
        public override void Attack()
        {
            if (timer % 150 == 0 && timer != 0)
                SlimeJump(jumpHeight(FacingWall()), true, speedX(), target.position.X > npc.position.X);
        }
        public override void SlimeJump(float speedY, bool horizontal = false, float speedX = 0, bool direction = true)
        {
            npc.velocity.Y -= speedY;
            if (horizontal)
                velX = direction ? speedX / 2f : speedX / 2f * -1;
        }
        #endregion
    }
}
