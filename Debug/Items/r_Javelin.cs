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
    public class r_Javelin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rust Javelin");
            Tooltip.SetDefault("Rusty but still useful");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 0;
            item.knockBack = 0f;
            item.value = 100;
            item.rare = 1;
            item.useTime = 30;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.maxStack = 250;
            item.consumable = true;
            item.shoot = mod.ProjectileType<Javelin>();
            item.shootSpeed = 6f;
            item.thrown = true;
        }
    }

    public class Javelin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Javelin");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.damage = 10;
            projectile.knockBack = 0f;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.friendly = true;
        }
        private int ai = -1;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    projectile.rotation = NPCs.ArchaeaNPC.AngleTo(owner.Center, Main.MouseWorld);
                    rotate = projectile.rotation;
                    projectile.position = new Vector2(ArchaeaItem.StartThrowX(owner), projectile.position.Y - 16f);
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }
        private int time = 90;
        private float rotate;
        private float x;
        private float y;
        private Vector2 hit;
        private Target target;
        private NPC npc;
        public override void AI()
        {
            if (ArchaeaItem.Elapsed(5))
                Dust.NewDustDirect(projectile.Center, 1, 1, DustID.Fire);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
        }
    }
}
