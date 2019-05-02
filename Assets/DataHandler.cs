using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DataSet;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class DataHandler : MonoBehaviour
{
    private DataSet.DataSet mapData;
    private AbstractMap map;

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<AbstractMap>();
        map.OnInitialized += LoadDataset;

        // Load data from  JSOIN
        var jsonTextFile = Resources.Load<TextAsset>("out");
        if(jsonTextFile) {
            mapData = LoadMapData(jsonTextFile);
            Debug.Log("Données chargées");

            var locOpt = map.Options.locationOptions;
            map.Initialize(Conversions.StringToLatLon(locOpt.latitudeLongitude),(int) locOpt.zoom);
        }
        else 
           Debug.LogError("Le fichier JSON est introuvable !");
    }

    private DataSet.DataSet LoadMapData(TextAsset textAsset) {
        return JsonUtility.FromJson<DataSet.DataSet>(textAsset.text);
    }

    private void LoadDataset() {
        foreach(DataPoint dataPoint in mapData.data) {
            Vector2d latLng = new Vector2d(dataPoint.point[1], dataPoint.point[0]);
            Vector3 point = map.GeoToWorldPosition(latLng, false);
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.localScale /= 15;
            cylinder.transform.position = point;
        }
        Debug.Log("Données affichées");
    }
}
