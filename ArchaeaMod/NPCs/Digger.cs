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

namespace ArchaeaMod.NPCs
{
    public class Digger : ModNPC
    {
        public static Digger digger;
        public override bool Autoload(ref string name)
        {
            digger = this;
            if (name == "Digger")
                return false;
            return true;
        }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
        }

        private bool begin;
        public bool direction;
        private bool hyper
        {
            get { return npc.life < npc.lifeMax / 3.5f; }
        }
        public virtual bool moveThroughAir
        {
            get { return true; }
            set { }
        }
        public int bodyType;
        public int tailType;
        public virtual int totalParts
        {
            get { return 8; }
        }
        private int[] body;
        private float acc = 1f;
        private float accelerate;
        private float rotateTo;
        public virtual float leadSpeed
        {
            get { return 3f; }
        }
        public virtual float followSpeed
        {
            get { return leadSpeed * 2f; }
        }
        public float turnSpeed
        {
            get { return leadSpeed / 60f; }
        }
        public float maxDistance
        {
            get { return leadSpeed * 534f; }
        }
        private float rotate;
        public Vector2 follow;
        public Rectangle bounds
        {
            get { return new Rectangle(-400, -300, 800, 600); }
        }
        public virtual void Initialize()
        {
        }
        protected void SpawnParts()
        {
            body = new int[totalParts];
            body[0] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, bodyType, 0, npc.whoAmI);
            for (int k = 1; k < body.Length - 1; k++)
                body[k] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, bodyType, 0, body[k - 1], npc.whoAmI);
            body[totalParts - 1] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, tailType, 0, body[totalParts - 2], npc.whoAmI);
            npc.ai[0] = Main.npc[body[totalParts - 1]].whoAmI;
            for (int l = 0; l < body.Length; l++)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, body[l]);
            NextDirection(FindAny(bounds));
            rotateTo = ArchaeaNPC.AngleTo(npc.Center, npc.Center + follow);
            rotate = npc.rotation;
        }
        public override bool PreAI()
        {
            if (!begin)
            {
                Initialize();
                SpawnParts();
                begin = true;
            }
            direction = follow.X > npc.Center.X;
            rotate = WrapAngle(ref rotate);
            ArchaeaNPC.RotateIncrement(true, ref rotate, rotateTo, turnSpeed, out npc.rotation);
            if (PreMovement() && Vector2.Distance(follow, npc.Center) > npc.width * 1.2f)
            {
                if (acc > 0.90f)
                {
                    if (!npc.Hitbox.Contains(follow.ToPoint()))
                        acc -= 0.01f;
                    else acc += 0.025f;
                }
                npc.Center += ArchaeaNPC.AngleToSpeed(npc.rotation, leadSpeed / acc);
            }
            PostMovement();
            return true;
        }
        public bool inGround(Vector2 position)
        {
            int i = (int)position.X / 16;
            int j = (int)position.Y / 16;
            if (!Inbounds(i, j))
                return false;
            Tile ground = Main.tile[i, j];
            if (ground.active() && Main.tileSolid[ground.type])
                return true;
            else return false;
        }
        public bool inRange
        {
            get { return npc.Distance(target().position) < range; }
        }
        internal int ai = -1;
        private int cycle;
        internal int time;
        public int interval = 180;
        public const int
            Reset = -2,
            JustSpawned = -1,
            Idle = 1,
            DigAround = 2,
            ChasePlayer = 3,
            Airborne = 4;
        private int leaps;
        internal float range
        {
            get { return maxDistance; }
        }
        internal Vector2 move;
        public Player target()
        {
            Player player = ArchaeaNPC.FindClosest(npc, true);
            if (player != null && npc.Distance(player.position) < range)
            {
                npc.target = player.whoAmI;
                return player;
            }
            else return Main.player[npc.target];
        }

        public override void AI()
        {
            if (follow == default(Vector2))
                follow = target().Center;
            switch (ai)
            {
                case Reset:
                    NextDirection(FindAny(bounds));
                    cycle = 0;
                    leaps = 0;
                    break;
                case JustSpawned:
                    if (StartDigging())
                        goto case DigAround;
                    break;
                case Idle:
                    ai = Idle;
                    if (npc.Distance(target().position) < range / 2f && time++ > interval / 4 && time != 0)
                    {
                        time = 0;
                        move = FindAny(bounds);
                        goto case DigAround;
                    }
                    break;
                case DigAround:
                    ai = DigAround;
                    Digging();
                    if (LookForGround())
                        return;
                    if (time++ > interval && time != 0)
                    {
                        move = FindAny(bounds);
                        cycle++;
                        time = 0;
                    }
                    if (npc.Hitbox.Contains(follow.ToPoint()))
                        move = FindAny(bounds);
                    if (move != Vector2.Zero)
                        NextDirection(move);
                    if (cycle > 2)
                    {
                        cycle = 0;
                        goto case ChasePlayer;
                    }
                    break;
                case ChasePlayer:
                    ai = ChasePlayer;
                    if (npc.Distance(target().Center) > 100f)
                        rotateTo = npc.AngleTo(target().Center);
                    if (time++ % interval / 2 == 0 && time != 0)
                        cycle++;
                    if (!inGround(npc.Center))
                        leaps++;
                    if (npc.Hitbox.Intersects(target().Hitbox) || cycle > 8 || (!moveThroughAir && leaps > 100))
                    {
                        ai = DigAround;
                        goto case Reset;
                    }
                    break;
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Lighting.AddLight(npc.Center, new Vector3(1f, 0.8f, 0.6f));
            drawColor = Color.White;
            return true;
        }
        protected void NextDirection(Vector2 chase)
        {
            follow = chase + npc.position;
            rotateTo = ArchaeaNPC.AngleTo(npc.Center, follow);
        }
        protected Vector2 FindAny(Rectangle check)
        {
            int x = Main.rand.Next(check.Width * -1, check.Width);
            int y = Main.rand.Next(check.Height * -1, check.Height);
            return new Vector2(x, y);
        }
        private bool inAir;
        protected bool LookForGround()
        {
            if (!Inbounds() || (!inGround(npc.Center) && !moveThroughAir))
            {
                rotateTo = (float)Math.PI / 2f;
                inAir = true;
                return true;
            }
            else if (inAir)
            {
                NextDirection(FindAny(bounds));
                inAir = false;
            }
            return false;
        }
        public bool Elapsed(int interval)
        {
            return Math.Round(Main.time, 0) % interval == 0;
        }
        internal bool Inbounds(int i, int j)
        {
            return i < Main.maxTilesX - 30 && i > 500 / 16 && j < Main.maxTilesY - 30 && j > 500 / 16;
        }
        internal bool Inbounds()
        {
            int i = (int)npc.Center.X / 16;
            int j = (int)npc.Center.Y / 16;
            return i < Main.maxTilesX - 30 && i > 500 / 16 && j < Main.maxTilesY - 30 && j > 500 / 16;
        }
        public static void Clamp(float input, float min, float max, out float result)
        {
            if (input < min)
                input = min;
            if (input > max)
                input = max;
            result = input;
        }
        public virtual bool PreMovement()
        {
            return true;
        }
        public virtual void PostMovement()
        {

        }
        public virtual bool StartDigging()
        {
            return true;
        }
        public virtual void Digging()
        {

        }
        public static void DiggerPartsAI(NPC npc, NPC part, float speed, ref float acc)
        {
            Vector2 connect = ArchaeaNPC.AngleBased(new Vector2(part.position.X, part.position.Y + part.height / 2), part.rotation, part.width);
            npc.rotation = ArchaeaNPC.AngleTo(npc.Center, part.Center);
            if (Vector2.Distance(part.Center, npc.Center) > npc.width * 1.2f)
            {
                if (!npc.Hitbox.Contains(connect.ToPoint()))
                    acc = 0.30f;
                else acc += 0.01f;
                Clamp(acc, 0.3f, 1f, out acc);
                npc.Center += ArchaeaNPC.AngleToSpeed(npc.rotation, speed * acc);
            }
        }
        public float WrapAngle(ref float angle)
        {
            if (angle > Math.PI)
                angle = (float)Math.PI;
            if (angle < Math.PI * -1)
                angle = (float)Math.PI * -1;
            return angle;
        }
    }
}
