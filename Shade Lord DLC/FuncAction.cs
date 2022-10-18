using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;

namespace Shade_Lord_DLC
{
    internal class FuncAction : FsmStateAction
    {
        private readonly Action _func;

        public FuncAction(Action func)
        {
            _func = func;
        }

        public override void OnEnter()
        {
            _func();
            Finish();
        }
    }
}
