using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using DataSet;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using vizualizers;

public class DataHandler : MonoBehaviour
{
    private HeightVizualizer primaryVizualizer;
    private DiscreteColorVizualizer secondaryDiscreteVizualizer;
    private RangeColorVizualizer rangeColorVizualizer;
    private AbstractMap map;

    public float pointsScale = 0.06f;
    public float maxHeight = 2.0f;

    public GameObject PointPrefab;
    public GameObject PointInfoPrefab;
    public GameObject PointInfoText;
    public Transform PanelTransform;
    public DataLoader DataLoader;

    public GameObject zoomMap;
    public float zoomedHeight = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<AbstractMap>();
        map.OnInitialized += LoadDataset;

        if(DataLoader.loaded){
            this.LoadMapData();
        } else {
            DataLoader.onLoaded = delegate(DataSet.DataSet dataset, DataLoader loader){ this.LoadMapData(); };
        }
        // Get panel
        PanelTransform = PointInfoPrefab.transform.Find("Panel");

        HideInfo();
    }

    private void LoadMapData() {
        var mapData = DataLoader.data;
            
        var primaryField = DataLoader.primaryField;
        if(primaryField.type == "number"){
            primaryVizualizer = new HeightVizualizer(mapData.data.Select(el => el.fields.Find(x => x.id == primaryField.id)).Select(el => float.Parse(el.value)),maxHeight);
        } else {
            Debug.LogError("Unsupported type '"+primaryField.type+"' on secondary field");
        }

        var secondaryField = DataLoader.secondaryField;
        if(secondaryField.type == "string"){
            secondaryDiscreteVizualizer = new DiscreteColorVizualizer((mapData.data.Select(el => el.fields.Find(x => x.id == secondaryField.id).value)));
        } else if(secondaryField.type == "number") {
            rangeColorVizualizer = new RangeColorVizualizer((mapData.data.Select(el => float.Parse(el.fields.Find(x => x.id == secondaryField.id).value))), Color.green, Color.red );
        } else {
            Debug.LogError("Unsupported type '"+secondaryField.type+"' on secondary field");
        }

        var locOpt = map.Options.locationOptions;
        map.Initialize(Conversions.StringToLatLon(locOpt.latitudeLongitude),(int) locOpt.zoom);
    }

    private void LoadDataset() {
        var mapData = DataLoader.data;
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
            var secondaryField = DataLoader.secondaryField;
            var primaryField = DataLoader.primaryField;
            parameters.point = point;
            
            if(primaryVizualizer != null){
                FieldValue val = point.fields.Find(el => el.id == primaryField.id);
                if(val != null){
                    parameters.height = primaryVizualizer.getVizualization(float.Parse(val.value));
                } else {
                    parameters.height = 0;
                }
            } else {
                parameters.height = 1;
            }

            if(secondaryDiscreteVizualizer != null){
                FieldValue val = point.fields.Find(el => el.id == secondaryField.id);
                if(val != null){
                    parameters.color = secondaryDiscreteVizualizer.getVizualization(val.value);
                }
            }
            else if(rangeColorVizualizer != null) {
                FieldValue val = point.fields.Find(el => el.id == secondaryField.id);
                if(val != null){
                    parameters.color = rangeColorVizualizer.getVizualization(float.Parse(val.value));
                }
            }

            parameters.scale = pointsScale;
    }

    public void ShowInfo(MapDataPoint dataPoint) {
        //Show panel with info
        PointInfoPrefab.SetActive(true);
        PointInfoPrefab.transform.position =  dataPoint.top;
        var mapData = DataLoader.data;

        List<string[]> lstValue =  dataPoint.point.fields.Select( el => {
            var def = mapData.dataset.fields.Find( f => f.id == el.id);
            if(def != null) {
                return new string[] {def.displayName,el.value};
            } else {
                return new string[] {el.id,el.value};
            }
        }).ToList();

        foreach(Transform text in PanelTransform) {
            Destroy(text.gameObject); 
        }
        
        PointInfoPrefab.transform.LookAt(Camera.main.transform.position);
        float yAxe = PointInfoPrefab.transform.eulerAngles.y;
        PointInfoPrefab.transform.eulerAngles = new Vector3(0, yAxe+180, 0);


        lstValue.ForEach( txt => {
            GameObject textInfoDescription = Instantiate(PointInfoText, PanelTransform.position, PointInfoPrefab.transform.rotation, PanelTransform);
            textInfoDescription.GetComponent<Text>().text = txt[0];
            GameObject textInfoValue = Instantiate(PointInfoText, PanelTransform.position, PointInfoPrefab.transform.rotation, PanelTransform);
            textInfoValue.GetComponent<Text>().text = txt[1];
        });

        var layout = PanelTransform.GetComponent<GridLayoutGroup>();
        var height = lstValue.Count * layout.cellSize.y + layout.padding.vertical;
        var panelRect = PanelTransform.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x,height);

        var distanceToFloor = (PointInfoPrefab.transform.position.y - ((height + layout.padding.bottom) * PointInfoPrefab.transform.localScale.y) ) - dataPoint.bottom.y;

        if(distanceToFloor < 0){
            PointInfoPrefab.transform.Translate(new Vector3(0,Math.Abs(distanceToFloor),0));
        }

    }

    public void HideInfo() {
         PointInfoPrefab.SetActive(false);
    }

    public void Zoom(Vector3 mapPos, float zoom = 16f){

        mapPos.y += zoomedHeight;
        zoomMap.transform.position = mapPos;

        zoomMap.SetActive(true);

        Vector2d center = map.WorldToGeoPosition(mapPos);
        AbstractMap zoomedMap = zoomMap.GetComponent<AbstractMap>();
        zoomedMap.UpdateMap(center,zoom);
    }
}


