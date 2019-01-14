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
    public class ArchaeaItem
    {
        public static void ActiveChannelStyle(Player player)
        {
            player.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
            float PI = (float)Math.PI;
            float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, Main.MouseWorld);
            player.itemRotation = angle + (player.direction == -1 ? PI : 0);
            player.itemLocation.X = NPCs.ArchaeaNPC.AngleBased(player.position, angle, player.width / 4).X - player.width / 2 - 4;
        }
        public static bool NotEquipped(Player player, Item item, ref int index)
        {
            for (int i = 0; i < player.armor.Length; i++)
            {
                if (player.armor[i] != item)
                    index++;
                else break;
                if (index == player.armor.Length - 1 && player.armor[i] != item)
                    return true;
            }
            index = 0;
            return false;
        }
        public static bool Elapsed(int interval)
        {
            return Math.Round(Main.time, 0) % interval == 0;
        }
    }
}
