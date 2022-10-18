using ItemChanger;
using System;
using System.Collections;
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
    internal record RecurringGeoCost(int geo) : GeoCost(geo)
    {
        public override void OnPay()
        {
            base.OnPay();
            // ItemChanger always sets Paid = true right after calling OnPay
            // and doesn't offer any hooks to let us override that behaviour.
            // So we work around it by just waiting a frame and then setting
            // Paid = false.
            GameManager.instance.StartCoroutine(Reset());
        }

        private IEnumerator Reset()
        {
            yield return null;
            Paid = false;
        }
    }
}
