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
        public List<bool> gotCharms = new List<bool>() { true, true, true, true, true, true, true, true, true, true };
        public List<bool> newCharms = new List<bool>() { false, false, false, false, false, false, false, false, false, false };
        public List<bool> equippedCharms = new List<bool>() { false, false, false, false, false, false, false, false, false, false };
        public List<int> charmCosts = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    }
    public class Global_Settings
    {
        public int count = 0;
    }
}