using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class HideUnderMap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<AbstractMap>() != null){
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponentInParent<AbstractMap>() != null){
            gameObject.SetActive(true);
        }
    }
}
