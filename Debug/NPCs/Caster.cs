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
    public class Caster : ModNPC
    {
        public override bool Autoload(ref string name)
        {
            if (name == "Caster")
                return false;
            return true;
        }
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;
            npc.lifeMax = 50;
            npc.defense = 10;
            npc.damage = 20;
            npc.value = 5000;
            npc.alpha = 255;
            npc.lavaImmune = true;
        }

        public Pattern pattern = Pattern.JustSpawned;
        public bool hasAttacked;
        public int timer
        {
            get { return (int)npc.ai[0]; }
            set { npc.ai[0] = value; }
        }
        public int elapse = 180;
        public int attacks;
        public int maxAttacks
        {
            get { return Main.rand.Next(2, 6); }
        }
        public int dustType;
        public Vector2 move;
        public Player npcTarget
        {
            get { return Main.player[npc.target]; }
        }
        public virtual Player nearbyPlayer()
        {
            Player player = ArchaeaNPC.FindClosest(npc, false);
            if (player != null && player.active && !player.dead)
            {
                npc.target = player.whoAmI;
                return player;
            }
            else return npcTarget;
        }
        public override bool PreAI()
        {
            if (timer++ > elapse)
                timer = 0;
            switch (pattern)
            {
                case Pattern.JustSpawned:
                    if (JustSpawned())
                        goto case Pattern.FadeIn;
                    break;
                case Pattern.FadeIn:
                    pattern = Pattern.FadeIn;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        break;
                    }
                    else goto case Pattern.Idle;
                case Pattern.FadeOut:
                    pattern = Pattern.FadeOut;
                    if (npc.alpha < 255)
                    {
                        npc.alpha += 5;
                        break;
                    }
                    goto case Pattern.Teleport;
                case Pattern.Teleport:
                    pattern = Pattern.Teleport;
                    move = ArchaeaNPC.FastMove(npc);
                    if (move != Vector2.Zero)
                    {
                        npc.position = move;
                        Teleport();
                        hasAttacked = false;
                        goto case Pattern.FadeIn;
                    }
                    break;
                case Pattern.Idle:
                    pattern = Pattern.Idle;
                    if (timer % elapse == 0 && Main.rand.Next(3) == 0)
                    {
                        if (!hasAttacked)
                        {
                            hasAttacked = true;
                            goto case Pattern.Attack;
                        }
                        else goto case Pattern.FadeOut;
                    }
                    return false;
                case Pattern.Attack:
                    pattern = Pattern.Attack;
                    if (PreAttack())
                    {
                        Attack();
                        attacks++;
                    }
                    if (attacks > maxAttacks)
                    {
                        pattern = Pattern.Idle;
                        attacks = 0;
                    }
                    return true;
            }
            return false;
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            knockback = 0f;
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            knockback = 0f;
        }
        public override void AI()
        {
        }
        public virtual bool JustSpawned()
        {
            return true;
        }
        public virtual void Teleport()
        {

        }
        public virtual bool PreAttack()
        {
            return true;
        }
        public virtual void Attack()
        {
        }
    }
}
