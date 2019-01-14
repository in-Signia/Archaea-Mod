using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod
{
    public class ModNPCID
    {
        protected static Mod getMod
        {
            get { return ModLoader.GetMod("ArchaeaMod_Debug"); }
        }
        public static int ItchySlime
        {
            get { return getMod.NPCType<NPCs.Slime_Itchy>(); }
        }
        public static int MercurialSlime
        {
            get { return getMod.NPCType<NPCs.Slime_Mercurial>(); }
        }
        public static int Mimic
        {
            get { return getMod.NPCType<NPCs.Mimic>(); }
        }
        public static int Fanatic
        {
            get { return getMod.NPCType<NPCs.Fanatic>(); }
        }
        public static int Hatchling
        {
            get { return getMod.NPCType<NPCs.Hatchling_head>(); }
        }
        public static int Observer
        {
            get { return getMod.NPCType<NPCs.Sky_1>(); }
        }
        public static int Marauder
        {
            get { return getMod.NPCType<NPCs.Sky_2>(); }
        }
        public static int MagnoliacHead
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_head>(); }
        }
        public static int MagnoliacBody
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_body>(); }
        }
        public static int MagnoliacTail
        {
            get { return getMod.NPCType<NPCs.Bosses.Magnoliac_tail>(); }
        }
        public static int SkyBoss
        {
            get { return getMod.NPCType<NPCs.Bosses.Sky_boss>(); }
        }
    }
}

namespace ArchaeaMod.NPCs
{
    public enum Pattern
    {
        JustSpawned,
        Idle,
        Active,
        Attack,
        Teleport,
        FadeIn,
        FadeOut
    }
    public class _GlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
        }
        public override bool CheckActive(NPC npc)
        {
            if (npc.TypeName.Contains("Sky"))
                return true;
            return base.CheckActive(npc);
        }
    }
    public class ArchaeaNPC
    {
        public static int defaultWidth = 800;
        public static int defaultHeight = 600;
        public static Rectangle defaultBounds(NPC npc)
        {
            return new Rectangle((int)npc.position.X - defaultWidth / 2, (int)npc.position.Y - defaultHeight / 2, defaultWidth, defaultHeight);
        }
        public static Rectangle defaultBounds(Player player)
        {
            return new Rectangle((int)player.position.X - defaultWidth / 2, (int)player.position.Y - defaultHeight / 2, defaultWidth, defaultHeight);
        }
        public static Vector2 FastMove(NPC npc)
        {
            return FindGround(npc, defaultBounds(npc));
        }
        public static Vector2 FastMove(Player player)
        {
            return FindGround(player, defaultBounds(player));
        }
        public static bool TargetBasedMove(NPC npc, Player target, bool playerRange = false)
        {
            int width = Main.screenWidth - 100;
            int height = Main.screenHeight - 100;
            Vector2 old = npc.position;
            Vector2 vector;
            if (target == null)
                return false;
            vector = FindGround(npc, new Rectangle((int)target.position.X - width / 2, (int)target.position.Y - height / 2, width, height));
            if (!MagnoDen.Inbounds((int)vector.X / 16, (int)vector.Y / 16))
                return false;
            if (vector != Vector2.Zero)
                npc.position = vector;
            if (old != npc.position)
                return true;
            return false;
        }
        public static Vector2 FindGround(NPC npc, Rectangle bounds)
        {
            var vector = FindEmptyRegion(npc, bounds);
            for (int k = 0; k < 5; k++)
            {
                int i = (int)vector.X / 16;
                int j = (int)(vector.Y + npc.height + 8) / 16;
                if (!MagnoDen.Inbounds(i, j))
                    continue;
                int count = 0;
                int max = npc.width / 16;
                for (int l = 0; l < npc.width / 16; l++)
                {
                    Tile ground = Main.tile[i + l, j + 1];
                    if (ground.active() && Main.tileSolid[ground.type])
                        count++;
                }
                while (vector.Y + npc.height < j * 16)
                    vector.Y++;
                if (Collision.SolidCollision(vector, npc.width - 4, npc.height - 4))
                    return Vector2.Zero;
                if (count == max)
                    return vector;
            }
            return Vector2.Zero;
        }
        public static Vector2 FindGround(Player player, Rectangle bounds)
        {
            var vector = FindEmptyRegion(player, bounds);
            for (int k = 0; k < 5; k++)
            {
                int i = (int)vector.X / 16;
                int j = (int)(vector.Y + player.height + 8) / 16;
                if (!MagnoDen.Inbounds(i, j))
                    continue;
                int count = 0;
                int max = player.width / 16;
                for (int l = 0; l < player.width / 16; l++)
                {
                    Tile ground = Main.tile[i + l, j + 1];
                    if (ground.active() && Main.tileSolid[ground.type])
                        count++;
                }
                while (vector.Y + player.height < j * 16)
                    vector.Y++;
                if (Collision.SolidCollision(vector, player.width - 4, player.height - 4))
                    return Vector2.Zero;
                if (count == max)
                    return vector;
            }
            return Vector2.Zero;
        }
        public static Vector2 FindEmptyRegion(NPC npc, Rectangle check)
        {
            int x = Main.rand.Next(check.X, check.Right);
            int y = Main.rand.Next(check.Y, check.Bottom);
            int tile = 18;
            for (int n = npc.height + tile; n >= 0; n--)
                for (int m = 0; m < npc.width + tile; m++)
                {
                    int i = (x + m) / 16;
                    int j = (y + n) / 16;
                    if (Collision.SolidCollision(new Vector2(x + m, y + n), npc.width, npc.height))
                        return Vector2.Zero;
                    return new Vector2(x, y);
                }
            return Vector2.Zero;
        }
        public static Vector2 FindEmptyRegion(Player player, Rectangle check)
        {
            int x = Main.rand.Next(check.X, check.Right);
            int y = Main.rand.Next(check.Y, check.Bottom);
            int tile = 18;
            for (int n = player.height + tile; n >= 0; n--)
                for (int m = 0; m < player.width + tile; m++)
                {
                    int i = (x + m) / 16;
                    int j = (y + n) / 16;
                    if (Collision.SolidCollision(new Vector2(x + m, y + n), player.width, player.height))
                        return Vector2.Zero;
                    return new Vector2(x, y);
                }
            return Vector2.Zero;
        }
        public static bool WithinRange(Vector2 position, Rectangle range)
        {
            return range.Contains(position.ToPoint());
        }
        protected static Rectangle Range(Vector2 position, int width, int height)
        {
            return new Rectangle((int)position.X - width / 2, (int)position.Y - width / 2, width, height);
        }
        public static Player FindClosest(NPC npc, bool unlimited = false, float range = 300f)
        {
            int[] indices = new int[Main.player.Length];
            if (!unlimited)
            {
                foreach (Player target in Main.player)
                    if (target.active)
                        if (npc.Distance(target.position) < range)
                            return target;
            }
            else
            {
                int count = 0;
                for (int i = 0; i < Main.player.Length; i++)
                    if (Main.player[i].active)
                        indices[count] = Main.player[i].whoAmI;
                float[] distance = new float[indices.Length];
                for (int k = 0; k < indices.Length; k++)
                    distance[k] = Vector2.Distance(Main.player[k].position, npc.position);
                return Main.player[indices[distance.ToList().IndexOf(distance.Min())]];
            }
            return null;
        }

        public static Vector2 AngleToSpeed(float angle, float amount = 2f)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static Vector2 AngleBased(Vector2 position, float angle, float radius)
        {
            float cos = position.X + (float)(radius * Math.Cos(angle));
            float sine = position.Y + (float)(radius * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static float RandAngle()
        {
            return Main.rand.NextFloat((float)(Math.PI * 2d));
        }
        public static float AngleTo(NPC from, Player to)
        {
            return (float)Math.Atan2(to.position.Y - from.position.Y, to.position.X - from.position.X);
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static Dust[] DustSpread(Vector2 v, int dustType = 6, int total = 10, float scale = 1f)
        {
            Dust[] dusts = new Dust[total];
            for (int k = 0; k < total; k++)
            {
                Vector2 speed = ArchaeaNPC.AngleToSpeed(ArchaeaNPC.RandAngle(), 8f);
                dusts[k] = Dust.NewDustDirect(v + speed, 1, 1, dustType, speed.X, speed.Y, 0, default(Color), 2f);
            }
            return dusts;
        }
        public static bool OnHurt(int life, int oldLife, out int newLife)
        {
            if (life < oldLife)
            {
                newLife = life;
                return true;
            }
            newLife = life;
            return false;
        }
        
        public static void PositionToVel(NPC npc, Vector2 change, float speedX, float speedY, bool clamp = false, float min = -2.5f, float max = 2.5f, bool wobble = false, double degree = 0f)
        {
            float cos = wobble ? (float)(0.05f * Math.Cos(degree)) : 0f;
            float sine = wobble ? (float)(0.05f * Math.Sin(degree)) : 0f;
            if (clamp)
                VelocityClamp(npc, min, max);
            if (npc.position.X < change.X)
                npc.velocity.X += speedX + cos;
            if (npc.position.X > change.X)
                npc.velocity.X -= speedX + cos;
            if (npc.position.Y < change.Y)
                npc.velocity.Y += speedY + sine;
            if (npc.position.Y > change.Y)
                npc.velocity.Y -= speedY + sine;
        }
        public static void VelocityClamp(NPC npc, float min, float max)
        {
            if (npc.velocity.X < min)
                npc.velocity.X = min;
            if (npc.velocity.X > max)
                npc.velocity.X = max;
            if (npc.velocity.Y < min)
                npc.velocity.Y = min;
            if (npc.velocity.Y > max)
                npc.velocity.Y = max;
        }
        public static void VelClampX(NPC npc, float min, float max)
        {
            if (npc.velocity.X < min)
                npc.velocity.X = min;
            if (npc.velocity.X > max)
                npc.velocity.X = max;
        }
        public static void RotateIncrement(bool direction, ref float from, float to, float speed, out float result)
        {
            if (!direction)
            {
                if (from >= to * -1)
                    from -= speed;
                if (from <= to * -1)
                    from += speed;
            }
            else
            {
                if (from >= to)
                    from -= speed;
                if (from <= to)
                    from += speed;
            }
            result = from;
        }

        protected static bool SolidGround(Tile[] tiles)
        {
            int count = 0;
            foreach (Tile ground in tiles)
                if (!NotActiveOrSolid(ground))
                {
                    count++;
                    if (count == tiles.Length)
                        return true;
                }
            return false;
        }
        protected static bool NotActiveOrSolid(int i, int j)
        {
            return (!Main.tile[i, j].active() && Main.tileSolid[Main.tile[i, j].type]) || (Main.tile[i, j].active() && !Main.tileSolid[Main.tile[i, j].type]);
        }
        protected static bool NotActiveOrSolid(Tile tile)
        {
            return (!tile.active() && Main.tileSolid[tile.type]) || (tile.active() && !Main.tileSolid[tile.type]);
        }

        #region out of view
        /*
        int add = 30;
        int count = 0;
        int max = 3;
        List<Vector2> vectors = new List<Vector2>();
        Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        while (count < max)
        {
            check.Inflate(add, add);
            if (check.Width > screen.Width + 30)
                break;
            count++;
        }
        for (int i = check.Left; i < check.Right; i++)
            for (int j = check.Left; j < check.Right; i++)
                if (!WithinRange(new Vector2(i, j), screen) && MagnoDen.Inbounds(i, j))
                    vectors.Add(new Vector2(i, j));
        if (vectors.Count > 0)
            return vectors.ToArray();
        return new Vector2[] { Vector2.Zero };*/
        #endregion
        #region depracated
        /*if (!MagnoDen.Inbounds(x, y))
        {
            npc.active = false;
            return Vector2.Zero;
        }
        int count = 0;
        int max = npc.width / 16 * (npc.height / 16);
        for (int l = 0; l < npc.height; l++)
            for (int k = 0; k < npc.width; k++)
            {
                int i = (x + k) / 16;
                int j = (y + l - npc.height) / 16;
                if (!MagnoDen.Inbounds(i, j))
                    continue;
                Tile tile = Main.tile[i, j];
                if (NotActiveOrSolid(tile))
                    count++;
                else
                {
                    count = 0;
                    break;
                }
                if (k == 0 && l == npc.height - 1)
                    for (int m = 0; m < npc.width / 16; m++)
                    {
                        Tile ground = Main.tile[i + m, j + 1];
                        if (ground.active() && Main.tileSolid[ground.type])
                            count++;
                    }
                if (count == max + npc.width / 16)
                    return new Vector2(x, y);
            }
        return new Vector2(rangeOut, rangeOut);*/
        #endregion
        #region Depracated SpawnOnGround
        /*
        Tile[] ground = new Tile[npc.width / 16];
        Vector2 spawn = FindNewPosition(npc, bounds);
        int i = (int)(spawn.X / 16);
        int j = (int)(spawn.Y + npc.height) / 16;
        int count = 0;
        for (int k = 0; k < ground.Length; k++)
        {
            ground[k] = Main.tile[i + k, j + 1];
            if (ground[k].active() && Main.tileSolid[ground[k].type])
            {
                count++;
                if (count == ground.Length)
                    return spawn;
            }
        }
        return Vector2.Zero;*/
        #endregion
    }
}
