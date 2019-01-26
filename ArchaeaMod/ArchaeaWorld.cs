﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

using ArchaeaMod.Merged.Tiles;
using ArchaeaMod.Merged.Walls;

namespace ArchaeaMod
{
    public class ArchaeaWorld : ModWorld
    {
        public static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod"); }
        }
        public static ushort magnoStone
        {
            get { return (ushort)getMod.TileType<m_stone>(); }
        }
        public static ushort magnoBrick
        {
            get { return (ushort)getMod.TileType<m_brick>(); }
        }
        public static ushort magnoOre
        {
            get { return (ushort)getMod.TileType<m_ore>(); }
        }
        public static ushort magnoChest
        {
            get { return (ushort)getMod.TileType<m_chest>(); }
        }
        public static ushort cOre
        {
            get { return (ushort)getMod.TileType<c_ore>(); }
        }
        public static ushort crystal
        {
            get { return (ushort)getMod.TileType<c_crystalsmall>(); }
        }
        public static ushort crystal2x1
        {
            get { return (ushort)getMod.TileType<c_crystal2x1>(); }
        }
        public static ushort crystal2x2
        {
            get { return (ushort)getMod.TileType<c_crystal2x2>(); }
        }
        public static ushort magnoBrickWall
        {
            get { return (ushort)getMod.WallType<magno_brick>(); }
        }
        public static ushort magnoStoneWall
        {
            get { return (ushort)getMod.WallType<magno_stone>(); }
        }
        public static Miner miner;
        public static List<Vector2> origins = new List<Vector2>();
        private Treasures t;
        public static Vector2[] genPosition;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int CavesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Granite")); // Granite
            if (CavesIndex != -1)
            {
                miner = new Miner();
                tasks.Insert(CavesIndex, new PassLegacy("Miner", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "MINER";
                    miner.active = true;
                    miner.Reset();
                    while (miner.active)
                        miner.Update();
                    genPosition = miner.genPos;
                    progress.End();
                }));
            }
            int shinies = tasks.FindIndex(pass => pass.Name.Equals("Shinies"));
            if (shinies != -1)
            {
                tasks.Insert(shinies, new PassLegacy("Mod Shinies", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    for (int k = 0; k < (int)((4200 * 1200) * 6E-05); k++)
                    {
                        //  WorldGen.TileRunner(WorldGen.genRand.Next((int)(genPosition[0].X / 16) - miner.edge / 2, (int)(genPosition[1].X / 16) + miner.edge / 2), WorldGen.genRand.Next((int)genPosition[0].Y / 16 - miner.edge / 2, (int)genPosition[1].Y / 16 + miner.edge / 2), WorldGen.genRand.Next(15, 18), WorldGen.genRand.Next(2, 6), magnoDirt, false, 0f, 0f, false, true);
                        int randX = WorldGen.genRand.Next((int)(genPosition[0].X / 16) - miner.edge / 2, (int)(genPosition[1].X / 16) + miner.edge / 2);
                        int randY = WorldGen.genRand.Next((int)genPosition[0].Y / 16 - miner.edge / 2, (int)genPosition[1].Y / 16 + miner.edge / 2);
                        if (Main.tile[randX, randY].type == magnoStone)
                            WorldGen.TileRunner(randX, randY, WorldGen.genRand.Next(9, 12), WorldGen.genRand.Next(2, 6), magnoOre, false, 0f, 0f, false, true);
                        progress.Value = k / (float)((4200 * 1200) * 6E-05);
                    }
                    progress.End();
                }));
            }
            int index2 = tasks.FindIndex(pass => pass.Name.Equals("Lakes"));
            if (index2 != -1)
            {
                tasks.Insert(index2, new PassLegacy("Sky Generation", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "Fort";
                    Vector2 position;
                    do
                    {
                        position = new Vector2(WorldGen.genRand.Next(200, Main.maxTilesX - 200), 125);
                    } while (position.X < Main.spawnTileX + 150 && position.X > Main.spawnTileX - 150);
                    var s = new Structures(position);
                    s.Initialize();
                    s.SkyRoom(true);
                    progress.Value = 1f;
                    progress.End();
                }));
            }
            int index3 = tasks.FindIndex(pass => pass.Name.Equals("Clean Up Dirt"));
            if (index3 != -1)
            {
                tasks.Insert(index3, new PassLegacy("Mod Generation", delegate (GenerationProgress progress)
                {
                    progress.Start(1f);
                    progress.Message = "Magno extras";
                    t = new Treasures();
                    int place = 0;
                    int width = Main.maxTilesX - 100;
                    int height = Main.maxTilesY - 100;
                    Vector2[] ceilings = Treasures.GetCeiling(new Vector2(100, 100), width, height, false, magnoStone);
                    Vector2[] walls = Treasures.GetWall(100, 100, (int)(Main.rightWorld / 16f) - 100, (int)(Main.bottomWorld / 16f) - 200, new ushort[] { magnoStone });
                    Vector2[] floors = Treasures.GetFloor(new Vector2(100, 100), width, height, false, new ushort[] { magnoStone });
                    foreach (Vector2 ceiling in ceilings)
                        t.PlaceTile((int)ceiling.X, (int)ceiling.Y, crystal, false, false, 8);
                    foreach (Vector2 wall in walls)
                        t.PlaceTile((int)wall.X, (int)wall.Y, crystal, false, false, 8);
                    foreach (Vector2 floor in floors)
                        if (floor != Vector2.Zero)
                        {
                            int i = (int)floor.X;
                            int j = (int)floor.Y;
                            if (!Main.tile[i + 1, j].active())
                            {
                                place++;
                                if (place % 3 == 0)
                                    t.PlaceTile(i, j, crystal2x2, true, false, 8);
                                else t.PlaceTile(i, j, crystal2x1, true, false, 8);
                            }
                            else t.PlaceTile(i, j, crystal, false, false, 8);
                        }
                    progress.Value = 1f;
                    progress.End();
                }));
            }
            int index4 = tasks.FindIndex(pass => pass.Name.Equals("Pyramids"));
            if (index4 != -1)
            {
                tasks.Insert(index4, new PassLegacy("Sorting Floating Tiles", delegate (GenerationProgress progress)
                {
                    progress.Message = "Magno Sorting";
                    for (int j = 100; j < Main.maxTilesY - 250; j++)
                        for (int i = 100; i < Main.maxTilesX - 100; i++)
                        {
                            Tile t = Main.tile[i, j];
                            Tile[] tiles = new Tile[]
                            {
                                Main.tile[i, j + 1],
                                Main.tile[i - 1, j],
                                Main.tile[i + 1, j]
                            };
                            int count = 0;
                            if (t.type == crystal)
                            {
                                foreach (Tile tile in tiles)
                                {
                                    if (!tile.active())
                                        count++;
                                    if (count == 3)
                                    {
                                        t.active(false);
                                        break;
                                    }
                                }
                            }
                        }
                }));
            }
            int index5 = tasks.FindIndex(pass => pass.Name.Equals("Dirt Rock Wall Runner"));
            tasks.Insert(index5, new PassLegacy("Structure Generation", delegate (GenerationProgress progress)
            {
                progress.Start(1f);
                progress.Message = "More Magno";
                var m = new Structures.Magno();
                m.tileID = magnoBrick;
                m.wallID = magnoBrickWall;
                Vector2 origin = new Vector2(100, 100);
                Vector2[] regions = Treasures.GetRegion(origin, Main.maxTilesX - 100, Main.maxTilesY - 250, false, true, new ushort[] { magnoStone });
                int count = 0;
                int total = (int)Math.Sqrt(regions.Length);
                int max = WorldGen.genRand.Next(5, 8);
                for (int i = 0; i < max; i++)
                {
                    m.Initialize();
                    while (!m.MagnoHouse(regions[WorldGen.genRand.Next(regions.Length)]))
                    {
                        if (count < total)
                            count++;
                        else break;
                    }
                    count = 0;
                    progress.Value = (float)i / max;
                }
                progress.Value = 1f;
                progress.End();
            }));
        
            #region Vector2 array
            /* int x = MagnoDen.bounds.X;
            int y = MagnoDen.bounds.Y;
            int right = MagnoDen.bounds.Right;
            int bottom = MagnoDen.bounds.Bottom;
            Vector2[] regions = new Vector2[MagnoDen.bounds.Width * MagnoDen.bounds.Height];
            for (int i = x; i < right; i++)
                for (int j = y; j < bottom; j++)
                {
                    if (Main.tile[i, j].type == TileID.PearlstoneBrick)
                        regions[count] = new Vector2(i, j);
                    count++;
                }*/
            #endregion
        }

        public bool MagnoBiome;
        public bool SkyFort;
        public override void TileCountsAvailable(int[] tileCounts)
        {
            MagnoBiome = tileCounts[magnoStone] >= 50;
            SkyFort = tileCounts[TileID.Dirt] >= 20;
        }
        private bool begin;
        private bool q;
        public override void PreUpdate()
        {
            //Structures.Magno s = null;
            if (ArchaeaPlayer.KeyPress(Keys.Q))
            {
                if (!begin)
                {
                    //s = new Structures.Magno();
                    //t = new Treasures();
                    begin = true;
                }
                //s.Initialize();
                //s.MagnoHouse(new Vector2(Main.spawnTileX, Main.spawnTileY));
                //var s = new Structures();
                //s.Initialize();
                //s.SkyRoom(true);
                //int i = Main.spawnTileX;
                //int j = 20;
                //s.CloudForm(i, j);
            }
        }
        public static bool Inbounds(int i, int j)
        {
            return i < Main.maxTilesX - 50 && i > 50 && j < Main.maxTilesY - 200 && j > 50;
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
                    if (!ArchaeaWorld.Inbounds(x, y)) continue;
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
                if (onlyOnWall && Main.tile[x, y].wall != wallID)
                {
                    index++;
                    continue;
                }
                if (proximity && Vicinity(getFloor[index], radius, newTileID))
                {
                    index++;
                    continue;
                }
                if (genPlace)
                    WorldGen.PlaceTile(x, y, newTileID, true, force);
                else
                {
                    tile.active(true);
                    tile.type = newTileID;
                }
                if (total == 1 && tile.type == newTileID && tile.active())
                    break;
                if (iterate && index == getFloor.Length - 1)
                    index = 0;
                index++;
            }
        }
        public bool PlaceTile(int i, int j, ushort tileType, bool genPlace = false, bool force = false, int proximity = -1, bool wall = false)
        {
            Tile tile = Main.tile[i, j];
            if (proximity != -1 && Vicinity(new Vector2(i, j), proximity, tileType))
                return false;
            if (!genPlace)
            {
                tile.active(true);
                tile.type = tileType;
            }
            else WorldGen.PlaceTile(i, j, tileType, true, force);
            if (tile.type == tileType)
                return true;
            return false;
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
        public static Vector2[] GetFloor(Vector2 region, int width, int height, bool overflow = false, ushort[] floorIDs = null)
        {
            int index = width * height * floorIDs.Length;
            int amount = (int)Math.Sqrt(index) / 10;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort floorType in floorIDs)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        Tile floor = Main.tile[i, j];
                        Tile ground = Main.tile[i, j + 1];
                        if (floor.active() && Main.tileSolid[floor.type]) continue;
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
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
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
        public static Vector2[] GetCeiling(Vector2 region, int width, int height, bool overflow = false, ushort tileType = 0)
        {
            var tiles = new List<Vector2>();
            for (int i = (int)region.X; i < width; i++)
                for (int j = (int)region.Y; j < height; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                    Tile roof = Main.tile[i, j];
                    Tile ceiling = Main.tile[i, j + 1];
                    if (ceiling.active() && Main.tileSolid[ceiling.type]) continue;
                    if (roof.active() && Main.tileSolid[roof.type] && roof.type == tileType)
                        tiles.Add(new Vector2(i, j + 1));
                }
            return tiles.ToArray();
        }
        public static Vector2[] GetRegion(Vector2 region, int width, int height, bool overflow = false, bool attach = false, ushort[] tileTypes = null)
        {
            int index = width * height * tileTypes.Length;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort tileType in tileTypes)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (count >= tiles.Length) continue;
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (attach && Main.tile[i, j].type != tileType) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        tiles[count] = new Vector2(i, j);
                        count++;
                    }
            return tiles;
        }
        public static Vector2[] GetWall(Vector2 region, int width, int height, bool overflow = false, bool attach = false, ushort[] attachTypes = null)
        {
            int index = width * height * attachTypes.Length;
            int count = 0;
            var tiles = new Vector2[index];
            foreach (ushort tileType in attachTypes)
                for (int i = (int)region.X; i < (int)region.X + width; i++)
                    for (int j = (int)region.Y; j < (int)region.Y + height; j++)
                    {
                        if (count >= tiles.Length) continue;
                        if (!ArchaeaWorld.Inbounds(i, j)) continue;
                        if (overflow & WorldGen.genRand.Next(5) == 0) continue;
                        Tile tile = Main.tile[i, j];
                        Tile wallL = Main.tile[i - 1, j];
                        Tile wallR = Main.tile[i + 1, j];
                        if (wallL.active() && Main.tileSolid[wallL.type])
                            if (!tile.active() || !Main.tileSolid[tile.type])
                            {
                                if (attach && wallL.type != tileType) continue;
                                tiles[count] = new Vector2(i, j);
                            }
                        if (wallR.active() && Main.tileSolid[wallR.type])
                            if (!tile.active() || !Main.tileSolid[tile.type])
                            {
                                if (attach && wallR.type != tileType) continue;
                                tiles[count] = new Vector2(i, j);
                            }
                        count++;
                    }
            return tiles;
        }
        public static Vector2[] GetWall(int x, int y, int width, int height, ushort[] tileTypes = null, int radius = -1)
        {
            int count = 0;
            List<Vector2> list = new List<Vector2>();
            foreach (ushort tileType in tileTypes)
                for (int i = x; i < width; i++)
                    for (int j = y; j < width; j++)
                    {
                        if (!ArchaeaWorld.Inbounds(i, j))
                            continue;
                        if (radius != -1 && Vicinity(new Vector2(i, j), radius, tileType))
                            continue;
                        Tile up = Main.tile[i, j - 1];
                        Tile left = Main.tile[i - 1, j];
                        Tile right = Main.tile[i + 1, j];
                        if ((left.type == tileType || right.type == tileType) && !up.active())
                        {
                            list.Add(new Vector2(i, j));
                            count++;
                        }
                    }
            return list.ToArray();
        }
        public static bool Vicinity(Vector2 region, int radius, ushort tileType)
        {
            int x = (int)region.X;
            int y = (int)region.Y;
            for (int i = x - radius; i < x + radius; i++)
                for (int j = y - radius; j < y + radius; j++)
                {
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    if (Main.tile[i, j].type == tileType)
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
                    if (!ArchaeaWorld.Inbounds(i, j)) continue;
                    Tile tile = Main.tile[i, j];
                    if (tile.type == tileType)
                        count++;
                }
            return count;
        }
    }
    public class Structures
    {
        internal bool[] direction;
        public static int index;
        private int count;
        private int max = 3;
        public int[][,] house;
        public int[][,] rooms;
        public ushort tileID;
        public ushort wallID;
        private ushort[] decoration = new ushort[] { TileID.Statues, TileID.Pots };
        private ushort[] furniture = new ushort[] { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
        private ushort[] useful = new ushort[] { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
        private Vector2 origin;
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
                Wire = 14,
                Window = 15;
        }
        class RoomID
        {
            public const int
                Empty = 0,
                FilledIn = 1,
                Safe = 2,
                Danger = 3,
                Chest = 4,
                Platform = 5,
                Start = 6,
                End = 7,
                Lighted = 8,
                Decorated = 9;
        }
        class FortID
        {
            public const int
                Light = 0,
                Dark = 1;
        }
        public Structures(Vector2 origin = default(Vector2), ushort tileID = TileID.StoneSlab, ushort wallID = WallID.StoneSlab)
        {
            this.tileID = tileID;
            this.wallID = wallID;
            this.origin = origin;
        }
        public void Initialize(bool forceFill = true, bool genWalls = false)
        {
            int x;
            int y;
            int radius = 105;
            int lX = radius - 45;
            int lY = radius - 6;
            int lengthX;
            int lengthY;
            int roomX = 15;
            int roomY = 9;
            index = -1;
            house = new int[max][,];
            rooms = new int[max - 1][,];
            for (int k = 0; k < max; k++)
            {
                switch (k)
                {
                    case -3:
                        break;
                    case -2:
                        RoomItems(index, k);
                        break;
                    case -1:
                        index++;
                        rooms[index] = new int[lX / roomX, lY / roomY];
                        int tries = 0;
                        while (true)
                        {
                            for (int i = 0; i < rooms[index].GetLength(0); i++)
                                for (int j = 0; j < rooms[index].GetLength(1); j++)
                                    rooms[index][i, j] = RoomID.FilledIn;
                            int length = rooms[index].GetLength(0);
                            bool flip = index == FortID.Dark;
                            bool b = false;
                            int m = flip ? length - 1 : 0;
                            for (int j = rooms[index].GetLength(1) - 1; j >= 0; j--)
                            {
                                b = false;
                                for (int i = m; flip ? i >= -1 : i <= rooms[index].GetLength(0); i -= flip ? 1 : -1)
                                {
                                    bool rand = WorldGen.genRand.Next(15) == 0;
                                    bool edge = j == 0;
                                    if (!flip)
                                    {
                                        m = Math.Min(i, length - 1);
                                        if ((i == length || rand) && !edge)
                                        {
                                            m = Math.Min(i, length - 1);
                                            rooms[index][m, j] = RoomID.Platform;
                                            b = true;
                                            break;
                                        }
                                        rooms[index][m, j] = RoomID.Empty;
                                    }
                                    else
                                    {
                                        m = Math.Max(i, 0);
                                        if ((i < 0 || rand) && !edge)
                                        {
                                            rooms[index][m, j] = RoomID.Platform;
                                            b = true;
                                            break;
                                        }
                                        rooms[index][m, j] = RoomID.Empty;
                                    }
                                }
                                flip = !flip;
                            }
                            tries++;
                            int count = 0;
                            int chest = 0;
                            int filledIn = 0;
                            for (int j = rooms[index].GetLength(1) - 1; j >= 0; j--)
                                for (int i = 0; i < rooms[index].GetLength(0); i++)
                                {
                                    if (rooms[index][i, j] == RoomID.FilledIn)
                                        filledIn++;
                                    if (j > 0 && rooms[index][i, j] == RoomID.Platform && (rooms[index][i, j - 1] == RoomID.Empty || rooms[index][i, j - 1] == RoomID.Platform))
                                    {
                                        count++;
                                        if (count == rooms[index].GetLength(1) - 1 && filledIn >= (forceFill ? Math.Max(12, (rooms[index].GetLength(0) * rooms[index].GetLength(1) / 2) - tries) : 0))
                                        {
                                            int lx = rooms[index].GetLength(0) - 1;
                                            int ly = rooms[index].GetLength(1) - 1;
                                            bool light = index == FortID.Light;
                                            rooms[index][light ? 0 : lx, ly] = RoomID.Start;
                                            rooms[index][light ? lx : 0, 0] = RoomID.End;
                                            goto case -2;
                                        }
                                    }
                                    int randRoom = WorldGen.genRand.Next(2, 5);
                                    if (randRoom == RoomID.Chest)
                                        chest++;
                                    if (rooms[index][i, j] != RoomID.FilledIn && rooms[index][i, j] != RoomID.Platform)
                                    {
                                        rooms[index][i, j] = randRoom;
                                        if (randRoom == RoomID.Chest && chest > 3)
                                            rooms[index][i, j] = WorldGen.genRand.Next(2, 4);
                                    }
                                }
                        }
                    case 0:
                        house[k] = new int[lX, lY];
                        lengthX = house[k].GetLength(0);
                        lengthY = house[k].GetLength(1);
                        for (int i = 0; i < lengthX; i++)
                            for (int j = 0; j < lengthY; j++)
                                if ((genWalls && (i == 0 || i == lengthX - 1 || i % roomX == 0 || j == 0 || j == lengthY - 1 || (j % roomY == 0 && j != lengthY - 1))) || i == 0 || i == lengthX - 1 || j % roomY == 0 || j == lengthY - 1)
                                    house[k][i, j] = ID.Wall;
                        goto case -1;
                    case 1:
                        house[k] = new int[30, radius / 2];
                        lengthX = house[k].GetLength(0);
                        lengthY = house[k].GetLength(1);
                        for (int i = 0; i < lengthX; i++)
                            for (int j = 0; j < lengthY; j++)
                            {
                                if (i == 0 || i == lengthX - 1 || j == 0 || j == lengthY - 1 || (j % roomY == 0 && j != radius))
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
                    case 2:
                        goto case 0;
                }
            }
        }
        public void CloudForm(int i, int j)
        {
            int width = 90;
            var d = new Draw();
            for (int m = 0; m < width; m++)
                for (int n = 0; n < width / 2; n++)
                {
                    if (WorldGen.genRand.Next(25) == 0)
                        for (float k = 0; k < 12; k += 12 * Draw.radian)
                            for (float r = 0f; r < Math.PI * 2f; r += d.radians(k))
                            {
                                int cos = (int)(i + m + k * Math.Cos(r));
                                int sine = (int)(j + n + k / 2f * Math.Sin(r));
                                Main.tile[cos, sine].active(true);
                                Main.tile[cos, sine].type = TileID.Cloud;
                            }
                }
        }
        public void RoomItems(int k, int index = 0, int roomX = 15, int roomY = 9, bool genWalls = false)
        {
            for (int i = 0; i < rooms[k].GetLength(0); i++)
                for (int j = 0; j < rooms[k].GetLength(1); j++)
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
                            switch (rooms[k][i, j])
                            {
                                case -2:
                                    if (genWalls)
                                    {
                                        int door = 0;
                                        if (rooms[k][Math.Max(i - 1, 0), j] != RoomID.FilledIn)
                                            if (i != 0 && q == m && r < height)
                                            {
                                                if (j != rooms[k].GetLength(1) - 1 && r >= height - 3)
                                                    house[index][q, r] = ID.Empty;
                                                else if (j == rooms[k].GetLength(1) - 1)
                                                {
                                                    door = 1;
                                                    if (r >= height - 4 && r < height - 1)
                                                        house[index][q, r] = ID.Empty;
                                                }
                                                else door = 0;
                                                if (r == height - 1 - door)
                                                    house[index][q, r] = ID.Door;
                                            }
                                    }
                                    if (rooms[k][i, j] == RoomID.Start)
                                    {
                                        if (q == (k == FortID.Light ? m : width - 1) && r >= height - 4 && r < height - 1)
                                        {
                                            house[index][q, r] = ID.Empty;
                                            if (r == height - 3)
                                                house[index][q, r] = ID.Door;
                                        }
                                        if (q > m + 5 && q < m + 10 && r == n)
                                            house[index][q, r] = ID.Platform;
                                    }
                                    if (rooms[k][i, j] == RoomID.End)
                                        if (q == (k == FortID.Light ? width - 1 : m) && r >= height - 3 && r < height)
                                        {
                                            house[index][q, r] = ID.Empty;
                                            if (r == height - 2)
                                                house[index][q, r] = ID.Door;
                                        }
                                    break;
                                case -1:
                                    if (genWalls)
                                    {
                                        if ((i == 0 && q == m) || (i == rooms[k].GetLength(0) - 1 && q == width - 1))
                                            house[index][q, r] = ID.Wall;
                                        if (rooms[k][Math.Max(i - 1, 0), j] != RoomID.FilledIn)
                                            if (i != 0 && q == m)
                                            {
                                                house[index][q, r] = ID.Wall;
                                                goto case -2;
                                            }
                                    }
                                    break;
                                case RoomID.Empty:
                                    goto case RoomID.Decorated;
                                case RoomID.Start:
                                    goto case -2;
                                case RoomID.End:
                                    goto case -2;
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
                                    if (j == rooms[k].GetLength(1) - 1)
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
                                    goto case RoomID.Lighted;
                                case RoomID.Decorated:
                                    added = 0;
                                    if (j == rooms[k].GetLength(1) - 1)
                                        floor = 2;
                                    else floor = 1;
                                    if (q == m + 1 && r == height - floor)
                                    {
                                        while (added < 4)
                                        {
                                            int ground = q + WorldGen.genRand.Next(1, 11);
                                            if (ground % 2 == 0)
                                            {
                                                house[index][ground, r] = ID.Decoration;
                                                added++;
                                            }
                                        }
                                    }
                                    goto case -1;
                                case RoomID.Lighted:
                                    bool lamp = false;
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
                                    if (j == rooms[k].GetLength(1) - 1)
                                        floor = 2;
                                    else floor = 1;
                                    if (q == m + 3 && q < width - 3 && r == height - floor)
                                    {
                                        while (added < 4)
                                        {
                                            int ground = q + WorldGen.genRand.Next(1, 11);
                                            if (ground % 2 == 1)
                                            {
                                                if (house[index][ground, r] == ID.Empty)
                                                {
                                                    house[index][ground, r] = ID.Decoration;
                                                    added++;
                                                }
                                                if (!placed)
                                                {
                                                    house[index][ground, r] = ID.Chest;
                                                    placed = true;
                                                }
                                            }
                                        }
                                    }
                                    if (q > m + 3 && q < width - 2 && r >= 3 && r <= 3)
                                        house[index][q, r] = ID.Window;
                                    goto case -1;
                                case RoomID.Danger:
                                    goto case -1;
                            }
                }
        }
        public void SkyRoom(bool killRegion = false)
        {
            int m;
            int n;
            int w = 0;
            for (int k = 0; k < max; k++)
            {
                int width = house[0].GetLength(0);
                int lengthX = house[k].GetLength(0);
                int lengthY = house[k].GetLength(1);
                ushort type;
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = (int)origin.X + i + (k == 1 ? width : 0) + (k == 2 ? width + house[1].GetLength(0) : 0);
                        n = (int)origin.Y - lengthY - 1 + j;
                        if (killRegion)
                            WorldGen.KillTile(m, n, false, false, true);
                        Tile tile = Main.tile[m, n];
                        if (house[k][i, j] == ID.Wall)
                        {
                            tile.active(true);
                            tile.type = tileID;
                        }
                        if (house[k][i, j] != ID.Wall && house[k][i, j] != ID.Door && house[k][i, j] != ID.Window)
                            WorldGen.PlaceWall(m, n, wallID);
                    }
                for (int i = 0; i < lengthX; i++)
                    for (int j = 0; j < lengthY; j++)
                    {
                        m = (int)origin.X + i + (k == 1 ? width : 0) + (k == 2 ? width + house[1].GetLength(0) : 0);
                        n = (int)origin.Y - lengthY - 1 + j;
                        Tile tile = Main.tile[m, n];
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
                                tile.active(false);
                                tile.type = 0;
                                WorldGen.PlaceDoor(m, n, TileID.ClosedDoor);
                                break;
                            case ID.Window:
                                tile.wall = (ushort)WorldGen.genRand.Next(88, 93);
                                break;
                        }
                    }
            }
        }
        public class Magno : Structures
        {
            public void Initialize()
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
                if (!ArchaeaWorld.Inbounds(x, y))
                    return false;
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
            var floor = Treasures.GetFloor(v2, radius, radius, false, new ushort[] { tileType, TileID.Platforms });
            var ceiling = Treasures.GetCeiling(v2, radius, false, tileType);
            ushort[] decoration = new ushort[]  { TileID.Statues, TileID.Pots };
            ushort[] furniture = new ushort[]   { TileID.Tables, TileID.Chairs, TileID.Pianos, TileID.GrandfatherClocks, TileID.Dressers };
            ushort[] useful = new ushort[]      { TileID.Loom, TileID.SharpeningStation, TileID.Anvils, TileID.CookingPots };
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
                    for (int i = (int)lerp.X - offset + 2; i < (int)lerp.X + offset - 1; i++)
                        for (int j = (int)lerp.Y - offset + 2; j < (int)lerp.Y + offset - 1; j++)
                            Main.tile[i, j].wall = wallID;
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

    public class Miner : ModWorld
    {
        public Mod moda = ModLoader.GetMod("ArchaeaMod");
        public static string progressText = "";
        static int numMiners = 0, randomX, randomY, bottomBounds = Main.maxTilesY, rightBounds = Main.maxTilesX, circumference, ticks;
        public int edge = 128;
        float mineBlockX = 256, mineBlockY = 256;
        float RightBounds;
        static bool runner = false, grassRunner = false, fillerRunner = false, russianRoulette = false;
        public Vector2 center = new Vector2((Main.maxTilesX / 2) * 16, (Main.maxTilesY / 2) * 16);
        public int buffer = 1, offset = 200;
        int whoAmI = 0, type = 0;
        int XOffset = 512, YOffset = 384;
        public int jobCount = 0;
        public int jobCountMax = 32;
        static int moveID, lookFurther, size = 1;
        public Vector2 minerPos;
        public Vector2 finalVector;
        static Vector2 oldMinerPos, deadZone = Vector2.Zero;
        Vector2 position;
        Vector2 mineBlock;
        public Vector2 baseCenter;
        bool init = false;
        bool fail;
        bool switchMode = false;
        public bool active = true;
        public Vector2[] genPos = new Vector2[2];
        Vector2[] minePath = new Vector2[800 * 800];
        //  for loop takes care of need to generate new miners
        //  Miner[] ID = new Miner[400];
        public void Init()
        {
            if (whoAmI == 0)
            {
                //  remove these comments for public version
                float offset = XOffset * WorldGen.genRand.Next(-1, 1);
                if (offset == 0)
                {
                    offset = XOffset;
                }
                minerPos = center + new Vector2(offset * 16f, Main.maxTilesY - YOffset);
                center = minerPos;
                baseCenter = minerPos;
            }
            else
            {
                int RandomX = WorldGen.genRand.Next(-2, 2);
                int RandomY = WorldGen.genRand.Next(-2, 2);
                if (RandomX != 0 && RandomY != 0)
                {
                    mineBlock = new Vector2(mineBlockX * RandomX, mineBlockY * RandomY);
                    minerPos += mineBlock;
                }
                else
                {
                    mineBlock = new Vector2(mineBlockX, mineBlockY);
                    minerPos += mineBlock;
                    return;
                }
            }
            minePath[0] = center;
            init = true;
            //  Main.spawnTileX = (int)center.X / 16;
            //  Main.spawnTileY = (int)center.Y / 16;
            progressText = jobCount + " initiated, " + Math.Round((double)((float)jobCount / jobCountMax) * 10, 0) + "%";
        }
        public void Update()
        {
            if (!init) Init();
            if (init && whoAmI == 0)
            {
                for (int k = 0; whoAmI < 800; k++)
                {
                    Mine();
                }
            }
            else if (whoAmI > 0 && whoAmI <= 800)
            {
                for (int k = 0; whoAmI < 800; k++)
                {
                    Mine();
                }
                if (whoAmI == 800)
                {
                    jobCount++;
                    Init();
                    whoAmI = 1;
                }
            }

            if (minerPos.X < center.X)
                center.X = minerPos.X;
            if (minerPos.Y < center.Y)
                center.Y = minerPos.Y;
            if (minerPos.X > oldMinerPos.X)
                oldMinerPos.X = minerPos.X;
            if (minerPos.Y > oldMinerPos.Y)
                oldMinerPos.Y = minerPos.Y;

            if (jobCount > jobCountMax)
            {
                progressText = "Process complete";
                int layer = (int)Main.worldSurface;
                int offset = Main.maxTilesY / 2;
                if (minerPos.X < center.X)
                {
                    genPos[0] = new Vector2(minerPos.X, center.Y);
                    genPos[1] = oldMinerPos;
                }
                if (minerPos.X > center.X)
                {
                    genPos[0] = center;
                    genPos[1] = oldMinerPos;
                }
                if (!switchMode)
                {
                    switchMode = true;
                    Dig();
                }
            }
            if (switchMode)
            {
                //  jobCount--;
                Terminate();
                //  Reset();
            }
        }
        public void AverageMove() // most average path, sometimes most interesting
        {
            size = WorldGen.genRand.Next(1, 3);
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.X += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1)
            {
                minerPos.Y -= 16;
                Dig();
            }
            GenerateNewMiner();
        }
        public void DirectionalMove() // tends to stick to a path
        {
            size = WorldGen.genRand.Next(1, 3);
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                minerPos.X += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                minerPos.X -= 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active())
            {
                minerPos.Y += 16;
                Dig();
            }
            if (WorldGen.genRand.Next(4) == 1 && Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (!Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active() &&
                !Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active() &&
                !Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active() &&
                !Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                lookFurther++;
                if (lookFurther % 2 == 0) progressText = "Looking " + lookFurther + " tiles further";
                PlaceWater();
            }
            else lookFurther = 0;
            GenerateNewMiner();
        }
        public void ToTheSurfaceMove() // it likes randomizer = 3
        {
            moveID = 0;
            if (Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].active())
            {
                moveID++;
            }
            if (Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].active())
            {
                moveID++;
            }
            int randomizer = WorldGen.genRand.Next(0, moveID);
            size = WorldGen.genRand.Next(1, 3);
            if (randomizer == 0)
            {
                lookFurther++;
                int adjust = WorldGen.genRand.Next(1, 4);
                if (adjust == 1)
                {
                    minerPos.X -= 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 2)
                {
                    minerPos.X += 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 3)
                {
                    minerPos.Y -= 16;
                    PlaceWater();
                    Dig();
                }
                else if (adjust == 4)
                {
                    minerPos.Y += 16;
                    PlaceWater();
                    Dig();
                }
                return;
            }
            if (randomizer == 1)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (randomizer == 2)
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (randomizer == 3)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (randomizer == 4)
            {
                minerPos.X += 16;
                Dig();
            }
            GenerateNewMiner();
            lookFurther = 0;
        }
        public void StiltedMove()    // stilted, might work if more iterations of movement, sometimes longest tunnel
        {                                   // best water placer, there's another move that could be extracted from this if the ID segments were removed
            moveID = 0;
            if (Main.tileSolid[Main.tile[(int)(minerPos.X + 16 + (16 * lookFurther)) / 16, (int)minerPos.Y / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)(minerPos.X - 16 - (16 * lookFurther)) / 16, (int)minerPos.Y / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y + 16 + (16 * lookFurther)) / 16].type])
            {
                moveID++;
            }
            if (Main.tileSolid[Main.tile[(int)minerPos.X / 16, (int)(minerPos.Y - 16 - (16 * lookFurther)) / 16].type])
            {
                moveID++;
            }
            int randomizer = WorldGen.genRand.Next(0, moveID);
            size = WorldGen.genRand.Next(1, 3);
            if (randomizer == 0)
            {
                lookFurther++;
                int adjust = WorldGen.genRand.Next(1, 4);
                if (adjust == 1)
                {
                    minerPos.X -= 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 2)
                {
                    minerPos.X += 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 3)
                {
                    minerPos.Y -= 16 * 2;
                    PlaceWater();
                }
                else if (adjust == 4)
                {
                    minerPos.Y += 16 * 2;
                    PlaceWater();
                }
                return;
            }
            if (randomizer == 1 && WorldGen.genRand.Next(6) == 2)
            {
                minerPos.X -= 16;
                Dig();
            }
            if (randomizer == 2 && WorldGen.genRand.Next(10) == 4)
            {
                minerPos.Y -= 16;
                Dig();
            }
            if (randomizer == 3)
            {
                minerPos.Y += 16;
                Dig();
            }
            if (randomizer == 4 && WorldGen.genRand.Next(5) == 4)
            {
                minerPos.X += 16;
                Dig();
            }
            GenerateNewMiner();
            lookFurther = 0;
        }
        public void GenerateNewMiner()
        {
            int randomizer = WorldGen.genRand.Next(0, 100);
            if (randomizer < 20 && whoAmI < 800)
            {
                //  Codable.RunGlobalMethod("ModWorld", "miner.Init", new object[] { 0 });
                //  progressText = "Miner " + whoAmI + " created";
                whoAmI++;

                //  unecessary, jobCount takes care of new mining tasks
                //  miner.whoAmI = whoAmI;
                /*  int newMiner = NewMiner(minerPos.X, minerPos.Y, 0, whoAmI);
                    ID[newMiner].init = false;
                    ID[newMiner].Dig(); */
                //  miner.ID[newID].minerPos = Miner.minerPos;
            }
        }
        public void Dig()
        {
            if (type < 800 * 800)
            {
                type++;
                minePath[type] = minerPos;
            }

            if (!switchMode)
            {
                for (int k = 2; k < 24; k++)
                {
                    int i = (int)minerPos.X / 16;
                    int j = (int)minerPos.Y / 16;
                    Tile[] tiles = new Tile[]
                    {
                        Main.tile[i + k, j + k],
                        Main.tile[i - k, j - k],
                        Main.tile[i + k, j - k],
                        Main.tile[i - k, j + k]
                    };
                    foreach (Tile tile in tiles)
                    {
                        tile.type = ArchaeaWorld.magnoStone;
                        tile.active(true);
                    }
                    WorldGen.KillWall((int)minerPos.X / 16 + k, (int)minerPos.Y / 16 + k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 - k, (int)minerPos.Y / 16 - k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 + k, (int)minerPos.Y / 16 - k, false);
                    WorldGen.KillWall((int)minerPos.X / 16 - k, (int)minerPos.Y / 16 + k, false);
                }
            }
            if (switchMode)
            {
                for (int k = 0; k < type; k++)
                {
                    minerPos = minePath[k];
                    if (WorldGen.genRand.Next(60) == 0) PlaceWater();
                    if (size == 1)
                    {
                        int i = (int)minerPos.X / 16;
                        int j = (int)minerPos.Y / 16;
                        Tile[] tiles = new Tile[]
                        {
                            Main.tile[i + circumference, j + circumference],
                            Main.tile[i + circumference, j],
                            Main.tile[i, j + circumference],
                            Main.tile[i, j],
                            Main.tile[i + 1, j],
                            Main.tile[i - 1, j],
                            Main.tile[i, j + 1],
                            Main.tile[i, j - 1]
                        };
                        foreach (Tile tile in tiles)
                        {
                            tile.type = 0;
                            tile.active(false);
                        }
                    }
                    else if (size == 2)
                    {
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 2].active(false);
                    }
                    else if (size == 3)
                    {
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 3, (int)(minerPos.Y / 16) + circumference * 3].active(false);
                        Main.tile[(int)(minerPos.X / 16) + circumference * 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference * 3].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 1, (int)(minerPos.Y / 16) - 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 1, (int)(minerPos.Y / 16) + 1].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 2, (int)(minerPos.Y / 16) - 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 2, (int)(minerPos.Y / 16) + 2].active(false);
                        Main.tile[(int)(minerPos.X / 16) + 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16) - 3, (int)(minerPos.Y / 16)].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + 3].active(false);
                        Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) - 3].active(false);
                    }
                }
            }
        }
        public void PlaceWater()
        {
            int randomizer = WorldGen.genRand.Next(0, 100);
            if (randomizer < 8)
            { // old randomizer%12 == 0
                Main.tile[(int)(minerPos.X / 16) + circumference, (int)(minerPos.Y / 16)].liquid = 255;
                Main.tile[(int)(minerPos.X / 16), (int)(minerPos.Y / 16) + circumference].liquid = 255;
                WorldGen.SquareTileFrame((int)(minerPos.X / 16), (int)(minerPos.Y / 16));
            }
            if (circumference != 1) return;
        }
        public void Mine()
        {
            //	AverageMove();
            DirectionalMove();
            //	ToTheSurfaceMove();
            //	StiltedMove();
            //  used only in the removal of newly generated miners
            /*          if(russianRoulette){
                            int life = -5;
                            int death = 60;
                            int roulette = WorldGen.genRand.Next(life, death);
                            if(roulette == death && whoAmI > 0){
                                ID[whoAmI] = null;
                                whoAmI++;
                            }
                        }   */
            if (!switchMode && minerPos != Vector2.Zero && jobCount < jobCountMax/* && (minerPos.X < edge * 16 && minerPos.X > (rightBounds - edge) * 16 && minerPos.Y < edge * 16 && minerPos.Y > (bottomBounds - edge) * 16)*/)
            {
                int RandomX = WorldGen.genRand.Next(-2, 2);
                int RandomY = WorldGen.genRand.Next(-2, 2);
                if (RandomX != 0 && RandomY != 0)
                {
                    if (minerPos.Y / 16 > Main.maxTilesY / 3 && minerPos.Y < bottomBounds - edge)
                    {
                        mineBlock = new Vector2(mineBlockX * RandomX, mineBlockY * RandomY);
                        minerPos += mineBlock;
                    }
                    if (minerPos.Y / 16 < Main.maxTilesY / 3)
                    {
                        minerPos.Y += mineBlockY;
                    }
                    if (minerPos.Y / 16 > bottomBounds - edge)
                    {
                        minerPos.Y -= mineBlockY;
                    }
                }
                else return;
            }
        }
        public void Randomizer()
        {
            randomX = WorldGen.genRand.Next(edge, rightBounds - edge);
            randomY = WorldGen.genRand.Next((int)Main.rockLayer, bottomBounds - edge);
            for (int j = -1; j < 1; j++)
            {
                circumference = j;
            }
        }
        public void Terminate()
        {
            jobCount = jobCountMax;
            whoAmI = 800;
            ArchaeaWorld.miner.active = false;
        }
        public void Reset()
        {
            progressText = "";
            type = 0;
            whoAmI = 0;
            jobCount = 0;
            switchMode = false;
            init = false;
            center = new Vector2((Main.maxTilesX / 2) * 16, (Main.maxTilesY / 2) * 16);
            minerPos = center;
            oldMinerPos = default(Vector2);
            genPos[0] = default(Vector2);
            genPos[1] = default(Vector2);
            for (int i = 0; i < minePath.Length - 1; i++)
            {
                minePath[i] = default(Vector2);
            }
            if (Main.maxTilesX == 4200)
                jobCountMax = 32;
            if (Main.maxTilesX == 6400)
                jobCountMax = 48;
            if (Main.maxTilesX == 8400)
                jobCountMax = 64;
        }
    }
}
