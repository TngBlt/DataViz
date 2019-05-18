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

    public ControllerTooltip TriggerTooltip;
    public ControllerTooltip GrabTooltip;

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
            	TriggerTooltip.enabled = true;
                TriggerTooltip.text = "Sélectionner";
                selectionModel.SetActive(false);
            } else if(hit.collider.gameObject.tag == "ui-close") {
            	TriggerTooltip.enabled = true;
                TriggerTooltip.text = "Fermer";
                selectionModel.SetActive(false);
            } else if (hit.collider.GetComponentInParent<DataHandler>() == dataHandler) {
            	TriggerTooltip.enabled = true;
                TriggerTooltip.text = "Zoomer";
                selectionModel.SetActive(true);
            	selectionModel.transform.position = hit.point;
            	selectionModel.transform.up = Vector3.up;
            } else {
            	TriggerTooltip.enabled = false;
            	selectionModel.SetActive(false);
            }
        } else {
            lineRenderer.SetPosition(1,transform.TransformDirection(Vector3.forward) * 1000);
            TriggerTooltip.enabled = false;
            selectionModel.SetActive(false);
        }

        if(!device.isValid){
            device = InputDevices.GetDeviceAtXRNode(handNode);
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

            if(!isTriggering && hasHit && hit.collider.GetComponentInParent<DataHandler>() == dataHandler){
            	dataHandler.Zoom(hit.point);
            }

            if(hasHit && hit.collider.gameObject.tag == "ui-close"){
                hit.collider.GetComponentInParent<DataHandler>().Hide();
            }

            isTriggering = true;
        } else {
        	isTriggering = false;
        }
    }
}
