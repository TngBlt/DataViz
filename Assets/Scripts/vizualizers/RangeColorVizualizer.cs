using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace vizualizers {

    class RangeColorVizualizer : IVizualizer<float,Color> {

        private Color startColor;
        private Color endColor;
        private float minValue;
        private float maxValue;

        public RangeColorVizualizer(IEnumerable<float> data, Color startColor, Color endColor){
            this.endColor = endColor;
            this.startColor = startColor;
            maxValue = data.Max();
            minValue = data.Min();
        }       

        public Color getVizualization(float input){
            return Color.Lerp(startColor, endColor, (input-minValue)/(maxValue-minValue));
        }

    }

}