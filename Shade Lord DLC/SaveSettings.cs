using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using UnityEngine;

namespace Shade_Lord_DLC
{
    public class Local_Settings
    {
        public CharmSettings FightersPride = new();
    }
    public class Global_Settings
    {
        public bool AddCharms = true;
        public int IncreaseMaxCharmCostBy = 7;
        public string LogicSettings = "{}";
    }
}