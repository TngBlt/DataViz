using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace vizualizers {

    public class TimeManager {

        private long _maxTime;
        private long _minTime;

        public long maxTime {
            get { return _maxTime; }
        }

        public long minTime {
            get { return _minTime; }
        }

        private List<long> _steps = new List<long>();

        public TimeManager(IEnumerable<long> data){
            _maxTime = data.Max();
            _minTime = data.Min();

            foreach(long date in data){
                if(!_steps.Contains(date)) {
                    _steps.Add(date);
                }
            }
            _steps.Sort();
        }

        public long GetNearest(long currentDate){
            int i = 0;
            foreach(long date in _steps){
                if(date > currentDate) {
                    if(i == 0 || i == _steps.Count-1){
                        return date;
                    } else if(_steps[i-1] <= currentDate) {
                        return _steps[i-1];
                    }
                }
                i++;
            }
            return _maxTime;
        }

    }

}