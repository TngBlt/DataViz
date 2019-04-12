using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadController : MonoBehaviour
{

    public XRNode headNode;

    // Start is called before the first frame update
    void Start()
    {
        InputTracking.Recenter();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 head_pos = InputTracking.GetLocalPosition(headNode);
        Quaternion head_rot = InputTracking.GetLocalRotation(headNode);
        gameObject.transform.position = head_pos;
        gameObject.transform.rotation = head_rot;
    }
}
