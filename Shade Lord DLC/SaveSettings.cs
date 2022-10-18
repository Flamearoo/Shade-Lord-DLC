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
        public CharmSettings charm1 = new();
    }
    public class Global_Settings
    {
        public int count = 0;
    }
}