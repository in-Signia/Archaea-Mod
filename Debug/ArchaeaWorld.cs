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
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using ReLogic.Graphics;

namespace ArchaeaMod_Debug
{
    public class ArchaeaWorld : ModWorld
    {
        public static List<Vector2> origins = new List<Vector2>();
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(pass => pass.Name.Equals("Granite"));
            tasks.Insert(index, new PassLegacy("Cave Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                var cave = new MagnoCave();
                progress.Message = "Underground Magno";
                cave.Initialize(TileID.PearlstoneBrick, WallID.CaveUnsafe);
                for (int i = 0; i < cave.diggers.Length; i++)
                {
                    Vector2 center = MagnoCave.Center();
                    origins.Add(center);
                    cave.diggers[i].DigSequence(center);
                    progress.Value += cave.rand * 0.1f / cave.rand;
                }
                cave = null;
                progress.End();
            }));
            tasks.Insert(index + 1, new PassLegacy("Den Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "Magno Cavern";
                var den = new MagnoDen();
                den.Initialize();
                while (MagnoDen.active)
                {
                    progress.Value = (float)MagnoDen.whoAmI / MagnoDen.max;
                    MagnoDen.mDen[MagnoDen.whoAmI].Update();
                }
                den = null;
                progress.End();
            }));
            #region Dec. 27th PM Debug
            /*tasks.Insert(index + 2, new PassLegacy("Structure Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "More Magno";
                var s = new Structures();
                s.Initialize();
                Vector2[] regions = Treasures.GetRegion(MagnoDen.bounds.Center.ToVector2(), MagnoDen.bounds.Width / 2, false, true, TileID.PearlstoneBrick);
                int total = regions.Length;
                int max = WorldGen.genRand.Next(5, 8);
                int count = (int)Math.Sqrt(total);
                for (int i = 0; i < total; i += count)
                {
                    s.MagnoHouse(regions[WorldGen.genRand.Next(total / count)]);
                    progress.Value = (float)i / total;
                }
                s = null;
                progress.Value = 1f;
                progress.End();
            })); */
            #endregion
            tasks.Insert(index + 2, new PassLegacy("Mod Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "Magno extras";
                var t = new Treasures();
                t.Initialize(50, TileID.Containers, TileID.PearlstoneBrick, WallID.CaveUnsafe);
                t.PlaceChests(10, 15);
                origins.Clear();
                progress.Value = 0.5f;
                int radius = MagnoDen.bounds.Width / 2;
                int total = (int)Math.Pow(radius * 2, 2);
                Vector2[] regions = Treasures.GetWall(MagnoDen.bounds.Center.ToVector2(), radius, false, true, TileID.PearlstoneBrick);
                t.PlaceTile(regions, total, 10, TileID.Torches, true, true, true, total / (int)Math.Sqrt(radius));
                t = null;
                progress.Value = 1f;
                progress.End();
            }));
            int index2 = tasks.FindIndex(pass => pass.Name.Contains("House"));
            tasks.Insert(index2, new PassLegacy("Structure Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "More Magno";
                var s = new Structures.Magno();
                s.Initialize();
                Vector2[] regions = Treasures.GetRegion(MagnoDen.bounds.Center.ToVector2(), MagnoDen.bounds.Width / 2, false, true, TileID.PearlstoneBrick);
                int count = 0;
                int total = (int)Math.Sqrt(regions.Length);
                int max = WorldGen.genRand.Next(5, 8);
                for (int i = 0; i < max; i++)
                {
                    while (!s.MagnoHouse(regions[WorldGen.genRand.Next(regions.Length)]))
                    {
                        if (count < total)
                            count++;
                        else break;
                    }
                    count = 0;
                    progress.Value = (float)i / max;
                }
                s = null;
                progress.Value = 1f;
                progress.End();
            }));
        }

        public static string[] debug = new string[10];
        public override void PostDrawTiles()
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int i = 0; i < debug.Length; i++)
                if (debug[i] != null)
                    sb.DrawString(Main.fontMouseText, debug[i], new Vector2(400, 80 + 16 * i), Color.White);
            sb.End();
        }

        private static int index2;
        public Vector2 ForeachIndex(Vector2[] regions)
        {
            if (index2 < regions.Length)
                index2++;
            else index2 = 0;
            return regions[index2];
        }

        private bool begin;
        private bool q;
        private Structures s;
        private Treasures t;
        public override void PreUpdate()
        {
            if (!begin)
            {
                //s = new Structures(new Vector2(Main.spawnTileX, Main.spawnTileY - 80));
                //t = new Treasures();
                begin = true;
            }
            if (ArchaeaPlayer.KeyPress(Keys.Q))
            {
                //s.Initialize();
                //s.SkyRoom();
                //  Main.NewText("Center: " + MagnoDen.bounds.Center.ToVector2().ToString() + " Width: " + MagnoDen.bounds.Width);
                //s.Initialize();
                //t.PlaceTile(regions, 50, 3, TileID.Torches, 10, false, true, true, true);
                //foreach (Vector2 region in regions)
                //  s.MagnoHouse(region);
                //s.Reset();
            }
        }

        public static void TextOutput(Vector2[] regions, string name, bool append = false, string text = "", string text2 = "")
        {
            List<Vector2[]> list = new List<Vector2[]>();
            list.Add(regions);
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Norm\Documents\My Games\Terraria\ModLoader\Mod Sources\ArchaeaMod\" + name + ".txt", append))
            {
                sw.WriteLine(text);
                sw.WriteLine(text2);
                for (int i = 0; i < list.Count; i++)
                    foreach (Vector2 v2 in list[i])
                        if (v2 != Vector2.Zero)
                            sw.WriteLine("Index: " + i + " " + v2.ToString());
            }
        }
        public static void TextOutput(int[,] array, string name, bool append = false, string text = "", string text2 = "")
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Norm\Documents\My Games\Terraria\ModLoader\Mod Sources\ArchaeaMod\" + name + ".txt", append))
            {
                sw.Write(text + sw.NewLine);
                sw.WriteLine(text2 + sw.NewLine);
                for (int i = 0; i < array.GetLength(1); i++)
                    for (int j = 0; j < array.GetLength(0); j++)
                    {
                        string newLine = j == 0 ? sw.NewLine : string.Empty;
                        sw.Write(newLine + array[j, i]);
                    }
            }
        }
    }
    
    public class MagnoCave
    {
        private static int x
        {
            get { return WorldGen.genRand.Next(100, (int)Main.rightWorld / 16 - 100); }
        }
        private static int y
        {
            get { return WorldGen.genRand.Next((int)Main.worldSurface / 16 + 300, (int)Main.bottomWorld / 16 - 250); }
        }
        public int rand;
        public float progress;
        public Digger[] diggers;
        public static Vector2 Center()
        {
            return new Vector2(x, y);
        }
        public void Initialize(ushort tileID, ushort wallID)
        {
            rand = WorldGen.genRand.Next(7, 12);
            diggers = new Digger[rand];
            for (int i = 0; i < rand; i++)
                diggers[i] = new Digger(25, tileID, wallID);
        }
    }
    public class MagnoDen
    {
        public static bool active = true;
        public bool complete;
        private int width
        {
            get { return (int)Main.rightWorld / 16 / 3; }
        }
        private int leftBounds
        {
            get { return WorldGen.genRand.Next(width, width * 2); }
        }
        private int maxY
        {
            get { return (int)Main.bottomWorld / 16 - 200; }
        }
        private int centerX
        {
            get { return leftBounds + width / 2; }
        }
        private int centerY
        {
            get { return (int)(Main.bottomWorld / 16 / 2f); }
        }
        private int lookFurther;
        public static int whoAmI;
        public static readonly int max = 200;
        private readonly int border = 2;
        private readonly int cave = 2;
        private int x
        {
            get { return (int)center.X; }
        }
        private int y
        {
            get { return (int)center.Y; }
        }
        private int points;
        private int cycle;
        private int id;
        private int X = Main.maxTilesX;
        private int Y = Main.maxTilesY;
        private int Width;
        private int Height;
        public Vector2 center;
        public static Vector2 origin
        {
            get { return new Vector2(bounds.X + bounds.Width / 2, bounds.X + bounds.Height / 2); }
        }
        public static Rectangle bounds;
        public static Dictionary<Vector2, int> plots = new Dictionary<Vector2, int>();
        public static MagnoDen[] mDen;
        public void Initialize()
        {
            active = true;
            mDen = new MagnoDen[max];
            center = new Vector2(centerX / 2, centerY);
            //  bounds = new Rectangle(leftBounds, centerY, leftBounds + width / 3, centerY + width / 4);
            mDen[whoAmI] = new MagnoDen();
            mDen[whoAmI].id = whoAmI;
            mDen[whoAmI].center = center;
        }
        public void Start(Vector2 position)
        {
            center = position;
        }
        public void Update()
        {
            if (complete)
                return;
            CheckComplete();
            while (!StandardMove());
            lookFurther = 0;
            points = 0;
            if (center.Y > maxY)
            {
                int block = -200;
                bool bRand = WorldGen.genRand.Next(2) == 0;
                center += new Vector2(bRand ? block / 4: block * -1 / 4 , block);
            }
            GetBounds();
            //  GenerateNewMiner();
        }
        public bool StandardMove()
        {
            int size = WorldGen.genRand.Next(1, 4);
            int rand = WorldGen.genRand.Next(1, 5);
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x + 1 + lookFurther, y].active())
            {
                center.X += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x - 1 - lookFurther, y].active())
            {
                center.X -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y + 1 + lookFurther].active())
            {
                center.Y += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y - 1 - lookFurther].active())
            {
                center.Y -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (!Main.tile[x + 1 + lookFurther, y].active() &&
                !Main.tile[x - 1 - lookFurther, y].active() &&
                !Main.tile[x, y + 1 + lookFurther].active() &&
                !Main.tile[x, y - 1 - lookFurther].active())
                lookFurther++;
            if (!plots.ContainsKey(center))
                plots.Add(center, size);
            if (points > 10)
                return true;
            return false;
        }
        public void VerticalMove()
        {
            Vector2 old = center;
            while (center == old)
            {
                int rand = WorldGen.genRand.Next(1, 5);
                int x = (int)center.X;
                int y = (int)center.Y;
                switch (rand)
                {
                    case 1:
                        do
                        {
                            center.X += 1f;
                            lookFurther++;
                        } while (!Main.tile[x + 1 + lookFurther, y].active()
                                && x < Main.rightWorld / 16);
                        break;
                    case 2:
                        do
                        {
                            center.X -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x - 1 - lookFurther, y].active()
                                && x > 50);
                        break;
                    case 3:
                        do
                        {
                            center.Y += 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y + 1 + lookFurther].active()
                                && y < Main.bottomWorld / 16);
                        break;
                    case 4:
                        do
                        {
                            center.Y -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y - 1 - lookFurther].active()
                                && y > maxY);
                        break;
                    default:
                        break;
                }
                if (lookFurther % 2 == 0)
                    PlaceWater(center);
                lookFurther = 0;
            }
        }
        public bool AverageMove()
        {
            Vector2 old = center;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X -= 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y -= 1f;
            return center != old;
        }
        public MagnoDen GenerateNewMiner()
        {
            if (this == mDen[0])
            {
                whoAmI++;
                if (whoAmI == max)
                    FinalDig();
                if (whoAmI < max)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                }
                else
                    Terminate();
            }
            return mDen[Math.Min(whoAmI, max - 1)];
        }
        public Vector2 NewPosition(Vector2 previous)
        {
            return new Vector2(previous.X, origin.Y);
        }
        public static bool Inbounds(int x, int y)
        {
            return x < Main.maxTilesX - 50 && x > 50 && y < Main.maxTilesY - 200 && y > 50;
        }
        public void DigPlot(int size)
        {
            for (int i = (int)center.X - size; i < (int)center.X + size; i++)
                for (int j = (int)center.Y - size; j < (int)center.Y + size; j++)
                {
                    if (Inbounds(i, j))
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, false, true);
                    }
                }
        }
        public void FinalDig()
        {
            var v2 = plots.Keys.ToArray();
            var s = plots.Values.ToArray();
            for (int k = 1; k < v2.Length; k++)
            {
                int x = (int)v2[k].X;
                int y = (int)v2[k].Y;
                for (int i = x - s[k] * border; i < x + s[k] * border; i++)
                {
                    for (int j = y - s[k] * border; j < y + s[k] * border; j++)
                    {
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, true, true);
                        //  WorldGen.KillWall(i, j);
                    }
                }
            }
            for (int l = 1; l < v2.Length; l++)
            {
                int x = (int)v2[l].X;
                int y = (int)v2[l].Y;
                for (int i = (int)x - s[l]; i < (int)x + s[l]; i++)
                    for (int j = (int)y - s[l]; j < (int)y + s[l]; j++)
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = 0;
                        Main.tile[i, j].active(false);
                        //  WorldGen.KillTile(i, j, false, false, true);
                    }
            }
        }
        public void PlaceWater(Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (Inbounds(x, y))
                Main.tile[x, y].liquid = 60;
        }
        public void CheckComplete()
        {
            cycle++;
            if (cycle == max / 2)
            {
                whoAmI++;
                if (whoAmI == max)
                    FinalDig();
                if (whoAmI < max)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].id = whoAmI;
                    mDen[whoAmI].center = mDen[whoAmI - 1].center;
                }
                else
                    Terminate();
                complete = true;
            }
        }
        internal void GetBounds()
        {
            int count = 0;
            var a = new Vector2[mDen.Length];
            foreach (MagnoDen m in mDen)
                if (m != null && m.center != Vector2.Zero)
                {
                    if (m.center.X < X)
                        X = (int)m.center.X;
                    if (m.center.Y < Y)
                        Y = (int)m.center.Y;
                    if (m.center.X - X > Width)
                        Width = (int)m.center.X - X;
                    if (m.center.Y - Y > Height)
                        Height = (int)m.center.Y - Y;
                    a[count] = m.center;
                    if (m.id == mDen.Length - 1)
                    {
                        bounds = new Rectangle(X, Y, Width, Height);
                        break;
                    }
                    count++;
                }
        }
        public void Terminate()
        {
            Unload();
            active = false;
            for (int i = 0; i < mDen.Length; i++)
                mDen[i] = null;
        }
        protected void Unload()
        {
            whoAmI = 0;
            plots.Clear();
        }
    }
    public class Treasures
    {
        public int offset;
        private ushort floorID;
        private ushort newTileID;
        private ushort wallID;
        private List<Vector2> list;
        public void Initialize(int offset, ushort newTileID, ushort floorID, ushort wallID)
        {
            this.offset = offset;
            this.newTileID = newTileID;
            this.floorID = floorID;
            this.wallID = wallID;
            list = ArchaeaWorld.origins;
        }
        public void PlaceChests(int total, int retries)
        {
            int index = 1;
            int count = 0;
            int loop = 0;
            var getFloor = GetFloor();
            int length = list.Count;
            bool[] added = new bool[length];
            while (count < total)
            {
                if (loop < total * retries)
                    loop++;
                else
                {
                    index++;
                    loop = 0;
                }
                foreach (Vector2 ground in getFloor[index - 1])
                {
                    int x = (int)ground.X;
                    int y = (int)ground.Y;
                    if (!MagnoDen.Inbounds(x, y)) continue;
                    if (Main.tile[x, y].wall == wallID && WorldGen.genRand.Next(8) == 0)
                        WorldGen.PlaceTile(x, y, newTileID, true, true);
                    if (Main.tile[x, y].type == newTileID)
                    {
                        added[index] = true;
                        count++;
                        break;
                    }
                }
                if (added[index])
                {
                    index++;
                    loop = 0;
                }
                if (index == length)
                    break;
            }
        }
        public void PlaceTile(Vector2[] region, int total, int retries, ushort newTileID, bool genPlace = true, bool force = false, bool random = false, int odds = 5, bool proximity = false, int radius = 30, bool iterate = false, bool onlyOnWall = false)
        {
            int loop = 0;
            int index = 0;
            var getFloor = region;
            while (index < getFloor.Length)
            {
                if (loop < total * retries)
                    loop++;
                else break;
                if (getFloor[index] == Vector2.Zero)
                {
                    index++;
                    continue;
                }
                int x = (int)getFloor[index].X;
                int y = (int)getFloor[index].Y;
                Tile tile = Main.tile[x, y];
                if (random && WorldGen.genRand.Next(odds) != 0) continue;
                if (onlyOnWall && Main.tile[x, y].wall != wallID) continue;
                if (proximity && Vicinity(getFloor[index], radius, newTileID)) continue;
                if (genPlace)
                    WorldGen.PlaceTile(x, y, newTileID, true, force);
                else
                {
                    tile.active(true);
                    tile.type = newTileID;
                }
                if (total == 1 && tile.type == newTileID && tile.active())
                    break;
                index++;
                if (iterate && index == getFloor.Length - 1)
                    index = 0;
            }
        }
        public Vector2[][] GetFloor()
        {
            int index = 0;
            int count = 0;
            int length = list.Count;
            var tiles = new Vector2[length][];
            for (int k = 0; k < length; k++)
                tiles[k] = new Vector2[length * length];
            foreach (Vector2 v2 in list)
            {
                for (int i = (int)v2.X - offset; i < (int)v2.X + offset; i++)
                    for (int j = (int)v2.Y - offset; j < (int)v2.Y + offset; j++)
                    {
                        Tile floor = Main.tile[i, j];
                        Tile ground = Main.tile[i, j + 1];
                        if ((!floor.active() || !Main.tileSolid[floor.type]) &&
                            ground.active() && Main.tileSolid[ground.type] && ground.type == floorID)
                        {
                            if (count < tiles[index].Length)
                            {
                                tiles[index][count] = new Vector2(i, j);
                                count++;
                            }
                        }
                    }  
                count = 0;
                if (index < length)
                    index++;
                else
                    break;
            }
            return tiles;
        }
        public static Vector2[] GetFloor(Vector2 region, int radius, bool overflow = false, ushort[] floorIDs = null)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int amount = (int)Math.Sqrt(index) / 10;
            int count = 0;
            var tiles = new Vector2[index];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile floor = Main.tile[i, j];
                    Tile ground = Main.tile[i, j + 1];
                    if (floor.active() && Main.tileSolid[floor.type]) continue;
                    foreach (ushort floorType in floorIDs)
                        if (ground.active() && Main.tileSolid[ground.type] && ground.type == floorType)
                        {
                            if (count < tiles.Length)
                            {
                                tiles[count] = new Vector2(i, j);
                                count++;
                            }
                        }
                }
            return tiles;
        }
        public static Vector2[] GetCeiling(Vector2 region, int radius, bool overflow = false, ushort tileType = 0)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int count = 0;
            var tiles = new Vector2[index];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile roof = Main.tile[i, j];
                    Tile ceiling = Main.tile[i, j + 1];
                    if (ceiling.active() && Main.tileSolid[ceiling.type]) continue;
                    if (roof.active() && Main.tileSolid[roof.type] && roof.type == tileType)
                    {
                        if (count < tiles.Length)
                        {
                            tiles[count] = new Vector2(i, j);
                            count++;
                        }
                    }
                }
            return tiles;
        }
        public static Vector2[] GetRegion(Vector2 region, int radius, bool overflow = false, bool attach = false, ushort tileType = 0)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int count = 0;
            var tiles = new Vector2[index];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (count >= tiles.Length) continue;
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (attach && Main.tile[i, j].type != tileType) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    tiles[count] = new Vector2(i, j);
                    count++;
                }
            return tiles;
        }
        public static Vector2[] GetWall(Vector2 region, int radius, bool overflow = false, bool attach = false, ushort wallTileType = 0)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int count = 0;
            var tiles = new Vector2[index];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (count >= tiles.Length) continue;
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile tile = Main.tile[i, j];
                    Tile wallL = Main.tile[i - 1, j];
                    Tile wallR = Main.tile[i + 1, j];
                    bool left = wallL.active() && Main.tileSolid[wallL.type];
                    bool right = wallR.active() && Main.tileSolid[wallR.type];
                    ArchaeaWorld.debug[0] = "Wall left: " + left;
                    ArchaeaWorld.debug[1] = "Wall right: " + right;
                    if (attach)
                    {
                        if (left && wallL.type != wallTileType) continue;
                        if (right && wallR.type != wallTileType) continue;
                        ArchaeaWorld.debug[2] = "Attach successful";
                    }
                    if (!tile.active() || !Main.tileSolid[tile.type])
                        if (left || right)
                            tiles[count] = new Vector2(i, j);
                            count++;
                }
            ArchaeaWorld.debug[3] = "Count: " + count;
            ArchaeaWorld.debug[4] = tiles[0].ToString();
            return tiles;
        }
        public static bool Vicinity(Vector2 region, int radius, ushort tileType)
        {
            int x = (int)region.X;
            int y = (int)region.Y;
            for (int i = x - radius; i < x + radius; i++)
                for (int j = y - radius; j < y + radius; j++)
                {
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (Main.tile[i, j].type == tileType && Main.tile[i, j].active())
                        return true;
                }
            return false;
        }
        public static int ProximityCount(Vector2 region, int radius, ushort tileType)
        {
            int x = (int)region.X;
            int y = (int)region.Y;
            int count = 0;
            for (int i = x - radius; i < x + radius; i++)
                for (int j = y - radius; j < y + radius; j++)
                {
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    Tile tile = Main.tile[i, j];
                    if (tile.type == tileType)
                        count++;
                }
            return count;
        }
        #region
        public static Vector2[] GetFloor(Vector2 region, int radius, ushort floorType)
        {
            int count = 0;
            var tiles = new Vector2[radius * 2 * 2];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    Tile floor = Main.tile[i, j];
                    Tile ground = Main.tile[i, j + 1];
                    if ((!floor.active() || !Main.tileSolid[floor.type]) &&
                        ground.active() && Main.tileSolid[ground.type] && ground.type == floorType)
                    {
                        if (count < tiles.Length)
                        {
                            tiles[count] = new Vector2(i, j);
                            count++;
                        }
                    }
                }
            return tiles;
        }
        #endregion
        /*  Prior GetWall version
        public static Vector2[] GetWall(Vector2 region, int radius, bool overflow = true, bool attach = false, ushort wallTileType = 0)
        {
            int index = (int)Math.Pow(radius * 2, 2);
            int count = 0;
            var tiles = new Vector2[radius * 2 * radius];
            for (int i = (int)region.X - radius; i < (int)region.X + radius; i++)
                for (int j = (int)region.Y - radius; j < (int)region.Y + radius; j++)
                {
                    if (count >= tiles.Length) continue;
                    if (!MagnoDen.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile tile = Main.tile[i, j];
                    Tile wallL = Main.tile[i + 1, j];
                    Tile wallR = Main.tile[i - 1, j];
                    if ((tile.active() && !Main.tileSolid[tile.type]) || !tile.active()) continue;
                    if ((wallL.active() && Main.tileSolid[wallL.type]) ||
                        (wallR.active() && Main.tileSolid[wallR.type]))
                        if (count < tiles.Length)
                        {
                            tiles[count] = new Vector2(i, j);
                            count++;
                        }
                }
            return tiles;
        }
        */
    }
    public class Structures
    {
        internal bool[] direction;
        public static int index;
        private int count;
        private int max = 3;
        public int[][,] house;
        public int[,] rooms;
        private ushort tileID;
        private ushort wallID;
        private Vector2 origin;
        private ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
        private ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
        private ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
        class ID
        {
            public const int
                Empty = 0,
                Wall = 1,
                Platform = 2,
                Stairs = 3,
                Floor = 4,
                Door = 5,
                Decoration = 6,
                Furniture = 7,
                Useful = 8,
                Lamp = 9,
                Chest = 10,
                Cloud = 11,
                Trap = 12,
                Danger = 13,
                Wire = 14;
        }
        class RoomID
        {
            public const int
                Empty = 0,
                FilledIn = 1,
                Safe = 2,
                Danger = 3,
                Chest = 4,
                Platform = 5;
        }
        public Structures(Vector2 origin = default(Vector2), ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
        {
            this.tileID = tileID;
            this.wallID = wallID;
            this.origin = origin;
        }
        public void Initialize()
        {
            int x;
            int y;
            int radius = 105;
            int lX = radius - 45;
            int lY = radius - 15;
            int lengthX;
            int lengthY;
            int roomX = 15;
            int roomY = 9;
            house = new int[max][,];
            for (int k = 0; k < max; k++)
            {
                switch (k)
                {
                    case -2:
                        int index = 1;
                        for (int i = 0; i < rooms.GetLength(0); i++)
                            for (int j = 0; j < rooms.GetLength(1); j++)
                            {
                                int m = i * roomX;
                                int n = j * roomY;
                                int width = m + roomX;
                                int height = n + roomY;
                                int added = 0;
                                int floor;
                                bool placed;
                                for (int q = m; q < width; q++)
                                    for (int r = n; r < height; r++)
                                        switch (rooms[i, j])
                                        {
                                            case -2:
                                                int door = 0;
                                                if (i != 0 && q == m)
                                                {
                                                    if (j != rooms.GetLength(1) - 1 && r >= height - 3)
                                                        house[index][q, r] = ID.Empty;
                                                    else if (j == rooms.GetLength(1) - 1)
                                                    {
                                                        door = 1;
                                                        if (r >= height - 4 && r < height - 1)
                                                            house[index][q, r] = ID.Empty;
                                                    }
                                                    else door = 0;
                                                    if (r >= height - 3 - door && r <= height - 1 - door)
                                                        house[index][q, r] = ID.Door;
                                                }
                                                break;
                                            case -1:
                                                if ((i == 0 && q == m) || (i == rooms.GetLength(0) - 1 && q == width - 1))
                                                    house[index][q, r] = ID.Wall;
                                                if (WorldGen.genRand.Next(4) == 0)
                                                    if (rooms[Math.Max(i - 1, 0), j] != RoomID.FilledIn)
                                                        if (i != 0 && q == m)
                                                        {
                                                            house[index][q, r] = ID.Wall;
                                                            goto case -2;
                                                        }
                                                break;
                                            case RoomID.Empty:
                                                goto case -1;
                                            case RoomID.FilledIn:
                                                house[index][q, r] = ID.Wall;
                                                break;
                                            case RoomID.Platform:
                                                if (q > m + 5 && q < m + 10 && r == n)
                                                    house[index][q, r] = ID.Platform;
                                                goto case -1;
                                            case RoomID.Safe:
                                                added = 0;
                                                bool useful = false;
                                                bool lamp = false;
                                                if (j == rooms.GetLength(1) - 1)
                                                    floor = 2;
                                                else floor = 1;
                                                if (q == m + 1 && r == height - floor)
                                                {
                                                    while (added < 5)
                                                    {
                                                        int ground = q + WorldGen.genRand.Next(1, 11);
                                                        if (ground % 2 == 0)
                                                        {
                                                            if (!useful)
                                                            {
                                                                house[index][ground, r] = ID.Useful;
                                                                useful = true;
                                                            }
                                                            house[index][ground, r] = WorldGen.genRand.Next(new int[] { ID.Decoration, ID.Furniture });
                                                            added++;
                                                        }
                                                    }
                                                }
                                                if (q == m && r == n + 1)
                                                {
                                                    if (!lamp)
                                                    {
                                                        int roof = q + WorldGen.genRand.Next(2, 10);
                                                        house[index][roof, r] = ID.Lamp;
                                                        lamp = true;
                                                    }
                                                }
                                                goto case -1;
                                            case RoomID.Chest:
                                                added = 0;
                                                placed = false;
                                                if (j == rooms.GetLength(1) - 1)
                                                    floor = 2;
                                                else floor = 1;
                                                if (q == m + 3 && q < width - 3 && r == height - floor)
                                                {
                                                    while (added < 4)
                                                    {
                                                        int ground = q + WorldGen.genRand.Next(1, 11);
                                                        if (ground % 2 == 1)
                                                        {
                                                            if (!placed)
                                                            {
                                                                house[index][ground, r] = ID.Chest;
                                                                placed = true;
                                                            }
                                                            house[index][ground, r] = ID.Decoration;
                                                            added++;
                                                        }
                                                    }
                                                }
                                                goto case -1;
                                            case RoomID.Danger:
                                                goto case -1;
                                        }
                            }
                        #region Designate
                        /*
                        index = 1;
                        lengthX = house[index].GetLength(0);
                        lengthY = house[index].GetLength(1);
                        for (int i = 0; i < lengthX - roomX; i += roomX)
                            for (int j = 0; j < lengthY - roomY; j += roomY)
                                for (int o = 0; o < roomX; o++)
                                    for (int p = 0; p < roomY; p++)
                                    {
                                        x = i + o;
                                        y = j + p;
                                        switch (rooms[i / roomX, j / roomY])
                                        {
                                            case RoomID.FilledIn:
                                                house[index][x, y] = ID.Wall;
                                                break;
                                            case RoomID.Platform:
                                                if (o > 6 && o < 12 && p == 0)
                                                    house[index][x, y] = ID.Platform;
                                                break;
                                        }
                                    } */
                        #endregion
                        break;
                    case -1:
                        rooms = new int[lX / roomX, lY / roomY];
                        int tries = 0;
                        while (true)
                        {
                            for (int i = 0; i < rooms.GetLength(0); i++)
                                for (int j = 0; j < rooms.GetLength(1); j++)
                                    rooms[i, j] = RoomID.FilledIn;
                            int length = rooms.GetLength(0);
                            bool flip = false;
                            bool b = false;
                            int m = 0;
                            for (int j = rooms.GetLength(1) - 1; j >= 0; j--)
                            {
                                b = false;
                                for (int i = m; flip ? i >= -1 : i <= rooms.GetLength(0); i -= flip ? 1 : -1)
                                {
                                    bool rand = WorldGen.genRand.Next(6) == 0;
                                    if (!flip)
                                    {
                                        if (i == length || rand)
                                        {
                                            m = Math.Min(i, length - 1);
                                            rooms[m, j] = RoomID.Platform;
                                            b = true;
                                            break;
                                        }
                                        rooms[i, j] = RoomID.Empty;
                                    }
                                    else
                                    {
                                        if (i < 0 || rand)
                                        {
                                            m = Math.Max(i, 0);
                                            rooms[m, j] = RoomID.Platform;
                                            b = true;
                                            break;
                                        }
                                        rooms[i, j] = RoomID.Empty;
                                    }
                                }
                                flip = !flip;
                            }
                            tries++;
                            int count = 0;
                            int chest = 0;
                            int filledIn = 0;
                            for (int j = rooms.GetLength(1) - 1; j >= 0; j--)
                                for (int i = 0; i < rooms.GetLength(0); i++)
                                {
                                    if (rooms[i, j] == RoomID.FilledIn)
                                        filledIn++;
                                    if (j > 0 && rooms[i, j] == RoomID.Platform && (rooms[i, j - 1] == RoomID.Empty || rooms[i, j - 1] == RoomID.Platform))
                                    {
                                        count++;
                                        if (count == rooms.GetLength(1) - 1 && filledIn >= Math.Max(12, (rooms.GetLength(0) * rooms.GetLength(1) / 2) - tries))
                                            goto case -2;
                                    }
                                    int randRoom = WorldGen.genRand.Next(2, 5);
                                    if (randRoom == RoomID.Chest)
                                        chest++;
                                    if (rooms[i, j] != RoomID.FilledIn && rooms[i, j] != RoomID.Platform)
                                    {
                                        rooms[i, j] = randRoom;
                                        if (randRoom == RoomID.Chest && chest > 3)
                                            rooms[i, j] = WorldGen.genRand.Next(2, 4);
                                    }
                                }
                        }
                        #region Depracated Rooms ID
                        /*
                        int roomsX = rooms.GetLength(0);
                        int roomsY = rooms.GetLength(1);
                        for (int i = 0; i < roomsX; i++)
                            for (int j = 0; j < roomsY; j++)
                                rooms[i, j] = RoomID.FilledIn;
                        bool flip = false;
                        rooms[0, 0] = 0;
                        int m = -1;
                        int n = 0;
                        while (n < roomsY - 1)
                        {
                            if (!flip)
                            {
                                m++;
                                if (m == roomsX || WorldGen.genRand.Next(10) == 0)
                                {
                                    n++;
                                    int min = Math.Min(m, roomsX - 1);
                                    if (n < roomsY - 1)
                                    {
                                        rooms[min, n] = RoomID.Platform;
                                        if (rooms[min, n - 1] != RoomID.Platform && rooms[min, Math.Max(n - 2, 0)] == RoomID.Platform)
                                            rooms[min, n - 1] = RoomID.Platform;
                                        if (rooms[min, n - 1] == RoomID.FilledIn)
                                            rooms[min, n - 1] = RoomID.Empty;
                                        m--;
                                        flip = true;
                                    }
                                }
                                rooms[m, n] = RoomID.Empty;
                            }
                            if (n == roomsY - 1)
                                break;
                            if (flip)
                            {
                                rooms[Math.Max(0, m - 1), n] = RoomID.Empty;
                                m--;
                                if (m < 0 || WorldGen.genRand.Next(10) == 0)
                                {
                                    n++;
                                    int _max = Math.Max(0, m);
                                    if (n == roomsY - 1)
                                    {
                                        rooms[_max, n] = RoomID.Platform;
                                        if (rooms[_max, n - 1] != RoomID.Platform && rooms[_max, Math.Max(n - 2, 0)] == RoomID.Platform)
                                            rooms[_max, n - 1] = RoomID.Platform;
                                        if (rooms[_max, n - 1] == RoomID.FilledIn)
                                            rooms[_max, n - 1] = RoomID.Empty;
                                        flip = false;
                                    }
                                }
                            }
                        }
                        */
                        #endregion
                    case 0:
                        for (int i = -3; i <= 3; i++)
                        {
                            x = (int)origin.X + 15 * i;
                            y = (int)origin.Y;
                            //  CloudForm(x, y, 20);
                        }
                        break;
                    case 1:
                        house[k] = new int[lX, lY];
                        lengthX = house[k].GetLength(0);
                        lengthY = house[k].GetLength(1);
                        for (int i = 0; i < lengthX; i++)
                            for (int j = 0; j < lengthY; j++)
                            {
                                if (j == 0 || j == lengthY - 1 || j % roomY == 0)
                                    house[k][i, j] = ID.Floor;
                                else house[k][i, j] = ID.Empty;
                            }
                        goto case -1;
                    case 2:
                        house[k] = new int[30, radius / 2];
                        lengthX = house[k].GetLength(0);
                        lengthY = house[k].GetLength(1);
                        for (int i = 0; i < lengthX; i++)
                            for (int j = 0; j < lengthY; j++)
                            {
                                if (i == 0 || i == lengthX - 1 || j == 0 || j == lengthY - 1 || (j % 10 == 0 && j != radius))
                                {
                                    house[k][i, j] = ID.Wall;
                                    if (i < 10 || i > 20)
                                        house[k][i, j] = ID.Wall;
                                    else
                                        house[k][i, j] = ID.Platform;
                                }
                                else house[k][i, j] = ID.Empty;
                            }
                        break;
                }
            }
            ArchaeaWorld.TextOutput(rooms, "FortArrays", false, "Room array:");
        }
        public void CloudForm(int i, int j, float width = 30, int odds = 10)
        {
            Vector2 origin = new Vector2(i, j);
            float radian = (float)Math.PI / 180f;
            float degrees = 0f;
            for (degrees = 0f; degrees < Math.PI * 2d; degrees += radian)
            {
                for (int r = 0; r < width; r++)
                {
                    int cos = (int)(origin.X + (r * Math.Cos(degrees)));
                    int sine = (int)(origin.Y + (r / 2 * Math.Sin(degrees)));
                    ushort tileType = WorldGen.genRand.NextFloat() <= 34f ? TileID.Dirt : TileID.Cloud;
                    if (r >= 0.75f && WorldGen.genRand.Next(odds) == 0)
                        WorldGen.PlaceTile(cos, sine, TileID.Cloud, true, true);
                    cos = (int)(origin.X + (r * 1.5f * Math.Cos(degrees)));
                    sine = (int)(origin.Y + (r / 1.5f * Math.Sin(degrees)));
                    if (r <= width * 0.34f)
                        WorldGen.PlaceTile(cos, sine, tileType, true, true);
                    if (r > width * 0.34f && r < width * 0.75f)
                        WorldGen.PlaceTile(cos, sine, tileType, true, true);
                }
            }
        }
        public void SkyRoom()
        {
            int m;
            int n;
            for (int k = 1; k < max; k++)
            {
                int width = house[1].GetLength(0);
                int lengthX = house[k].GetLength(0);
                int lengthY = house[k].GetLength(1);
                ushort type;
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = (int)origin.X + i + (k == 2 ? width : 0);
                        n = (int)origin.Y - lengthY - 1 + j;
                        WorldGen.KillTile(m, n, false, false, true);
                    }
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                        {
                            m = (int)origin.X + i + (k == 2 ? width : 0);
                            n = (int)origin.Y - lengthY + j;
                            Tile tile = Main.tile[m, n];
                            if (house[k][i, j] == ID.Wall)
                            {
                                tile.active(true);
                                tile.type = tileID;
                            }
                            switch (house[k][i, j])
                            {
                                case -2:
                                    tile.active(false);
                                    break;
                                case -1:
                                    tile.active(true);
                                    break;
                                case ID.Cloud:
                                    tile.type = TileID.Cloud;
                                    goto case -1;
                                case ID.Floor:
                                    tile.type = tileID;
                                    goto case -1;
                                case ID.Platform:
                                    tile.type = TileID.Platforms;
                                    goto case -1;
                                case ID.Chest:
                                    WorldGen.PlaceChest(m, n);
                                    break;
                                case ID.Furniture:
                                    type = furniture[WorldGen.genRand.Next(furniture.Length)];
                                    WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Useful:
                                    type = useful[WorldGen.genRand.Next(useful.Length)];
                                    if (!Treasures.Vicinity(origin, 20, type))
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Decoration:
                                    type = decoration[WorldGen.genRand.Next(decoration.Length)];
                                    WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Lamp:
                                    WorldGen.PlaceTile(m, n, TileID.HangingLanterns);
                                    break;
                                case ID.Door:
                                    WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                                    break;
                            }
                        }
            }
            foreach (int[,] a in house)
                if (a != null && a.Length > 10)
                    ArchaeaWorld.TextOutput(a, "FortArrays", true, "House array:");
        }
        public class Magno : Structures
        {
            public new void Initialize()
            {
                house = new int[max][,];
                bool rand = WorldGen.genRand.Next(2) == 0;
                direction = new bool[] { !rand, rand, !rand };
                int randFloor = WorldGen.genRand.Next(max);
                bool craft = false;
                bool chest = false;
                for (int k = 0; k < max; k++)
                {
                    int randX = k != 1 ? WorldGen.genRand.Next(15, 28) : WorldGen.genRand.Next(15, 20);
                    int randY = WorldGen.genRand.Next(7, 9);
                    int numLights = 0;
                    int numObjects = 0;
                    bool furniture = false;
                    bool stairs = false;
                    house[k] = new int[randX, randY];
                    for (int i = 0; i < randX; i++)
                        for (int j = 0; j < randY; j++)
                        {
                            if (i == 0 || i == randX - 1 || (k != 1 && (j == 0 || j == randY - 1)))
                                house[k][i, j] = tileID;
                            if (k != 1 && (i == 0 || i == randX - 1) && j == randY - 3)
                                house[k][i, j] = ID.Door;
                            if (i > 0 && i < randX - 1)
                            {
                                int x;
                                int y;
                                int count = 0;
                                int top = direction[k] ? 8 : 12;
                                if (i == top && j == 1)
                                {
                                    while (!stairs)
                                    {
                                        if (count == randY - 1)
                                            stairs = true;
                                        if (direction[k] && i + count < randX - 1 && j + count < randY - 1)
                                            house[k][i + count, j + count] = ID.Stairs;
                                        if (!direction[k] && i - count > 0 && j + count < randY - 1)
                                            house[k][i - count, j + count] = ID.Stairs;
                                        count++;
                                    }
                                }
                                while (i == 1 && j == 1 && numLights < 2)
                                {
                                    x = i + WorldGen.genRand.Next(randX - 2);
                                    house[k][x, j] = ID.Lamp;
                                    numLights++;
                                }
                                if (i == 1 && j == randY - 2)
                                {
                                    while (numObjects < 3)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (x % 2 == 1)
                                            if (house[k][x, j] == 0)
                                            {
                                                house[k][x, j] = ID.Decoration;
                                                house[k][x + 1, j] = ID.Empty;
                                                numObjects++;
                                            }
                                    }
                                    while (!craft && k == randFloor)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (house[k][x, j] == 0)
                                        {
                                            house[k][x, j] = ID.Useful;
                                            house[k][x + 1, j] = ID.Empty;
                                            craft = true;
                                        }
                                    }
                                    while (!furniture && k != 1)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (x % 2 == 0)
                                            if (house[k][x, j] == 0)
                                            {
                                                house[k][x, j] = ID.Furniture;
                                                house[k][x + 1, j] = ID.Empty;
                                                furniture = true;
                                            }
                                    }
                                    while (!chest && k == randFloor)
                                    {
                                        x = i + WorldGen.genRand.Next(randX - 3);
                                        if (house[k][x, j] == 0)
                                        {
                                            house[k][x, j] = ID.Chest;
                                            house[k][x + 1, j] = ID.Empty;
                                            chest = true;
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            public bool MagnoHouse(Vector2 origin, bool fail = false)
            {
                if (fail || origin == Vector2.Zero)
                    return false;
                bool success = false;
                int x = (int)origin.X;
                int y = (int)origin.Y;
                int m = 0;
                int n = 0;
                int height = 0;
                int randFloor = WorldGen.genRand.Next(max);
                for (int k = 0; k < max; k++)
                {
                    int lengthX = house[k].GetLength(0);
                    int lengthY = house[k].GetLength(1);
                    for (int i = 0; i < lengthX; i++)
                        for (int j = 0; j < lengthY; j++)
                        {
                            m = i + x;
                            n = j + y + height;
                            Tile tile = Main.tile[m, n];
                            if (tile.wall == wallID || tile.type == tileID)
                                return false;
                            if (i > 0 && i < lengthX - 1 && j > 0 && j < lengthY - 1)
                                if (WorldGen.genRand.NextFloat() < 0.67f)
                                {
                                    tile.wall = wallID;
                                    WorldGen.PlaceWall(m, n, wallID, true);
                                }
                            if (house[k][i, j] == tileID)
                            {
                                tile.active(true);
                                tile.type = tileID;
                            }
                            else tile.active(false);
                            if (tile.type == tileID && tile.active())
                                success = true;
                            if (i > 6 && i < 14)
                            {
                                if (k == 0 && (j == 0 || j == lengthY - 1))
                                {
                                    tile.type = TileID.Platforms;
                                    WorldGen.SquareTileFrame(m, n, true);
                                }
                                if (k == max - 1 && j == 0)
                                {
                                    tile.type = TileID.Platforms;
                                    WorldGen.SquareTileFrame(m, n, true);
                                }
                                if (direction[k] && i == 7)
                                    tile.slope(1);
                                else if (!direction[k] && i == 13)
                                    tile.slope(2);
                            }
                        }
                    for (int i = 0; i < lengthX; i++)
                        for (int j = 0; j < lengthY; j++)
                        {
                            m = i + x;
                            n = j + y + height;
                            ushort type;
                            switch (house[k][i, j])
                            {
                                case ID.Door:
                                    WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                                    break;
                                case ID.Stairs:
                                    WorldGen.PlaceTile(m, n, TileID.Platforms);
                                    Main.tile[m, n].slope((byte)(direction[k] ? 1 : 2));
                                    break;
                                case ID.Lamp:
                                    WorldGen.PlaceTile(m, n, TileID.HangingLanterns);
                                    break;
                                case ID.Chest:
                                    WorldGen.PlaceChest(m, n, TileID.Containers);
                                    break;
                                case ID.Furniture:
                                    type = furniture[WorldGen.genRand.Next(furniture.Length)];
                                    if (!Treasures.Vicinity(origin, 50, type))
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Useful:
                                    type = useful[WorldGen.genRand.Next(useful.Length)];
                                    if (!Treasures.Vicinity(origin, 50, type))
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                case ID.Decoration:
                                    type = decoration[WorldGen.genRand.Next(decoration.Length)];
                                    if (Treasures.ProximityCount(origin, 50, type) < 5)
                                        WorldGen.PlaceTile(m, n, type);
                                    break;
                                default:
                                    break;
                            }
                        }
                    height += lengthY;
                }
                index++;
                return success;
            }
        }
        internal void PlaceChest(int i, int j, int width, ushort groundID)
        {
            i -= width;
            j -= 1;
            bool chest = false;
            int count = 0;
            int total = 100;
            while (!chest)
            {
                int m = i + WorldGen.genRand.Next(width - 1);
                Tile floor = Main.tile[m, j];
                Tile ground = Main.tile[m, j + 1];
                if (!floor.active() && ground.type == groundID)
                {
                    WorldGen.PlaceChest(m, j);
                    if (floor.type == TileID.Containers)
                        chest = true;
                }
                if (count < total)
                    count++;
                else break;
            }
        }
        internal void Decorate(int i, int j, int radius, ushort tileType)
        {
            var t = new Treasures();
            Vector2 v2 = new Vector2(i, j);
            var floor = Treasures.GetFloor(v2, radius, false, new ushort[] { tileType, TileID.Platforms });
            var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
            ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
            ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
            ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
            int length = 0;
            foreach (ushort tile in furniture)
                t.PlaceTile(floor, 2, 30, tile, true, false, false, 0, true, 40);
            length = WorldGen.genRand.Next(useful.Length);
            t.PlaceTile(floor, 1, 30, useful[length], true, false, false, 0, true, 40);
            foreach (ushort tile in decoration)
                t.PlaceTile(floor, 4, 20, tile, true);
            t.PlaceTile(ceiling, 3, 1, TileID.HangingLanterns, true, true);
            t = null;
        }
        public void Reset()
        {
            index = 0;
        }
    }
    /*  Final Debug Version
    public class Structures
    {
        public static int index;
        private int max = 3;
        public int[][,] house;
        public void Initialize(ushort tileID = TileID.StoneSlab)
        {
            house = new int[max][,];
            for (int k = 0; k < max; k++)
            {
                int randX = k != 1 ? WorldGen.genRand.Next(15, 28) : WorldGen.genRand.Next(12, 15);
                int randY = WorldGen.genRand.Next(8, 10);
                house[k] = new int[randX, randY];
                for (int i = 0; i < randX; i++)
                    for (int j = 0; j < randY; j++)
                    {
                        if (i == 0 || i == randX - 1 || j == 0 || j == randY - 1)
                            house[k][i, j] = tileID;
                        if (k != 1 && (i == 0 || i == randX - 1) && j > randY - 5 && j < randY - 1)
                            house[k][i, j] = 1;
                        if (k == 1 && i > 0 && i < randX - 1 && (j == 0 || j == randY - 1))
                            house[k][i, j] = 0;
                    }
            }
        }
        public bool MagnoHouse(Vector2 origin, bool fail = false, ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
        {
            if (fail || origin == Vector2.Zero)
                return false;
            bool success = false;
            int x = (int)origin.X;
            int y = (int)origin.Y;
            int m = 0;
            int n = 0;
            int height = 0;
            int randFloor = WorldGen.genRand.Next(max);
            for (int k = 0; k < max; k++)
            {
                int lengthX = house[k].GetLength(0);
                int lengthY = house[k].GetLength(1);
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = i + x;
                        n = j + y + height;
                        Tile tile = Main.tile[m, n];
                        if (tile.wall == wallID || tile.type == tileID)
                            return false;
                        if (i > 0 && i < lengthX - 1 && j > 0 && j < lengthY - 1)
                            if (WorldGen.genRand.NextFloat() < 0.67f)
                            {
                                tile.wall = wallID;
                                WorldGen.PlaceWall(m, n, wallID, true);
                            }
                        tile.type = (ushort)house[k][i, j];
                        if (tile.type != tileID)
                        {
                            tile.type = 0;
                            tile.active(false);
                        }
                        else tile.active(true);
                        if (tile.type == tileID && tile.active())
                            success = true;
                    }
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = i + x;
                        n = j + y + height;
                        Tile tile = Main.tile[m, n];
                        if (i > 6 && i < 14)
                        {
                            if (k == 0 && (j == 0 || j == lengthY - 1))
                            {
                                tile.type = TileID.Platforms;
                                WorldGen.SquareTileFrame(m, n, true);
                            }
                            if (k == max - 1 && j == 0)
                            {
                                tile.type = TileID.Platforms;
                                WorldGen.SquareTileFrame(m, n, true);
                            }
                        }
                        if (house[k][i, j] == 1)
                            WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                    }
                if (k == randFloor)
                    PlaceChest(m, n, lengthX, tileID);
                Decorate(m, n, 50, tileID);
                int[] rand = new int[] { 3, 4, 5, 6, 7, 8 };
                x -= k != 1 ? rand[WorldGen.genRand.Next(rand.Length)] : 0;
                height += lengthY;
            }
            index++;
            return success;
        }
        internal void PlaceChest(int i, int j, int width, ushort groundID)
        {
            i -= width;
            j -= 1;
            bool chest = false;
            int count = 0;
            int total = 100;
            while (!chest)
            {
                int m = i + WorldGen.genRand.Next(width - 1);
                Tile floor = Main.tile[m, j];
                Tile ground = Main.tile[m, j + 1];
                if (!floor.active() && ground.type == groundID)
                {
                    WorldGen.PlaceChest(m, j);
                    if (floor.type == TileID.Containers)
                        chest = true;
                }
                if (count < total)
                    count++;
                else break;
            }
        }
        internal void Decorate(int i, int j, int radius, ushort tileType)
        {
            var t = new Treasures();
            Vector2 v2 = new Vector2(i, j);
            var floor = Treasures.GetFloor(v2, radius, false, new ushort[] { tileType, TileID.Platforms });
            var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
            ArchaeaWorld.TextOutput(floor, "DecorationFloor");
            ArchaeaWorld.TextOutput(ceiling, "DecorationCeiling");
            ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
            ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
            ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
            int length = 0;
            foreach (ushort tile in furniture)
                if (!Treasures.Vicinity(v2, 60, tile))
                    t.PlaceTile(floor, 2, 30, tile, true, true);
            length = WorldGen.genRand.Next(useful.Length);
            for (int l = 0; l < 10; l++)
                if (!Treasures.Vicinity(v2, 60, useful[length]))
                    t.PlaceTile(floor, 1, 30, useful[length], true, true);
            foreach (ushort tile in decoration)
                t.PlaceTile(floor, 4, 20, tile, true, true);
            t.PlaceTile(ceiling, 1, 60, TileID.Chandeliers, true, true, false, 5, false, true);
            t = null;
        }
        public void Reset()
        {
            index = 0;
        }
        // Dec. 27th 8:00pm Debug
        private static int index;
        private int max = 3;
        public int[][,] house;
        public void Initialize(ushort tileID = TileID.StoneSlab)
        {
            house = new int[max][,];
            for (int k = 0; k < max; k++)
            {
                int randX = WorldGen.genRand.Next(14, 22);
                int randY = WorldGen.genRand.Next(8, 10);
                house[k] = new int[randX, randY];
                for (int i = 0; i < randX; i++)
                    for (int j = 0; j < randY; j++)
                    {
                        if (i == 0 || i == randX - 1 || j == 0 || j == randY - 1)
                            house[k][i, j] = tileID;
                    }
            }
        }
        public bool MagnoHouse(Vector2 origin, bool fail = false, ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
        {
            if (fail || origin == Vector2.Zero)
                return false;
            bool success = false;
            int x = (int)origin.X;
            int y = (int)origin.Y;
            int m = 0;
            int n = 0;
            int height = 0;
            int randFloor = WorldGen.genRand.Next(max);
            for (int k = 0; k < max; k++)
            {
                int lengthX = house[k].GetLength(0);
                int lengthY = house[k].GetLength(1);
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = i + x;
                        n = j + y + height;
                        Tile tile = Main.tile[m, n];
                        if (tile.wall == wallID || tile.type == tileID)
                            return false;
                    }
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = i + x;
                        n = j + y + height;
                        Tile tile = Main.tile[m, n];
                        tile.type = (ushort)house[k][i, j];
                        tile.active(tile.type == tileID ? true : false);
                        if (i > 0 && i < lengthX - 1 && j > 0 && j < lengthY - 1)
                        {
                            tile.wall = wallID;
                            WorldGen.PlaceWall(m, n, wallID, true);
                            if (tile.type != tileID)
                            {
                                tile.type = 0;
                                tile.active(false);
                                //  WorldGen.KillTile(m, n, false, false, true);
                            }
                        }
                        if (tile.type == tileID && tile.active())
                            success = true;
                    }
                ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, success.ToString());
                if (k == randFloor)
                    PlaceChest(m, n, lengthX, tileID);
                Decorate(m, n, 30, tileID);
                x += WorldGen.genRand.Next(-6, 6);
                height += lengthY;
            }
            index++;
            return success;
        }
        public void PlaceChest(int i, int j, int width, ushort groundID)
        {
            i -= width;
            j -= 1;
            int total = 100;
            for (int k = 0; k < total; k++)
            { 
                int m = i + WorldGen.genRand.Next(width - 1);
                Tile floor = Main.tile[m, j];
                Tile ground = Main.tile[m, j + 1];
                if (!floor.active() && ground.type == groundID)
                {
                    ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, "Can place chest here: " + m + " " + j);
                    WorldGen.PlaceTile(m, j, TileID.Containers);
                    if (floor.type == TileID.Containers)
                    {
                        ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, "Chest placed");
                        break;
                    }
                }
            }
        }
        public void Decorate(int i, int j, int radius, ushort tileType)
        {
            var t = new Treasures();
            Vector2 v2 = new Vector2(i, j);
            var floor = Treasures.GetFloor(v2, radius, false, tileType);
            var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
            ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
            ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
            ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation };
            foreach (ushort tile in decoration)
                t.PlaceTile(floor, 4, 10, tile, true, true);
            int randIndex = 0;
            int total = 30;
            for (int k = 0; k < total; k++)
            {
                ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, "Decorate For loop beginning reached: " + k);
                randIndex = WorldGen.genRand.Next(furniture.Length);
                if (!Treasures.Vicinity(v2, 40, furniture[randIndex]))
                    t.PlaceTile(floor, 1, 15, furniture[randIndex], true);
                randIndex = WorldGen.genRand.Next(useful.Length);
                if (!Treasures.Vicinity(v2, 40, useful[randIndex]))
                    t.PlaceTile(floor, 1, 15, useful[randIndex], true);
                if (!Treasures.Vicinity(v2, 40, TileID.Chandeliers))
                    t.PlaceTile(ceiling, 3, 25, TileID.Chandeliers, true, false);
                ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, "Decorate For loop end reached: " + k);
            }
            ArchaeaWorld.TextOutput(new Vector2[1], "HouseCheck", true, "This iteration of Decorate complete");
            t = null;
        }
        public void Reset()
        {
            index = 0;
        } */
    /* Dec. 27th 1:55pm Debug
    private static int index;
    private int max = 3;
    public int[][,] house;
    public void Initialize(ushort tileID = TileID.StoneSlab)
    {
        house = new int[max][,];
        for (int k = 0; k < max; k++)
        {
            int randX = WorldGen.genRand.Next(14, 22);
            int randY = WorldGen.genRand.Next(8, 10);
            house[k] = new int[randX, randY];
            for (int i = 0; i < randX; i++)
                for (int j = 0; j < randY; j++)
                {
                    if (i == 0 || i == randX - 1 || j == 0 || j == randY - 1)
                        house[k][i, j] = tileID;
                }
        }
    }
    public void MagnoHouse(Vector2 origin, bool fail = false, ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
    {
        if (fail || origin == Vector2.Zero)
            return;
        int x = (int)origin.X;
        int y = (int)origin.Y;
        int m = 0;
        int n = 0;
        int height = 0;
        int randFloor = WorldGen.genRand.Next(max);
        for (int k = 0; k < max; k++)
        {
            int lengthX = house[k].GetLength(0);
            int lengthY = house[k].GetLength(1);
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                {
                    m = i + x;
                    n = j + y + height;
                    Tile tile = Main.tile[m, n];
                    if (tile.wall == wallID || tile.type == tileID)
                        return;
                }
            for (int i = 0; i < lengthX; i++)
                for (int j = 0; j < lengthY; j++)
                {
                    m = i + x;
                    n = j + y + height;
                    Tile tile = Main.tile[m, n];
                    tile.type = (ushort)house[k][i, j];
                    tile.active(tile.type == tileID ? true : false);
                    if (i > 0 && i < lengthX - 1 && j > 0 && j < lengthY - 1)
                    {
                        tile.wall = wallID;
                        WorldGen.PlaceWall(m, n, wallID, true);
                        if (tile.type != tileID)
                            WorldGen.KillTile(m, n, false, false, true);
                    }
                }
            if (k == randFloor)
                PlaceChest(m, n, lengthX, tileID);
            Decorate(m, n, 30, tileID);
            x += WorldGen.genRand.Next(-6, 6);
            height += lengthY;
        }
        index++;
    }
    internal void PlaceChest(int i, int j, int width, ushort groundID)
    {
        i -= width;
        j -= 1;
        bool chest = false;
        while (!chest)
        {
            int m = i + WorldGen.genRand.Next(width - 1);
            Tile floor = Main.tile[m, j];
            Tile ground = Main.tile[m, j + 1];
            if (!floor.active() && ground.type == groundID)
            {
                WorldGen.PlaceTile(m, j, TileID.Containers);
                if (floor.type == TileID.Containers)
                    chest = true;
            }
        }
    }
    internal void Decorate(int i, int j, int radius, ushort tileType)
    {
        var t = new Treasures();
        Vector2 v2 = new Vector2(i, j);
        var floor = Treasures.GetFloor(v2, radius, false, tileType);
        var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
        ushort[] decoration = new ushort[]  { TileID.Tables, TileID.Statues, TileID.Pots };
        ushort[] furniture = new ushort[]   { TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
        ushort[] useful = new ushort[]      { TileID.Loom, TileID.SharpeningStation };
        foreach (ushort tile in decoration)
            t.PlaceTile(floor, 4, 30, tile);
        t.PlaceTile(floor, 2, 30, furniture[WorldGen.genRand.Next(furniture.Length)]);
        t.PlaceTile(floor, 1, 30, useful[WorldGen.genRand.Next(useful.Length)]);
        t.PlaceTile(ceiling, 3, 50, TileID.Chandeliers);
        t = null;
    }
    public void Reset()
    {
        index = 0;
    }*/
    /* Depracated structure generation
    public int total;
    private int m = 4;
    private int X;
    private int Y;
    private int[,] array;
    public void Start()
    {
        int x = MagnoDen.bounds.X;
        int y = MagnoDen.bounds.Y;
        int width = MagnoDen.bounds.Width / m;
        int height = MagnoDen.bounds.Height / m;
        int index = 0;
        array = new int[width, height];
        while (index < Math.Min(array.GetLength(0), array.GetLength(1)))
        {
            for (int i = index; i < array.GetLength(0); i++)
            {
                for (int j = index; j < array.GetLength(1); j++)
                {
                    int rand = WorldGen.genRand.Next(25);
                    array[i, j] = rand;
                    if (rand == 0)
                    {
                        X = x + (i * m);
                        Y = y + (j * m);
                        Vector2 origin = new Vector2(X, Y);
                        ArchaeaWorld.origins.Add(origin);
                        Generate(origin);
                        total++;
                        break;
                    }
                }
                index++;
                break;
            }
        }
    }
    public void Generate(Vector2 region, ushort tileType = TileID.StoneSlab, ushort wallType = WallID.StoneSlab)
    {
        int count = 0;
        int max = WorldGen.genRand.Next(3, 4);
        int x = (int)region.X;
        int y = (int)region.Y;
        int randX = WorldGen.genRand.Next(7, 13);
        int randY = WorldGen.genRand.Next(5, 7);
        for (int i = x - 5; i < x + randX; i++)
            for (int j = y; j < y + randY; j++)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.PlaceWall(i, j, wallType, true);
                Main.NewText(new Vector2(i, j).ToString());
            }
        while (count < max)
        {
            for (int i = x - 5; i < x + randX; i++)
                for (int j = y; j < y + randY; j++)
                {
                    int left = x - 5;
                    int right = x - 5 + randX;
                    int top = y;
                    int bottom = y + randY;
                    if (i == left || i == right || j == top || j == bottom)
                        WorldGen.PlaceTile(i, j, tileType, true, true);
                    bool gap = j == bottom & i >= 3 && i <= 6;
                    if (gap)
                        WorldGen.KillTile(i, j, false, false, true);
                }
            x += randX / 4 * (WorldGen.genRand.Next(2) == 0 ? -1 : 1);
            y += randY;
            count++;
        }
    }
    internal Tile ModifyTile(int i, int j, ushort type)
    {
        Main.tile[i, j].type = type;
        Main.tile[i, j].active(true);
        return Main.tile[i, j];
    }
    */
    //    }
    public class Digger
    {
        private int max;
        private ushort tileID;
        private ushort wallID;
        private readonly float radians = 0.017f;
        private Vector2[] centers;
        public Digger(int size, ushort tileID, ushort wallID)
        {
            max = size;
            this.tileID = tileID;
            this.wallID = wallID;
        }
        public void DigSequence(Vector2 center)
        {
            int num = 0;
            int border = 0;
            int size = 10;
            float radius = 0f;
            Relocate(ref center, out centers);
            size = WorldGen.genRand.Next(8, 15);
            border = size + 4;
            List<Vector2> list = new List<Vector2>();
            foreach (Vector2 path in centers)
            {
                num++;
                float weight = 0f;
                while (weight < 1f)
                {
                    list.Add(Vector2.Lerp(centers[num - 1], centers[Math.Min(num, centers.Length - 1)], weight));
                    weight += 0.2f;
                }
            }
            foreach (Vector2 lerp in list)
            {
                radius = size;
                while (radius < border)
                {
                    int offset = border / 2;
                    for (int i = (int)lerp.X - offset; i < (int)lerp.X + offset; i++)
                        for (int j = (int)lerp.Y - offset; j < (int)lerp.Y + offset; j++)
                        {
                            Main.tile[i, j].type = tileID;
                            Main.tile[i, j].active(true);
                            //  WorldGen.PlaceTile(i, j, tileID, true, true);
                        }
                    for (int i = (int)lerp.X - offset - 1; i < (int)lerp.X + offset - 1; i++)
                        for (int j = (int)lerp.Y - offset - 1; j < (int)lerp.Y + offset - 1; j++)
                            WorldGen.PlaceWall(i, j, wallID, true);
                    radius += 0.5f;
                }
            }
            radius = 0f;
            foreach (Vector2 lerp in list)
            {
                while (radius < size)
                {
                    int offset = size / 2;
                    for (int i = (int)lerp.X - offset; i < (int)lerp.X + offset; i++)
                        for (int j = (int)lerp.Y - offset; j < (int)lerp.Y + offset; j++)
                        {
                            Main.tile[i, j].type = 0;
                            Main.tile[i, j].active(false);
                            //  WorldGen.KillTile(i, j, false, false, true);
                        }
                    radius += 0.5f;
                }
                radius = 0f;
            }
            list.Clear();
        }
        public void Relocate(ref Vector2 position, out Vector2[] path)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int count = 0;
            int max = this.max;
            Vector2[] paths = new Vector2[max];
            while (count < max)
            {
                x = (int)position.X;
                y = (int)position.Y;
                int[] direction = new int[]
                {
                    -2, 5, -8, 10, 3, -1,
                    3, 1, -6, -3, 9, 2
                };
                int randX = direction[WorldGen.genRand.Next(direction.Length)];
                int randY = direction[WorldGen.genRand.Next(direction.Length)];
                position.X += randX;
                position.Y += randY;
                paths[count] = position;
                count++;
            }
            path = paths;
        }
    }
}
