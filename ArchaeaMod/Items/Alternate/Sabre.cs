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

namespace ArchaeaMod.Items
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

        private float upward = 0.5f;
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.MouseWorld.X > player.position.X)
                player.direction = 1;
            else player.direction = -1;
            Projectile.NewProjectileDirect(player.direction == 1 ? hitbox.TopRight() : hitbox.TopLeft(), NPCs.ArchaeaNPC.AngleToSpeed(player.direction == 1 ? upward * -1 : (float)Math.PI + upward, 6f), mod.ProjectileType<Pixel>(), item.damage, item.knockBack, player.whoAmI, Pixel.Fire);
        }
    }
}
