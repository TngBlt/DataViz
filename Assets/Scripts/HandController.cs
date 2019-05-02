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
    GameObject text;
    TextMesh textMesh;
    MeshRenderer meshRenderer;

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        imap = map.GetComponent<IMap>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Text
        text = new GameObject();
        textMesh = text.AddComponent<TextMesh>();
        meshRenderer = text.AddComponent<MeshRenderer>();
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
            lineRenderer.SetPosition(1,hit.point);
            addTextNextToHand(hit.point);
        }
            
        else {
           lineRenderer.SetPosition(1,transform.TransformDirection(Vector3.forward) * 1000);
        }
    }

    private void addTextNextToHand(Vector3 hitOnMap) {
        var geoLoc = imap.WorldToGeoPosition(hitOnMap);
        text.transform.position = this.transform.position + Vector3.left;
        textMesh.text = geoLoc.ToString();
    }
}
