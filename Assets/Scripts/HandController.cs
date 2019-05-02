using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Map.Interfaces;

public class HandController : MonoBehaviour
{

    public XRNode handNode;
    public GameObject map;
    private IMap imap;
    private DataHandler dataHandler;
    LineRenderer lineRenderer;
    private InputDevice device;
    private bool isOverBar;

    // Start is called before the first frame update
    void Start()
    {
        imap = map.GetComponent<IMap>();
        dataHandler = map.GetComponent<DataHandler>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        //get device
        device = InputDevices.GetDeviceAtXRNode(handNode);
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

        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out hit, 1000)) {
            if(hit.collider.GetComponentInParent<MapDataPoint>() != null) {
                isOverBar = true;
                dataHandler.ShowInfo(hit.collider.GetComponentInParent<MapDataPoint>());
            }
            else {
                isOverBar = false;
                dataHandler.HideInfo();
            }
            lineRenderer.SetPosition(1,hit.point);
        }
            
        else {
            lineRenderer.SetPosition(1,transform.TransformDirection(Vector3.forward) * 1000);
        }

        // check trigger press
        //bool triggerValue;
        // if(device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue) {
        //     if(hit.collider.gameObject)
        //     zoom(hit.point);
        // }
    }
}
