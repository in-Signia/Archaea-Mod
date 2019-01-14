using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod_Debug.Items
{
    public class Sabre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sabre");
            Tooltip.SetDefault("Forged from the materials of magno");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 3f;
            item.value = 3000;
            item.rare = ItemRarityID.Green;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
        }

        private int count;
        private float upward = 0.5f;
        public override bool UseItem(Player player)
        {
            count = 0;
            if (Main.MouseWorld.X > player.position.X)
                player.direction = 1;
            else player.direction = -1;
            return true;
        }
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (count++ % 4 == 0)
                Projectile.NewProjectileDirect(player.direction == 1 ? hitbox.TopRight() : hitbox.TopLeft(), NPCs.ArchaeaNPC.AngleToSpeed(player.direction == 1 ? upward * -1 : (float)Math.PI + upward, 6f), mod.ProjectileType<Pixel>(), item.damage, item.knockBack, player.whoAmI, Pixel.Fire);
        }
    }

    public class Pixel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.damage = 0;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Black * 0f;
        }
        private bool direction;
        private int ai;
        private float rotate;
        private Dust dust;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case 0:
                    direction = owner.direction == 1 ? true : false;
                    rotate = direction ? 0f : (float)Math.PI;
                    dust = SetDust();
                    goto case 1;
                case 1:
                    ai = 1;
                    break;
            }
            return true;
        }
        public override void AI()
        {
            NPCs.ArchaeaNPC.RotateIncrement(true, ref rotate, (float)Math.PI / 2f, 0.15f, out rotate);
            projectile.velocity += NPCs.ArchaeaNPC.AngleToSpeed(rotate, 0.25f);
            dust.position = projectile.position;
        }
        public override void Kill(int timeLeft)
        {
            NPCs.ArchaeaNPC.DustSpread(projectile.Center, 6, 4, 2f);
        }
        public const int
            Default = 0,
            Fire = 1,
            Dark = 2;
        public Dust SetDust()
        {
            switch ((int)projectile.ai[0])
            {
                case Fire:
                    return Dust.NewDustDirect(projectile.Center, 2, 2, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
            }
            return null;
        }
    }
}
