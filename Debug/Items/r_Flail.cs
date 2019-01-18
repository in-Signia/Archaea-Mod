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
    public class r_Flail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Flail");
            Tooltip.SetDefault("");
        }
    }
}
