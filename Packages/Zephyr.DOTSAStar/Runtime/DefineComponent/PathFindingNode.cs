using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Zephyr.Define.Runtime;

namespace Zephyr.DOTSAStar.Runtime.DefineComponent
{
    [InfoBox("Input name and cost, use -1 as cost means obstacle")]
    public class PathFindingNode : ComponentBase
    {
        public string Name;
        public float Cost;
    }
}