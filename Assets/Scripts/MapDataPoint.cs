using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataPoint : MonoBehaviour
{

    private float _height;
    private float _scale = 0.06f;

    public float height {
        get { return _height; }
        set {
            _height = value;
            Vector3 s = transform.localScale;
            s.y = _height;
            transform.localScale = s;
        }
    }
    public float scale {
        get { return _scale; }
        set {
            _scale = value;
            Vector3 s = transform.localScale;
            s.x = _scale;
            s.z = _scale;
            transform.localScale = s;
        }
    }
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        if(height == 0) 
            transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
