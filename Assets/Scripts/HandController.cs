using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{

    public XRNode handNode;

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hand_pos = InputTracking.GetLocalPosition(handNode);
        Quaternion hand_rot = InputTracking.GetLocalRotation(handNode);
        gameObject.transform.position = hand_pos;
        gameObject.transform.rotation = hand_rot;

        
        lineRenderer.SetPosition(0,transform.position);

        RaycastHit hit;

        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out hit, 1000))
            lineRenderer.SetPosition(1,hit.point);
        else {
           lineRenderer.SetPosition(1,transform.TransformDirection(Vector3.forward) * 1000);
        }
    }
}
