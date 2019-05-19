using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTooltip : MonoBehaviour
{

    [SerializeField]
    private string _text = "Tooltip";
    [SerializeField]
    private bool _enabled = true;

    public string text {
        get {
            return _text;
        }
        set {
            _text = value;
            GetComponentInChildren<Text>().text = _text;
        }
    }

    public bool enabled {
        get {
            return _enabled;
        }
        set {
            _enabled = value;
            gameObject.SetActive(_enabled);
        }
    }

    void start(){
        GetComponentInChildren<Text>().text = _text;
        gameObject.SetActive(_enabled);
    }
}
