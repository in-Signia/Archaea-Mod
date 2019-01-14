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

namespace ArchaeaMod_Debug.Items
{
    public class Blackened : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Staff");
            Tooltip.SetDefault("Casts fire wave");
        }
        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = 0;
            item.mana = 2;
            item.value = 5000;
            item.rare = ItemRarityID.Green;
            item.useTime = 20;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.channel = true;
            item.magic = true;
        }

        private int time;
        private int second = 60;
        private int elapsed
        {
            get { return (int)(second * 1.5f); }
        }
        private int maxTime
        {
            get { return elapsed * 5; }
        }
        private int manaCost
        {
            get { return second / item.useTime; }
        }
        private bool update = true;
        private int index;
        private int type = -1;
        public const int
            Reset = -1,
            Start = 0,
            Boost = 1,
            Hover = 2,
            Launch = 3;
        private float alpha;
        private Dust[] dust = new Dust[5];
        public Target[] targets;
        public override bool UseItem(Player player)
        {
            item.mana = manaCost;
            if (update)
            {
                targets = GetTargets(player).Where(t => t != null).ToArray();
                Debug.BasicInfo(targets.Length.ToString());
                update = false;
            }
            return true;
        }
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (time++ % elapsed == 0 && time != 0)
            {
                update = true;
                if (index < 5)
                    index++;
                else ResetItem();
            }
            for (int i = 0; i < index; i++)
            {
                dust[i] = Dust.NewDustDirect(player.Center - new Vector2(25f, 32f) + new Vector2(i * 10f, (float)Math.Cos(i / (Math.PI / 2f))), 1, 1, DustID.Fire, 0f, 0f, 0, default(Color), 2f);
                dust[i].noGravity = true;
            }
            if (targets == null)
                return;
            foreach (Target target in targets)
            {
                if (target.npc.Distance(player.Center) < 600f)
                {
                    switch (type)
                    {
                        case Reset:
                            ResetItem();
                            break;
                        case Start:
                            alpha = 0f;
                            if (index == 1)
                                goto case Boost;
                            break;
                        case Boost:
                            type = Boost;
                            if (player.releaseUseItem)
                            {
                                target.AttackEffect(type);
                                goto case Reset;
                            }
                            if (index == 3)
                                goto case Hover;
                            break;
                        case Hover:
                            type = Hover;
                            if (player.releaseUseItem)
                                target.hover = true;
                            if (index == 5)
                                goto case Launch;
                            break;
                        case Launch:
                            target.AttackEffect(Launch);
                            BlastWave(player);
                            goto case Reset;
                    }
                }
            }
        }
        public override void UseStyle(Player player)
        {
            ArchaeaItem.ActiveChannelStyle(player);
        }
        public void BlastWave(Player player)
        {
            for (float r = 0f; r < Math.PI; r += 0.017f * 9f)
            {
                Vector2 velocity = NPCs.ArchaeaNPC.AngleToSpeed(r, 12f);
                Dust dust = Dust.NewDustDirect(player.Center, 1, 1, DustID.Fire, velocity.X, velocity.Y, 0, default(Color), 2f);
                dust.position += velocity;
                dust.noGravity = true;
            }
        }
        protected void ResetItem()
        {
            dust = new Dust[5];
            time = 0;
            index = 0;
            type = 0;
            alpha = 0f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            alpha++;
            lightColor = Color.Gold * Math.Min(alpha / maxTime, 1f);
            return Color.White;
        }
        protected Target[] GetTargets(Player player)
        {
            Target[] targets = new Target[255];
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Shield.GetOnCondition(Main.npc[i])) continue;
                targets[i] = new Target(Main.npc[i], player);
            }
            return targets;
        }
    }
    public class Target : GlobalNPC
    {
        public bool hover;
        private bool strikeGround;
        private int time = 180;
        private int height
        {
            get { return npc.height / 16; }
        }
        private const float speed = 0.655f;
        public NPC npc;
        public Player player;
        public Target(NPC npc, Player player)
        {
            this.npc = npc;
            this.player = player;
        }
        public override void AI(NPC npc)
        {
            if (npc.Equals(this.npc))
            {
                Main.NewText(npc);
                if (hover)
                    AttackEffect(Blackened.Hover);
                if (strikeGround)
                {
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height + 16))
                    {
                        npc.StrikeNPC(20, 0f, npc.direction * -1);
                        strikeGround = false;
                    }
                }
            }
        }
        public void AttackEffect(int type)
        {
            switch (type)
            {
                case Blackened.Boost:
                    npc.velocity.Y -= speed * 5f;
                    break;
                case Blackened.Hover:
                    if (time-- > 0)
                    {
                        Vector2 ground = FindGround();
                        if (npc.position.Y > ground.Y + 50f)
                        {
                            npc.velocity.Y -= speed;
                            NPCs.ArchaeaNPC.VelocityClamp(npc, -3f, 3f);
                        }
                    }
                    else
                    {
                        npc.velocity.Y += 2f;
                        npc.velocity.Y *= 6f;
                        hover = false;
                        strikeGround = true;
                        time = 180;
                    }
                    break;
                case Blackened.Launch:
                    Main.NewText(type);
                    float angle = NPCs.ArchaeaNPC.AngleTo(player.Center, npc.Center);
                    npc.velocity.Y -= 8f;
                    npc.velocity += NPCs.ArchaeaNPC.AngleToSpeed(angle, 12f);
                    NPCs.ArchaeaNPC.VelocityClamp(npc, -10f, 10f);
                    break;
            }
        }
        protected Vector2 FindGround()
        {
            return Collision.TileCollision(new Vector2(npc.position.X, npc.position.Y + npc.height), Vector2.Zero, npc.width, npc.height);
        }
    }
}
