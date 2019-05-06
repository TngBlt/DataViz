using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace vizualizers {

    class HeightVizualizer : IVizualizer<float,float> {

        private float maxHeight;
        private float minHeight;
        private float maxDataValue;
        private float minDataValue;

        public HeightVizualizer(IEnumerable<float> data, float maxHeight, float minHeight = 0){
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
            maxDataValue = data.Max();
            minDataValue = data.Min();
        }

        public float getVizualization(float input){
            return minHeight + ( (input * maxHeight) / maxDataValue);
        }

    }

}