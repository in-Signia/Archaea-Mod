using Terraria;
using Terraria.ModLoader;

namespace ArchaeaMod
{
    public class ArchaeaMod : Mod
    {
        public static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public static string magnoHead = "ArchaeaMod/Gores/magno_head";
        public override void Load()
        {
            AddBossHeadTexture(magnoHead, ModNPCID.MagnoliacHead);
        }
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
