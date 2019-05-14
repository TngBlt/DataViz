using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SimpleController : MonoBehaviour
{
    public XRNode handNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hand_pos = InputTracking.GetLocalPosition(handNode);
        Quaternion hand_rot = InputTracking.GetLocalRotation(handNode);
        gameObject.transform.position = hand_pos;
        gameObject.transform.rotation = hand_rot;
        gameObject.transform.Rotate(0, 180, 0);
    }
}
