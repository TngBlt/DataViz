using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

namespace vizualizers {

    class HeightVizualizer : IVizualizer<float,float> {

        private float maxHeight;
        private float minHeight;
        private float maxDataValue;
        private float minDataValue;

        public float MaxHeight {
            get {return maxHeight;}
        }
        public float MinHeight {
            get {return minHeight;}
        }
        public float MinDataValue {
            get {return minDataValue;}
        }
        public float MaxDataValue {
            get {return maxDataValue;}
        }

        public HeightVizualizer(IEnumerable<float> data, float maxHeight, float minHeight = 0){
            this.maxHeight = maxHeight;
            this.minHeight = minHeight;
            maxDataValue = (float) Math.Ceiling(data.Max() );
            minDataValue = (float) Math.Floor(data.Min() );
        }

        public float getVizualization(float input){
            return (input - minDataValue) / (maxDataValue - minDataValue) * (maxHeight - minHeight) + minHeight;
        }

    }

}