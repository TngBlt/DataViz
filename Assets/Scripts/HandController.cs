using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Map.Interfaces;
using Mapbox.Unity.Map;
using UnityEngine.UI;
using System;

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
    private bool isGrabbing = false;
    private Vector3 grabbingStartOffset;
    private Vector3 grabbingStartPosition;
    private Quaternion grabbingStartRotation;
    public GameObject TopUI;

    public Text dateText;
    public Text timeText;

    public DateTime currentDate = DateTime.Now;
    public DateTime grabbingDate;

    public Double timeScale;

    public RectTransform GraduationImage;

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
                dataHandler.HideLegend();
            }

            isTriggering = true;
        } else {
        	isTriggering = false;
        }

        bool grabValue;
        if(device.TryGetFeatureValue(CommonUsages.gripButton, out grabValue) && grabValue){
            if(!isGrabbing){
                isGrabbing = true;
                grabbingStartPosition = TopUI.transform.position;
                grabbingStartRotation = TopUI.transform.rotation;
                grabbingStartOffset = grabbingStartPosition - (grabbingStartPosition + Vector3.Project(transform.position,grabbingStartRotation * Vector3.left));
            }
            TopUI.transform.position = grabbingStartPosition + Vector3.Project(transform.position,grabbingStartRotation * Vector3.left) + grabbingStartOffset;
            TopUI.transform.rotation = grabbingStartRotation;

            float grabbingDistance = Vector3.Dot(grabbingStartRotation * Vector3.left,(grabbingStartPosition - TopUI.transform.position).normalized) * (grabbingStartPosition - TopUI.transform.position).magnitude;
            
            grabbingDate = currentDate.AddMilliseconds(grabbingDistance*timeScale);

            dataHandler.displayedDate = (new DateTimeOffset(grabbingDate)).ToUnixTimeMilliseconds();
            UpdateDate(new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(dataHandler.displayedDate));

        } else if(isGrabbing){
            currentDate = grabbingDate;
            dataHandler.displayedDate = (new DateTimeOffset(currentDate)).ToUnixTimeMilliseconds();
            UpdateDate(new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(dataHandler.displayedDate));
            isGrabbing = false;
            TopUI.transform.localPosition = new Vector3(0,0,0);
            TopUI.transform.localRotation = new Quaternion(0,0,0,0);
        } else {
            currentDate = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(dataHandler.displayedDate);
            UpdateDate(currentDate);
        }
    }

    private void UpdateDate(DateTime date){
        dateText.text = string.Format("{0:dd/MM/yyyy}",date);
        timeText.text = string.Format("{0:hh}:{0:mm}",date);
    }
}
