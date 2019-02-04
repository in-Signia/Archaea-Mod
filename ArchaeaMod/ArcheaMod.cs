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
        public void SetModInfo(out string name, ref ModProperties properties)
        {
            name = "Archaea Mod";
            properties.Autoload = true;
            properties.AutoloadBackgrounds = true;
            properties.AutoloadGores = true;
            properties.AutoloadSounds = true;
        }
        public override void PostSetupContent()
        {
            // Showcases mod support with Boss Checklist without referencing the mod
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            { //TO DO: create a "downed" variable in ModWorld Class
                bossChecklist.Call("AddBossWithInfo", "Magnoliace", 5.5f, (Func<bool>)(() => ArchaeaWorld.downedMagnoliac), "Use a [i:" + mod.ItemType("MagnoliacSummonItemOrWhatever") + "] in the Archaea biome");
                bossChecklist.Call("AddBossWithInfo", "Purity Spirit", 15.5f, (Func<bool>)(() => ArchaeaWorld.downedSkyBoss), "Use a [i:" + mod.ItemType("SkyBossSummonItemOrWhatever") + "] in Space");
            }
        }
    }
}
