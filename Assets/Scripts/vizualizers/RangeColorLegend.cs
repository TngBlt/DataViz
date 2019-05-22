using UnityEngine;

namespace DataViz_A4.Assets.Scripts.vizualizers
{
    public class RangeColorLegend
    {
        public Color firstColor;
        public Color secondColor;
        public float firstValue;
        public float secondValue;

        public RangeColorLegend(Color frstColor,Color scndColor, float frstValue, float scndValue) {
            firstColor = frstColor;
            secondColor = scndColor;
            firstValue = frstValue;
            secondValue = scndValue;
        }
    }
}