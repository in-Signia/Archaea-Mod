using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Graphics;

namespace ArchaeaMod_Debug
{
    public class Debug
    {
        private static string[] basic = new string[2];
        private static string[] info = new string[6];
        public static void BasicInfo(string s1 = "", string s2 = "")
        {
            basic[0] = s1;
            basic[1] = s2;
        }
        public static void ModInfo(object[] o = null)
        {
            if (o != null)
                for (int k = 0; k < o.Length; k++)
                    if (o[k] != null)
                        if (k < info.Length)
                            info[k] = o[k].ToString();
        }
        public static void Draw(SpriteBatch sb)
        {
            for (int k = 0; k < basic.Length; k++)
                if (basic[k] != null && basic[k] != string.Empty)
                    sb.DrawString(Main.fontMouseText, basic[k], new Vector2(20, 120 + k * 16), Color.White);
            for (int l = 2; l < info.Length + 2; l++)
            {
                int i = l - 2;
                if (info[i] != null && info[i] != string.Empty)
                    sb.DrawString(Main.fontMouseText, info[i], new Vector2(20, 120 + l * 16), Color.White);
            }
        }
    }
}
