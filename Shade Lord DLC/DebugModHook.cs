using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonoMod.RuntimeDetour;

namespace Shade_Lord_DLC
{
    internal static class DebugModHook
    {
        public static void GiveAllCharms(Action a)
        {
            var commands = Type.GetType("DebugMod.BindableFunctions, DebugMod");
            if (commands == null)
            {
                return;
            }
            var method = commands.GetMethod("GiveAllCharms", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return;
            }
            new Hook(
                method,
                (Action orig) => {
                    orig();
                    a();
                }
            );
        }
    }
}
