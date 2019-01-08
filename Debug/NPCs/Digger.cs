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

namespace ArchaeaMod_Debug.NPCs
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
            npc.width = 32;
            npc.height = 32;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 10;
            npc.value = 0;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
        }

        private bool begin;
        public bool direction;
        public virtual bool moveThroughAir
        {
            get { return true; }
        }
        public int bodyType;
        public int tailType;
        private int[] body;
        private float acc = 1f;
        private float accelerate;
        private float rotateTo;
        public virtual float leadSpeed
        {
            get { return 3f; }
        }
        public float followSpeed
        {
            get { return leadSpeed * 1.34f; }
        }
        public float turnSpeed
        {
            get { return leadSpeed / 60f; }
        }
        public float maxDistance
        {
            get { return leadSpeed * 534f; }
        }
        public Vector2 follow;
        public Rectangle bounds
        {
            get { return new Rectangle(-400, -300, 800, 600); }
        }
        protected bool Initialize()
        {
            if (!begin)
            {
                body = new int[8];
                body[0] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, bodyType, 0, npc.whoAmI);
                for (int k = 1; k < body.Length - 1; k++)
                    body[k] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, bodyType, 0, body[k - 1], npc.whoAmI);
                body[7] = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, tailType, 0, body[6], npc.whoAmI);
                npc.ai[0] = Main.npc[body[7]].whoAmI;
                for (int l = 0; l < body.Length; l++)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, body[l]);
                follow = FindAny(bounds);
                rotateTo = ArchaeaNPC.AngleTo(npc.Center, npc.Center + follow);
                begin = true;
            }
            return begin;
        }
        public override bool PreAI()
        {
            Initialize();
            direction = follow.X > npc.Center.X;
            if (!direction)
            {
                if (npc.rotation >= rotateTo * -1)
                    npc.rotation -= turnSpeed;
                if (npc.rotation <= rotateTo * -1)
                    npc.rotation += turnSpeed;
            }
            else
            {
                if (npc.rotation >= rotateTo)
                    npc.rotation -= turnSpeed;
                if (npc.rotation <= rotateTo)
                    npc.rotation += turnSpeed;
            }
            if (PreMovement() && Vector2.Distance(follow, npc.Center) > npc.width * 1.2f)
            {
                if (acc > 0.90f)
                {
                    if (!npc.Hitbox.Contains(follow.ToPoint()))
                        acc -= 0.01f;
                    else acc += 0.025f;
                }
                npc.Center = npc.Center + ArchaeaNPC.AngleToSpeed(npc.rotation, leadSpeed / acc);
            }
            PostMovement();
            return true;
        }
        public bool inGround
        {
            get
            {
                int x = (int)npc.position.X;
                int y = (int)npc.position.Y;
                for (int n = y; n < y + npc.height; n++)
                    for (int m = x; m < x + npc.width; m++)
                    {
                        int i = m / 16;
                        int j = n / 16;
                        if (!Inbounds(i, j))
                            return false;
                        Tile ground = Main.tile[i, j];
                        if (ground.active() && Main.tileSolid[ground.type])
                            return true;
                    }
                return false;
            }
        }
        public bool InGround(Vector2 position)
        {
            int i = (int)position.X / 16;
            int j = (int)position.Y / 16;
            if (!Inbounds(i, j))
                return false;
            Tile ground = Main.tile[i, j];
            if (ground.active() && Main.tileSolid[ground.type])
                return true;
            return false;
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
            if (!Inbounds() || (!inGround && !moveThroughAir))
            {
                NextDirection(new Vector2(0, npc.height * -1));
                return;
            }
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
                    if (npc.Distance(target().position) < range || (time++ > interval / 4 && time != 0))
                    {
                        time = 0;
                        move = FindAny(bounds);
                        goto case DigAround;
                    }
                    break;
                case DigAround:
                    ai = DigAround;
                    Digging();
                    if (time++ > interval / 2 && time != 0)
                    {
                        move = FindAny(bounds);
                        cycle++;
                        time = 0;
                    }
                    if (npc.Hitbox.Contains(follow.ToPoint()))
                        move = FindAny(bounds);
                    if (move != Vector2.Zero)
                    {
                        follow = move;
                        NextDirection(follow);
                    }
                    if (cycle > 2)
                    {
                        cycle = 0;
                        if (npc.Distance(target().position) < range)
                            goto case ChasePlayer;
                        else goto case Idle;
                    }
                    break;
                case ChasePlayer:
                    ai = ChasePlayer;
                    if (!ArchaeaNPC.defaultBounds(target()).Contains(npc.Hitbox))
                        rotateTo = direction ? ArchaeaNPC.AngleTo(npc, target()) : ArchaeaNPC.AngleTo(npc, target()) * -1;
                    if (time++ % interval / 2 == 0 && time != 0)
                        cycle++;
                    if (!InGround(npc.Center))
                        leaps++;
                    if (npc.Hitbox.Intersects(target().Hitbox) || cycle > 12 || leaps > 60)
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
            follow = chase;
            rotateTo = ArchaeaNPC.AngleTo(npc.Center, npc.Center + follow);
        }
        protected Vector2 FindAny(Rectangle check)
        {
            int x = Main.rand.Next(check.X, check.Right);
            int y = Main.rand.Next(check.Y, check.Bottom);
            return new Vector2(x, y);
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
        public static void DiggerPartsAI(NPC npc, NPC part, ref float acc)
        {
            npc.rotation = ArchaeaNPC.AngleTo(npc.Center, part.Center);
            if (Vector2.Distance(part.Center, npc.Center) > npc.width * 1.2f)
            {
                if (acc > 0.80f)
                {
                    if (!npc.Hitbox.Contains(part.position.ToPoint()))
                        acc -= 0.01f;
                    else acc += 0.05f;
                }
                npc.Center = npc.Center + ArchaeaNPC.AngleToSpeed(npc.rotation, digger.followSpeed / acc);
            }
        }
    }
}
