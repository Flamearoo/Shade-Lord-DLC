using ItemChanger;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shade_Lord_DLC
{
    internal class FightersPride : Charm
    {
        public static readonly FightersPride Instance = new();

        private FightersPride() { }

        public override string Sprite => "FightersPride.png";
        public override string Name => "Fighter's Pride";
        public override string Description => "A charm containing the power of those who are yet to fall.\n\nIncreases your damage dramatically when at full hp.";
        public override int DefaultCost => 2;
        public override string Room => "Ruins1_27";
        public override float X => 53.6f;
        public override float Y => 23.4f;
        public override CharmSettings cSL(Local_Settings s) => s.FightersPride;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
        }

        private int BuffNail(string intName, int damage)
        {
            if (intName == "nailDamage" && Equipped())
            {
                damage *= 2;
            }
            return damage;
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
