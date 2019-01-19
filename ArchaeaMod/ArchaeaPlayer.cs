using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ArchaeaMod
{
    public class ArchaeaPlayer : ModPlayer
    {
        private bool once;
        public override void PreUpdate()
        {
            if (!once)
            {
                if (KeyPress(Keys.LeftShift))
                {
                    for (int i = 0; i < Main.rightWorld / 16; i++)
                        for (int j = 0; j < Main.bottomWorld / 16; j++)
                        {
                            Main.Map.Update(i, j, 255);
                            Main.Map.ConsumeUpdate(i, j);
                            once = true;
                        }
                }
            }
        }
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public static bool KeyHold(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }
    }

    public class Draw
    {
        public const float radian = 0.017f;
        public float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
}
