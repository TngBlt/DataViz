using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSet;
using System;
using System.Linq;

public class MapDataPoint : MonoBehaviour
{

    public GameObject bar;
    private bool _muted;
    [SerializeField]
    private float _height;
    private float _scale = 0.06f;
    [SerializeField]
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
            if(!_muted){
                this.SetHeight(_height);
            }
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
    public bool muted {
        get { return _muted; }
        set {
            _muted = value;
            if(_muted){
                this.SetHeight(0.05f);
                Color mutedColor = new Color(_color.r,_color.g,_color.b,_color.a);
                mutedColor.a = 0.1f;
                SetColor(mutedColor);
            } else {
                this.SetHeight(_height);
                SetColor(_color);
            }
        }
    }

    public Color color {
        get { return _color; }
        set {
            _color = value;
            _color.a = 0.7f;
            if(!_muted){
                SetColor(_color);
            } else {
                Color mutedColor = new Color(_color.r,_color.g,_color.b,_color.a);
                mutedColor.a = 0.1f;
                SetColor(mutedColor);
            }
        }
    }

    private void SetHeight(float height){
                Vector3 s = bar.transform.localScale;
                s.y = height;
                bar.transform.localScale = s;
                ResetPosition();
    }

    private void SetColor(Color color){
            Renderer rend = bar.GetComponent<Renderer>();
            rend.material.SetColor("_Color", color);
            rend.material.SetColor("_EmissionColor", color);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(height == 0) 
            bar.transform.gameObject.SetActive(false);
    }

    public void ResetPosition() {
        Vector3 pos = bar.transform.localPosition;
        pos.y = bar.transform.localScale.y/2; 
        bar.transform.localPosition = pos;
    }
}
