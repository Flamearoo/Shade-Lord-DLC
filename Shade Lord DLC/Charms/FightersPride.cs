using HutongGames.PlayMaker.Actions;
using ItemChanger;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

namespace Shade_Lord_DLC
{
    internal class FightersPride : Charm
    {
        public static readonly FightersPride Instance = new();

        private FightersPride() { }

        public override string Sprite => "FightersPride.png";
        public override string Name => "Fighter's Pride";
        public override string Description => "A charm containing the power of those who are yet to fall.\n\nIncreases the bearers damage dramatically when at full health.";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins1_27";
        public override float X => 53.6f;
        public override float Y => 23.4f;
        public override CharmSettings cSL(Local_Settings s) => s.FightersPride;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.GetPlayerIntHook += HealthCheck;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
        }

        private int BuffNail(string intName, int damage)
        {
            if (intName == "nailDamage" && Equipped() && PlayerData.instance.health == PlayerData.instance.maxHealth)
            {
                damage = (int)Math.Floor(damage * 2.0f);
            }
            return damage;
        }

        private int HealthCheck(string intName, int health)
        {
            if (intName == "health" && Equipped() && PlayerData.instance.health == PlayerData.instance.maxHealth)
            {
                Shade_Lord_DLC.UpdateNailDamage();
            }
            return health;
        }

        private bool UpdateNailDamageOnEquip(string boolName, bool value)
        {
            if (boolName == $"equippedCharm_{Num}")
            {
                Shade_Lord_DLC.UpdateNailDamage();
            }
            return value;
        }
    }
}
