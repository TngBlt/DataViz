using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using DataSet;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class DataHandler : MonoBehaviour
{
    private DataSet.DataSet mapData;
    private FieldDeclaration primaryField;
    private float primaryFiledMaxValue = 1f;
    private AbstractMap map;
    public float pointsScale = 0.06f;
    public float maxHeight = 2.0f;
    public GameObject PointPrefab;

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<AbstractMap>();
        map.OnInitialized += LoadDataset;

        // Load data from  JSOIN
        var jsonTextFile = Resources.Load<TextAsset>("out");
        if(jsonTextFile) {
            mapData = LoadMapData(jsonTextFile);
            
            primaryField = mapData.dataset.fields.Find(el => el.id == mapData.dataset.primaryField);
            if(primaryField.type == "string"){
                Debug.LogError("Un champ de type 'string' ne peut être un champ primaire");
            } else {
                primaryFiledMaxValue = mapData.data.Select(el => el.fields.Find(x => x.id == primaryField.id)).Select(el => float.Parse(el.value)).Max();
            }

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
            CreatePoint(dataPoint);
        }
        Debug.Log("Données affichées");
    }

    private void CreatePoint(DataPoint point){
            Vector2d latLng = new Vector2d(point.point[1], point.point[0]);
            Vector3 pos = map.GeoToWorldPosition(latLng, false);

            GameObject dataPoint = Instantiate(PointPrefab,pos,Quaternion.identity);
            MapDataPoint parameters = dataPoint.GetComponent<MapDataPoint>();

            if(primaryField == null || primaryField.type == "string"){
                parameters.height = 1;
            } else {
                FieldValue val = point.fields.Find(el => el.id == primaryField.id);
                if(val != null){
                    parameters.height = (float.Parse(val.value) * maxHeight)/primaryFiledMaxValue;
                } else {
                    parameters.height = 0;
                }
            }

            parameters.scale = pointsScale;

    }
}
