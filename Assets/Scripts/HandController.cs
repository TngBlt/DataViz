using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Map.Interfaces;
using Mapbox.Unity.Map;

public class HandController : MonoBehaviour
{

    public XRNode handNode;
    public GameObject map;
    private IMap imap;
    private DataHandler dataHandler;
    LineRenderer lineRenderer;
    private InputDevice device;

    public GameObject selectTooltip;
    public GameObject timeTooltip;
    public GameObject zoomTooltip;

    public GameObject selectionModel;

    private bool isTriggering = false;

    // Start is called before the first frame update
    void Start()
    {
        imap = map.GetComponent<IMap>();
        dataHandler = map.GetComponent<DataHandler>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        //get device
        device = InputDevices.GetDeviceAtXRNode(handNode);
        selectionModel.SetActive(false);
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
        bool hasHit = Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out hit, 1000);

        if(hasHit) {        	
            lineRenderer.SetPosition(1,hit.point);

            if(hit.collider.GetComponentInParent<MapDataPoint>() != null){
            	selectTooltip.SetActive(true);
            } else {
            	selectTooltip.SetActive(false);
            }

            if (hit.collider.GetComponentInParent<AbstractMap>() == imap){
            	zoomTooltip.SetActive(true);
            	selectionModel.SetActive(true);
            	selectionModel.transform.position = hit.point;
            	selectionModel.transform.up = Vector3.up;
            } else {
            	zoomTooltip.SetActive(false);
            	selectionModel.SetActive(false);
            }

        } else {
            lineRenderer.SetPosition(1,transform.TransformDirection(Vector3.forward) * 1000);
            selectTooltip.SetActive(false);
            zoomTooltip.SetActive(false);
        }

        // check trigger press
        bool triggerValue;
        if(device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue) {

			if(hasHit && hit.collider.GetComponentInParent<MapDataPoint>() != null) {
				if(!isTriggering){
	                dataHandler.ShowInfo(hit.collider.GetComponentInParent<MapDataPoint>());
	            }
            }
            else {
                dataHandler.HideInfo();
            }

            if(!isTriggering && hasHit && hit.collider.GetComponentInParent<AbstractMap>() == imap){
            	dataHandler.Zoom(hit.point);
            }

            isTriggering = true;
        } else {
        	isTriggering = false;
        }
    }
}
