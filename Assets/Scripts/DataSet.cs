using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataSet {
        
    [System.Serializable]
    public class FieldDeclaration
    {
        public string id;
        public string displayname;
        public string type;
    }

    [System.Serializable]
    public class DatasetInfo
    {
        public string name;
        public List<FieldDeclaration> fields;

        public string primaryField;
        public string secondaryField;
        public float zoom;
        public List<double> center;
    }

    [System.Serializable]
    public class TimeValue
    {
        public long timestamp;
        public List<FieldValue> fields;
    }

    [System.Serializable]
    public class FieldValue
    {
        public string id;
        public string value;
    }

    [System.Serializable]
    public class DataPoint
    {
        public List<double> point;
        public List<TimeValue> values;

        private TimeIndexer _indexer;

        public TimeIndexer indexer {
            get { return _indexer; }
        }

        public TimeIndexer InitializeIndexer(){
            IEnumerable<long> vals = values.Select(el => el.timestamp);

            _indexer = new TimeIndexer(vals.Min(), vals.Max() - vals.Min());

            foreach(TimeValue val in values){
                _indexer.Add(val);
            }

            return _indexer;
        }
    }

    [System.Serializable]
    public class DataSet
    {
        public DatasetInfo dataset;
        public List<DataPoint> data;
    }
}
