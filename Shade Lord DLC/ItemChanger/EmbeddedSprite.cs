using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Shade_Lord_DLC
{
    internal class EmbeddedSprite : ISprite
    {
        public string key;

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => EmbeddedSprites.Get(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
