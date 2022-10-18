using MenuChanger.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shade_Lord_DLC
{
    public class LogicSettings
    {
        public bool AntigravityAmulet;
        public GeoCharmLogicMode BluemothWings;
        [MenuLabel("Lemm's Strength")]
        public bool LemmsStrength;
        public int MinimumRelicsRequired;
        [MenuLabel("Florist's Blessing")]
        public bool FloristsBlessing;
        public bool SnailSoul;
        public bool SnailSlash;
        public bool Greedsong;
        [MenuLabel("Millibelle's Blessing")]
        public bool MillibellesBlessing;
        public bool NitroCrystal;
        public GeoCharmLogicMode Crystalmaster;
        [MenuLabel("Marissa's Audience")]
        public bool MarissasAudience;

        public enum GeoCharmLogicMode
        {
            Off,
            OnWithGeo,
            On
        }
    }
}
