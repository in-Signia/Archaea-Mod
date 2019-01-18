﻿using System;
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
    public class r_Flail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Flail");
            Tooltip.SetDefault("Scatters rust");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 10;
            item.knockBack = 2f;
            item.value = 3500;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 8f;
            item.channel = true;
            item.noUseGraphic = true;
        }

        private Projectile proj;
        public override void HoldItem(Player player)
        {
            if (proj != null && proj.active)
                player.controlUseItem = true;
        }
        public override bool UseItem(Player player)
        {
            if (proj == null || !proj.active)
            {
                proj = Throw(player, Flail.Fling);
                return true;
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (proj == null || !proj.active)
            {
                proj = Throw(player, Flail.Swing);
                return true;
            }
            return false;
        }
        protected Projectile Throw(Player player, int ai)
        {
            float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, Main.MouseWorld);
            Vector2 velocity = NPCs.ArchaeaNPC.AngleToSpeed(angle, item.shootSpeed);
            return Projectile.NewProjectileDirect(new Vector2(ArchaeaItem.StartThrowX(player), player.Center.Y - 24f), velocity, mod.ProjectileType<Flail>(), item.damage, item.knockBack, player.whoAmI, ai);
        }
    }

    public class Flail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flail");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
        }

        private int ai = -1;
        private int style
        {
            get { return (int)projectile.ai[0]; }
        }
        public const int
            Fling = 1,
            Swing = 2;
        private Vector2 oldMouse;
        private Vector2 mouse;
        private Vector2 velocity;
        private Player owner
        {
            get { return Main.player[projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    goto case 0;
                case 0:
                    ai = 0;
                    if (ArchaeaItem.Elapsed(5))
                        Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire);
                    break;
            }
            return true;
        }
        private bool collide;
        private bool reach = true;
        private int time;
        private const int maxTime = 180;
        private float angle;
        private float speed = 1f;
        private float range = 200f;
        private float distance;
        public override void AI()
        {
            switch (style)
            {
                case Fling:
                    if (projectile.Distance(owner.Center) > range)
                        collide = true;
                    break;
                case Swing:
                    if (!Main.mouseRight)
                        collide = true;
                    if (owner.controlUseItem)
                        projectile.timeLeft = 50;
                    if (projectile.Distance(owner.Center) > range)
                    {
                        velocity = Vector2.Zero;
                        angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, owner.Center);
                        Vector2 reverse = NPCs.ArchaeaNPC.AngleToSpeed(angle, 4f);
                        Vector2.Add(ref projectile.velocity, ref reverse, out projectile.velocity);
                        if (reach)
                        {
                            velocity = NPCs.ArchaeaNPC.AngleToSpeed(angle + (float)Math.PI, 8f);
                            for (int i = 0; i < 8; i++)
                                Projectile.NewProjectileDirect(projectile.position + new Vector2(Main.rand.NextFloat(projectile.width), Main.rand.NextFloat(projectile.height)), velocity, mod.ProjectileType<Pixel>(), projectile.damage, projectile.knockBack, owner.whoAmI, Pixel.Fire, Pixel.Sword);
                            for (int j = 0; j < 6; j++)
                                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire);
                            reach = false;
                        }
                        break;
                    }
                    else angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, Main.MouseWorld);
                    if (projectile.Center.Y < owner.Center.Y && !HitTile())
                        projectile.velocity.Y += 0.665f * 3f;
                    else projectile.velocity = Vector2.Zero;
                    velocity += NPCs.ArchaeaNPC.AngleToSpeed(angle, 0.5f);
                    NPCs.ArchaeaNPC.VelocityClamp(velocity, -6f, 6f);
                    projectile.Center += velocity;
                    break;
            }
            if (collide)
            {
                angle = NPCs.ArchaeaNPC.AngleTo(projectile.Center, owner.Center);
                projectile.Center += NPCs.ArchaeaNPC.AngleToSpeed(angle, 2f * (speed += 0.25f));
                if (projectile.Hitbox.Intersects(owner.Hitbox))
                    projectile.active = false;
            }
            if (!reach)
            {
                if (time++ > maxTime)
                {
                    time = 0;
                    reach = true;
                }
            }
        }

        private bool collision;
        public void FloatyAI()
        {
            if (TileCollide() != Collide.None && !collision)
            {
                switch (TileCollide())
                {
                    case Collide.Bottom:
                        projectile.velocity.Y = 0f;
                        break;
                    case Collide.Left:
                        projectile.velocity.X = 0f;
                        break;
                    case Collide.Right:
                        projectile.velocity.X = 0f;
                        break;
                    case Collide.Top:
                        projectile.velocity.Y = 0f;
                        break;
                }
                collision = true;
            }
            if (collision)
            {
                if (TileCollide() != Collide.Bottom)
                {
                    projectile.velocity.Y += 0.655f;
                }
            }
            NPCs.ArchaeaNPC.VelocityClamp(projectile, -6f, 6f);
        }
        protected bool HitTile()
        {
            for (int l = -8; l < projectile.height + 8; l++)
            {
                for (int k = -8; k < projectile.width + 8; k++)
                {
                    int i = (int)projectile.position.X / 16 + k;
                    int j = (int)projectile.position.Y / 16 + l;
                    Tile tile = Main.tile[i, j];
                    if (tile.active() && Main.tileSolid[tile.type])
                        return true;
                }
            }
            return false;
        }
        protected Collide TileCollide()
        {
            for (int l = -8; l < projectile.height + 8; l++)
            {
                for (int k = -8; k < projectile.width + 8; k++)
                {
                    int i = (int)projectile.position.X / 16 + k;
                    int j = (int)projectile.position.Y / 16 + l;
                    Tile top = Main.tile[i, j - 1];
                    Tile left = Main.tile[i - 1, j];
                    Tile bottom = Main.tile[i, j + 1];
                    Tile right = Main.tile[i + 1, j];
                    if (top.active() && Main.tileSolid[top.type])
                        return Collide.Top;
                    if (left.active() && Main.tileSolid[left.type])
                        return Collide.Left;
                    if (bottom.active() && Main.tileSolid[bottom.type])
                        return Collide.Bottom;
                    if (right.active() && Main.tileSolid[right.type])
                        return Collide.Right;
                }
            }
            return Collide.None;
        }
        public enum Collide : byte
        {
            None = 0,
            Top = 1,
            Left = 2,
            Bottom = 3,
            Right = 4
        }
    }
}
