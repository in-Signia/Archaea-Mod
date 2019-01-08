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
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod_Debug.NPCs
{
    public class Mimic : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mimic");
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 32;
            npc.lifeMax = 650;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = Item.sellPrice(0, 1, 50, 0);
            npc.lavaImmune = true;
            npc.knockBackResist = 0f;
        }
        private bool targeted;
        private bool inAir;
        private bool justJumped;
        private bool direction;
        private bool jump;
        private bool flip;
        private bool begin;
        private int targetRange = 800;
        private int timer;
        private int interval = 60;
        private int facingWall;
        private float jumpHeight = 3.4f;
        private float xMovement = 2.4f;
        private float velX;
        private Player target;
        public override bool PreAI()
        {
            target = ArchaeaNPC.FindClosest(npc, false);
            if (target != null && (!target.active || target.dead))
                targeted = false;
            if (target == null)
                return false;
            if (Main.mouseRight && npc.Hitbox.Contains(Main.MouseWorld.ToPoint()) 
                && ArchaeaNPC.WithinRange(target.position, new Rectangle(npc.Hitbox.X - 300, npc.Hitbox.Y - 200, 600, 400)))
                targeted = true;
            npc.immortal = !targeted;
            npc.target = target.whoAmI;
            if (!targeted)
                return false;
            return true;
        }
        public override void AI()
        {
            timer++;
            if (timer % 60 == 0)
                jump = Main.rand.Next(3) == 0;
            float jumpFactor = Main.rand.NextFloat(2f, 4f);
            if (npc.velocity.Y == 0f && jump)
            {
                npc.velocity.Y -= jumpHeight * jumpFactor;
                npc.direction = direction ? 1 : -1;
                velX = direction ? xMovement * jumpFactor / 2f : xMovement * jumpFactor / 2f * -1;
                jump = false;
                flip = false;
            }
            direction = flip ? direction : target.position.X > npc.position.X;
            if (Collision.SolidCollision(new Vector2(npc.position.X - 16, npc.position.Y), npc.width + 32, 16))
                facingWall++;
            if (facingWall > 180)
            {
                direction = !direction;
                flip = true;
                facingWall = 0;
            }
            inAir = npc.velocity.Y != 0f;
            if (inAir)
                npc.velocity.X = velX;
            else npc.velocity.X = 0f;
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (!targeted)
            {
                damage = 0;
                Main.PlaySound(SoundID.Tink, npc.position);
                targeted = true;
            }
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!targeted)
            {
                damage = 0;
                Main.PlaySound(SoundID.Tink, npc.position);
                targeted = true;
            }
        }
    }
}
