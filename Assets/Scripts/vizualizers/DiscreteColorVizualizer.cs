using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace vizualizers {

    class DiscreteColorVizualizer : IVizualizer<string,Color> {

        public Dictionary<string,Color> colors = new Dictionary<string,Color>();

        public DiscreteColorVizualizer(IEnumerable<string> data, float saturation = 0.88f, float brightness = 0.85f){
            List<string> values = new List<string>();

            foreach(string val in data){
                string cleanValue = getCleanValue(val);

                if(!values.Contains(cleanValue)) {
                    values.Add(cleanValue);
                }
            }

            for(int i = 0; i<values.Count; i++){
                float hue = (i * (1.0f / values.Count));
                string cleanValue = getCleanValue(values[i]);

                colors.Add(cleanValue,Color.HSVToRGB(hue,saturation,brightness));
            }
        }

        protected string getCleanValue(string dirtyValue){
            return dirtyValue.ToLower().Trim();
        }

        public Color getVizualization(string input){
            return colors[getCleanValue(input)];
        }

    }

}