using System;
using System.Text;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Zephyr.DOTSAStar.Core;

namespace Zephyr.DOTSAStar.Example
{
    public class CostUI : MonoBehaviour
    {
        public TMP_Text text;
        public Map map;

        public void UpdateCosts()
        {
            StringBuilder sb = new StringBuilder();
            for (var j = Const.MapHeight - 1; j >= 0; j--)
            {
                for (var i = 0; i < Const.MapWidth; i++)
                {
//                    var costString = String.Format("{0,6:D2}", map.map[new int2(i, j)].costCount);
//                    sb.Append(costString);
//                    if (i == Const.MapWidth - 1) sb.Append("\n");
                }
            }

            text.text = sb.ToString();
        }
    }
}