using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod
{
    public class ArchaeaMod : Mod
    {
        public void SetModInfo(out string name, ref ModProperties properties)
        {
            name = "Archaea Mod";
            properties.Autoload = true;
            properties.AutoloadBackgrounds = true;
            properties.AutoloadGores = true;
            properties.AutoloadSounds = true;
        }
    }
}
