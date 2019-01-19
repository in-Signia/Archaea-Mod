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

namespace ArchaeaMod.Buffs
{
    public class CatcherBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Rusty Catcher minion");
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "Junky Rust";
        }
    }
}
