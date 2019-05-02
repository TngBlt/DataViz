using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSet;

public class MapDataPoint : MonoBehaviour
{

    public GameObject bar;
    private float _height;
    private float _scale = 0.06f;
    public Color color;
    public DataPoint point;

    public Vector3 top {
        get {
            var pos = bar.transform.position;
            pos.y += bar.transform.localScale.y/2;
            return pos;
        }
    }

    public float height {
        get { return _height; }
        set {
            _height = value;
            Vector3 s = bar.transform.localScale;
            s.y = _height;
            bar.transform.localScale = s;
            ResetPosition();
        }
    }
    public float scale {
        get { return _scale; }
        set {
            _scale = value;
            Vector3 s = bar.transform.localScale;
            s.x = _scale;
            s.z = _scale;
            bar.transform.localScale = s;
            ResetPosition();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(height == 0) 
            bar.transform.gameObject.SetActive(false);
    }

    private void ResetPosition() {
        Vector3 pos = bar.transform.position;
        pos.y = bar.transform.localScale.y/2; 
        bar.transform.position = pos;
    }
}
