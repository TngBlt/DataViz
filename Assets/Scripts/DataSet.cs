using System.Collections;
using System.Collections.Generic;
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
    }

    [System.Serializable]
    public class DataSet
    {
        public DatasetInfo dataset;
        public List<DataPoint> data;
    }
}
