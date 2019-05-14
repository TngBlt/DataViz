using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSet;

public class MapDataPoint : MonoBehaviour
{

    public GameObject bar;
    private float _height;
    private float _scale = 0.06f;
    private Color _color = new Color(0f,0.6f,1f,0.7f);
    public DataPoint point;

    public Vector3 top {
        get {
            var pos = bar.transform.position;
            pos.y += bar.transform.localScale.y/2;
            return pos;
        }
    }

    public Vector3 bottom {
        get {
            var pos = bar.transform.position;
            pos.y -= bar.transform.localScale.y/2;
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

    public Color color {
        get { return _color; }
        set {
            _color = value;
            _color.a = 0.7f;
            Renderer rend = bar.GetComponent<Renderer>();
            //rend.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            //rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", _color);
            rend.material.SetColor("_EmissionColor", _color);
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
