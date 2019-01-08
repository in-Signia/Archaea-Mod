using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod_Debug
{
    public class ArchaeaMod_Debug : Mod
    {
        public static bool prompt;
        public void SetModInfo(out string name, ref ModProperties properties)
        {
            name = "Archaea Mod Debug";
            properties.Autoload = true;
            properties.AutoloadBackgrounds = true;
            properties.AutoloadGores = true;
            properties.AutoloadSounds = true;
        }
    }
}
