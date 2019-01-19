using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Graphics;

namespace ModStatEditor
{
    public class DrawUI : ModPlayer
    {
        private string output = string.Empty;
        private string[] text;
        private int[] values;
        private bool draw;
        public bool update
        {
            get { return draw && !Main.playerInventory && !Main.mapFullscreen && !Main.gameMenu; }
        }
        private const int max = 7;
        private int selected;
        public const int
            left = 80,
            top = 160,
            margin = 32,
            width = 430;
        public static int height
        {
            get { return margin * max; }
        }
        public Rectangle main
        {
            get { return new Rectangle(left, top, width + left, height); }
        }
        private Texture2D texture
        {
            get { return Main.magicPixel; }
        }
        private TextLine itemName;
        private TextLine[] info;
        private Button enter;
        private Button[] button;
        private Item item;
        private ModItem modItem;
        private Mod getMod;
        public void Init()
        {
            info = new TextLine[max];
            values = new int[max];
            for (int i = 0; i < max; i++)
                info[i] = new TextLine(i);
            text = new string[]
            {
                "Width",    "Height",
                "Damage",   "Knockback",
                "Use time", "Use animation", "Use style"
            };
            enter = new Button("Enter", left * 2 + width - "Enter".Length * Button.fontWidth, top - margin);
            itemName = new TextLine(-1, 350 - enter.box.Width);
            button = new Button[]
            {
                new Button("Apply", left, top + height),
                new Button("Clear", left + Button.fontWidth * "Apply".Length, top + height),
                new Button("Output", left + Button.fontWidth * ("Apply".Length + "Clear".Length), top + height),
                new Button("Get Item", left + Button.fontWidth * ("Apply".Length + "Clear".Length + "Output".Length), top + height)
            };
        }
        public override void PostUpdate()
        {
            if (KeyPress(Keys.F3))
                draw = !draw;
            if (update)
            {
                Main.chatRelease = true;
                Main.chatText = string.Empty;
                if (info == null)
                    Init();
                if (itemName.active)
                {
                    itemName.InputText();
                    if (KeyPress(Keys.Enter) || (HoverOver(enter.box) && LeftClick()))
                    {
                        foreach (string mod in ModLoader.GetLoadedMods())
                        {
                            getMod = ModLoader.GetMod(mod);
                            modItem = getMod.GetItem(itemName.input);
                            output = getMod.Name;
                            if (modItem != null)
                            {
                                item = modItem.item;
                                output += ", " + item.Name;
                                break;
                            }
                        }
                    }
                }
                for (int i = 0; i < max; i++)
                {
                    if (LeftClick())
                    {
                        if (HoverOver(itemName.box))
                        {
                            itemName.active = !itemName.active;
                            selected = -1;
                        }
                        if (HoverOver(info[i].box))
                        {
                            info[i].active = !info[i].active;
                            itemName.active = false;
                            selected = i;
                            break;
                        }
                    }
                    if (KeyPress(Keys.Tab))
                    {
                        if (selected++ < max - 1);
                        else selected = 0;
                        if (itemName.active)
                        {
                            selected = 0;
                            itemName.active = false;
                        }
                        info[selected].active = true;
                        break;
                    }
                    if (i != selected)
                        info[i].active = false;
                    if (info[i].active)
                        info[i].InputValues();
                    if (i < button.Length)
                    {
                        if (item == null)
                            continue;
                        if (HoverOver(button[i].box) && LeftClick())
                        {
                            switch (button[i].text)
                            {
                                case "Apply":
                                    for (int j = 0; j < max; j++)
                                        int.TryParse(info[j].input, out values[j]);
                                    item.width = values[0];
                                    item.height = values[1];
                                    item.damage = values[2];
                                    float.TryParse(info[3].input, out item.knockBack);
                                    item.useTime = values[4];
                                    item.useAnimation = values[5];
                                    item.useStyle = values[6];
                                    break;
                                case "Clear":
                                    foreach (TextLine line in info)
                                        line.input = string.Empty;
                                    break;
                                case "Output":
                                    info[0].input = item.width.ToString();
                                    info[1].input = item.height.ToString();
                                    info[2].input = item.damage.ToString();
                                    info[3].input = item.knockBack.ToString();
                                    info[4].input = item.useTime.ToString();
                                    info[5].input = item.useAnimation.ToString();
                                    info[6].input = item.useStyle.ToString();
                                    break;
                                case "Get Item":
                                    Main.item[0] = item;
                                    Main.item[0].stack = item.maxStack;
                                    Main.item[0].active = true;
                                    Main.item[0].position = player.Center;
                                    break;
                                case "Export":
                                    using (StreamWriter sw = new StreamWriter(ModLoader.ModPath + "\\" + getMod.Name + "\\" + modItem.Name + ".ini"))
                                        foreach (TextLine line in info)
                                            sw.Write(line.input + "\r\n");
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (info == null || itemName == null)
                    return;
                foreach (TextLine line in info.Where(t => t != null))
                    line.active = false;
                itemName.active = false;
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            SpriteBatch sb = Main.spriteBatch;
            if (update)
            {
                sb.Draw(texture, main, Color.CornflowerBlue * 0.25f);
                sb.DrawString(Main.fontMouseText, "Item Class Name", new Vector2(left + 10, top - margin), Color.White);
                sb.Draw(texture, itemName.box, itemName.color(itemName.active));
                sb.DrawString(Main.fontMouseText, itemName.input, itemName.box.TopLeft(), Color.White);
                if (item != null && modItem != null && !output.Contains("texture"))
                {
                    try
                    {
                        string textureName = modItem.Texture.Substring(modItem.Texture.IndexOf("/") + 1);
                        Texture2D itemTexture = getMod.GetTexture(textureName);
                        sb.Draw(itemTexture, new Vector2(left, top - itemTexture.Height - margin), Color.White);
                    }
                    catch
                    {
                        if (!output.Contains("texture"))
                            output += ", texture not found";
                        return;
                    }
                }
                for (int i = 0; i < max; i++)
                {
                    sb.DrawString(Main.fontMouseText, text[i], new Vector2(left + 10, top + i * margin), Color.White);
                    sb.Draw(texture, info[i].box, info[i].color(info[i].active));
                    sb.DrawString(Main.fontMouseText, info[i].input, info[i].box.TopLeft(), Color.White);
                    if (i < button.Length)
                    {
                        sb.Draw(texture, button[i].box, button[i].color(HoverOver(button[i].box)));
                        sb.DrawString(Main.fontMouseText, button[i].text, button[i].box.TopLeft(), Color.White);
                    }
                }
                sb.DrawString(Main.fontMouseText, output, new Vector2(left, height + top + margin), Color.White);
                sb.Draw(texture, enter.box, enter.color(HoverOver(enter.box)));
                sb.DrawString(Main.fontMouseText, enter.text, enter.box.TopLeft(), Color.White);
            }
        }
        public bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public bool HoverOver(Rectangle box)
        {
            return box.Contains(Main.MouseScreen.ToPoint());
        }
    }

    public class TextLine
    {
        public string input = string.Empty;
        public bool active;
        private float alpha = 0.50f;
        public const int
            height = 28;
        private int time;
        public Rectangle box;
        public TextLine(int index, int width = DrawUI.width - DrawUI.left)
        {
            box = textBox(index, width);
        }
        public Rectangle textBox(int index, int width = DrawUI.width - DrawUI.left)
        {
            return new Rectangle(DrawUI.left * 3, DrawUI.top + index * DrawUI.margin, width, height);
        }
        public Color color(bool active)
        {
            return Color.CornflowerBlue * (active ? 1f : alpha);
        }
        public void InputText()
        {
            if (Main.keyState.GetPressedKeys().Contains(Keys.Back))
            {
                if (time++ % 10 == 0)
                    input = input.Substring(0, Math.Max(input.Length - 1, 0));
                return;
            }
            if (input.Length > Button.fontWidth * DrawUI.width - DrawUI.left)
                return;
            time = 0;
            foreach (Keys key in Main.keyState.GetPressedKeys())
            {
                if (Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key))
                {
                    switch (key)
                    {
                        case Keys.Space:
                            input += " ";
                            return;
                        case Keys.Back:
                            continue;
                        case Keys.OemPeriod:
                            input += ".";
                            continue;
                        case Keys.LeftShift:
                            continue;
                        case Keys.RightShift:
                            continue;
                        case Keys.Enter:
                            continue;
                        case Keys.Escape:
                            continue;
                        case Keys.Tab:
                            continue;
                    }
                    if (key.ToString().StartsWith("D") && key.ToString().Length > 1)
                    {
                        input += key.ToString().Substring(1);
                        continue;
                    }
                    if (Main.keyState.GetPressedKeys().Contains(Keys.LeftShift) || Main.keyState.GetPressedKeys().Contains(Keys.RightShift))
                    {
                        if (key == Keys.OemMinus)
                        {
                            input += "_";
                            break;
                        }
                        input += key.ToString();
                        break;
                    }
                    else
                    {
                        input += key.ToString().ToLower();
                        break;
                    }
                }
            }
        }
        public void InputValues()
        {
            foreach (Keys key in Main.keyState.GetPressedKeys())
            {
                if (Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key))
                {
                    switch (key)
                    {
                        case Keys.Space:
                            input += " ";
                            continue;
                        case Keys.Back:
                            input = input.Substring(0, Math.Max(input.Length - 1, 0));
                            continue;
                        case Keys.OemPeriod:
                            input += ".";
                            continue;
                    }
                    if (key.ToString().StartsWith("D"))
                    {
                        input += key.ToString().Substring(1);
                        continue;
                    }
                }
            }
        }
    }

    public class Button
    {
        public string text;
        public bool active;
        public const int fontWidth = 11;
        public Rectangle box;
        public Color color(bool active)
        {
            return active ? Color.IndianRed : Color.CornflowerBlue;
        }
        public Button(string text, int x, int y)
        {
            this.text = text;
            box = new Rectangle(x, y, fontWidth * text.Length, TextLine.height);
        }
    }
}
