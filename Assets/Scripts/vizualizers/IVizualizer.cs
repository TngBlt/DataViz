using System.Linq;

namespace vizualizers {
    interface IVizualizer<InputType,OutputType> {
        OutputType getVizualization(InputType input);
    }
}