using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using UnityEngine;

namespace Shade_Lord_DLC
{
    internal abstract class Charm
    {
        public abstract string Sprite { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract int Cost { get; }
        public abstract string Room { get; }
        public abstract float X { get; }
        public abstract float Y { get; }

        public int Num { get; set; }

        public bool Equipped()
        {
            return PlayerData.instance.GetBool($"equippedCharm_{Num}");
        }

        public abstract CharmSettings Settings(Local_Settings s);

        public virtual void Hook() { }
        public virtual List<(string obj, string fsm, Action<PlayMakerFSM> edit)> FsmEdits => new();
        public virtual List<(int Period, Action Func)> Tickers => new();
    }
}
