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
    public class Sky_2 : Sky_ground
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marauder");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 350;
            npc.lavaImmune = true;
        }

        private bool isBlind;
        private bool direction
        {
            get { return target().position.X > npc.position.X; }
        }
        public bool FacingWall()
        {
            if (Collision.SolidCollision(npc.position + new Vector2(-8f, 0f), npc.width + 16, npc.height))
                return true;
            else return false;
        }
        public bool OnGround()
        {
            int x = (int)npc.position.X;
            int y = (int)npc.position.Y + npc.height;
            for (int j = y; j < y + 16; j++)
                for (int i = x; i < x + npc.width; i++)
                {
                    Tile tile = Main.tile[i / 16, j / 16];
                    if (tile.active() && (Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]))
                        return true;
                }
            return false;
        }
        private int timer
        {
            get { return (int)npc.ai[0]++; }
            set { npc.ai[0] = value; }
        }
        private int interval = 600;
        private int ai = 0;
        private float randY
        {
            get { return Main.rand.NextFloat(7f, 10f); }
        }
        private float velX;
        public override void AI()
        {
            if (timer > interval)
                timer = 0;
            foreach (Projectile a in bombs)
                if (a != null && a.active)
                    if (a.Hitbox.Intersects(target().Hitbox))
                        a.timeLeft = 1;
            if (pattern != Pattern.Attack)
                if (npc.alpha > 0)
                    npc.alpha--;
            npc.direction = direction ? 1 : -1;
            if (target().Distance(npc.position) < range && canSee())
                if (npc.velocity.Y == 0f && (timer % 150 == 0 || ArchaeaNPC.OnHurt(npc.life, oldLife, out oldLife)))
                {
                    velX = npc.velocity.X;
                    npc.velocity.Y -= randY;
                }
            if (!OnGround())
                npc.velocity.X = velX;
            if (!canSee() || !target().active || target().dead)
            {
                if (FacingWall())
                    isBlind = !isBlind;
                if (OnGround())
                    npc.velocity.X -= isBlind ? -0.2f : 0.2f;
                ArchaeaNPC.VelClampX(npc, -2f, 2f);
            }
        }
        public override bool JustSpawned()
        {
            isBlind = Main.rand.Next(2) == 0;
            oldLife = npc.life;
            return base.JustSpawned();
        }
        public override float BeginInRange()
        {
            if (canSee())
                npc.velocity.X -= direction ? 8f * -1 : 8f;
            ai = 0;
            return 1f;
        }

        public override bool ActiveMovement()
        {
            if (timer % 300 == 0)
                ai++;
            switch (ai)
            {
                case 0:
                    if (canSee() && OnGround())
                        npc.velocity.X -= direction ? 0.25f * -1 : 0.25f;
                    if (timer > 90)
                        ArchaeaNPC.VelClampX(npc, -3f, 3f);
                    break;
                case 1:
                    ai = 0;
                    return true;
            }
            return false;
        }
        public override bool PostActive()
        {
            return true;
        }

        public override void AttackMovement()
        {
            ActiveMovement();
        }
        private int projIndex;
        private Projectile[] bombs = new Projectile[6];
        public override void Attack()
        {
            projIndex = 0;
            for (int k = -3; k <= 3; k += 2)
            {
                bombs[projIndex] = Projectile.NewProjectileDirect(npc.Center, ArchaeaNPC.AngleToSpeed(ArchaeaNPC.RandAngle(), 4f), ProjectileID.BouncyGrenade, 15, 5f);
                bombs[projIndex].timeLeft = 180;
                bombs[projIndex].hostile = true;
                bombs[projIndex].friendly = false;
                projIndex++;
            }
        }
    }
}
