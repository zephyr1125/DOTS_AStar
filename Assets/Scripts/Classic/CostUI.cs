using System;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Classic
{
    public class CostUI : MonoBehaviour
    {
        public Text text;
        public Map map;

        public void UpdateCosts()
        {
            StringBuilder sb = new StringBuilder();
            for (var j = map.mapSize.y - 1; j >= 0; j--)
            {
                for (var i = 0; i < map.mapSize.x; i++)
                {
//                    sb.Append(i);
//                    sb.Append(',');
//                    sb.Append(j);
//                    sb.Append('|');
                    var costString = String.Format("{0,6:D2}", map.map[new int2(i, j)].costCount);
                    sb.Append(costString);
                }
            }

            text.text = sb.ToString();
        }
    }
}