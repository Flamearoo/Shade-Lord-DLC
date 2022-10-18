using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shade_Lord_DLC
{
    public class RandoSettings
    {
        public bool AddCharms;

        [MenuChanger.Attributes.MenuRange(0, 14)]
        public int IncreaseMaxCharmCostBy;

        public LogicSettings Logic = new();

        public RandoSettings(Global_Settings rs)
        {
            AddCharms = rs.AddCharms;
            IncreaseMaxCharmCostBy = rs.IncreaseMaxCharmCostBy;
            if (Modding.ModHooks.GetMod("MenuChanger") != null)
            {
                Logic = RandomizerMod.RandomizerData.JsonUtil.DeserializeString<LogicSettings>(rs.LogicSettings);
            }
        }

        public bool Enabled() => AddCharms;
    }
}
