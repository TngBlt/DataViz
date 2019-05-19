using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using DataSet;
using Tiequar;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using vizualizers;
using UnityEngine.Networking;
using System.Globalization;

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

    private GameObject pointsGroup;

    public bool canClose;

    public GameObject closeCanvas;
    public GameObject tiequarCanvas;

    public DataHandler parentMap;

    public Text SuburbText;
    public Text CityDistrictText;

    public GameObject BorderObject;

    private TimeManager timeManager;

    [SerializeField]
    private long date;

    private MapDataPoint shownInfo = null;

    public long displayedDate {
        get {return date; }
        set { 
            long newDate = timeManager.GetNearest(value);
            if(newDate != date){
                date = newDate;
                if(zoomMap != null){
                    zoomMap.GetComponent<DataHandler>().displayedDate = date;
                }
                UpdateAllPoints();
                if(shownInfo != null){
                    updateInfoPanel(shownInfo);
                }
            }
        }
    }

    public Bounds boundingBox {
        get {

            var b = new Bounds(map.gameObject.transform.position, Vector3.zero);
            foreach (Renderer r in map.GetComponentsInChildren<Renderer>()) {
                b.Encapsulate(r.bounds);
            }
            return b;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pointsGroup = new GameObject("MapPointGroup");

        map = GetComponentInChildren<AbstractMap>();
        map.OnInitialized += LoadDataset;
        map.OnUpdated += UpdateUi;

        if(DataLoader.loaded){
            this.LoadMapData();
        } else {
            DataLoader.onLoaded += delegate(DataSet.DataSet dataset, DataLoader loader){ this.LoadMapData(); };
        }
        // Get panel
        PanelTransform = PointInfoPrefab.transform.Find("Panel");

        HideInfo();
    }

    private void LoadMapData() {
        var mapData = DataLoader.data;
            
        var primaryField = DataLoader.primaryField;
        if(primaryField.type == "number"){
            primaryVizualizer = new HeightVizualizer(
                    mapData.data
                        .SelectMany(el => el.values)
                        .Select(el => el.fields.Find(x => x.id == primaryField.id))
                        .Select(el => {
                            try {
                                return float.Parse(el.value,System.Globalization.NumberStyles.Any,System.Globalization.CultureInfo.InvariantCulture);
                            } catch(Exception e) {
                                Debug.LogError("Can't parse number '"+el.value+"' : "+e.Message);
                                return 0;
                            }
                        }),
                    maxHeight
                );
        } else {
            Debug.LogError("Unsupported type '"+primaryField.type+"' on secondary field");
        }

        var secondaryField = DataLoader.secondaryField;
        if(secondaryField.type == "string"){

            secondaryDiscreteVizualizer = new DiscreteColorVizualizer(
                mapData.data
                    .SelectMany(el => el.values)
                    .Select(el => el.fields.Find(x => x.id == secondaryField.id).value));

        } else if(secondaryField.type == "number") {

            rangeColorVizualizer = new RangeColorVizualizer((
                mapData.data
                    .SelectMany(el => el.values)
                    .Select(el => {
                            var strVal = el.fields.Find(x => x.id == secondaryField.id).value;
                            try {
                                return float.Parse(strVal,System.Globalization.NumberStyles.Any,System.Globalization.CultureInfo.InvariantCulture);
                            } catch(Exception e) {
                                Debug.LogError("Can't parse number '"+strVal+"' : "+e.Message);
                                return 0;
                            }
                    })),
                Color.green,
                Color.red);
        
        } else {
            Debug.LogError("Unsupported type '"+secondaryField.type+"' on secondary field");
        }

        timeManager = new TimeManager(mapData.data.SelectMany(el => el.values).Select(el => el.timestamp));
        date = timeManager.maxTime;

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

            GameObject dataPoint = Instantiate(PointPrefab,pos,Quaternion.identity,pointsGroup.transform);
            
            MapDataPoint parameters = dataPoint.GetComponent<MapDataPoint>();
            var secondaryField = DataLoader.secondaryField;
            var primaryField = DataLoader.primaryField;
            parameters.point = point;
            
            TimeValue currentValue = point.values.Find(el => el.timestamp == date);

            if(primaryVizualizer != null){
                if(currentValue != null){
                    FieldValue val = currentValue.fields.Find(el => el.id == primaryField.id);
                    if(val != null){
                        parameters.height = primaryVizualizer.getVizualization(float.Parse(val.value,CultureInfo.InvariantCulture));
                    } else {
                        parameters.height = 0;
                    }
                } else {
                        parameters.height = 0;
                }
            } else {
                parameters.height = 1;
            }

            if(secondaryDiscreteVizualizer != null && currentValue != null){
                FieldValue val = currentValue.fields.Find(el => el.id == secondaryField.id);
                if(val != null){
                    parameters.color = secondaryDiscreteVizualizer.getVizualization(val.value);
                }
            }
            else if(rangeColorVizualizer != null && currentValue!= null) {
                FieldValue val = currentValue.fields.Find(el => el.id == secondaryField.id);
                if(val != null){
                    parameters.color = rangeColorVizualizer.getVizualization(float.Parse(val.value,CultureInfo.InvariantCulture));
                }
            }

            parameters.scale = pointsScale;
    }

    private void UpdatePoint(GameObject datapoint){
        MapDataPoint dataPointScript = datapoint.GetComponent<MapDataPoint>();
        Vector2d latLng = new Vector2d(dataPointScript.point.point[1], dataPointScript.point.point[0]);
        Vector3 pos = map.GeoToWorldPosition(latLng, false);
        datapoint.transform.position = pos;
        dataPointScript.ResetPosition();
        if(boundingBox.Contains(pos)){
            datapoint.SetActive(true);
        } else {
            datapoint.SetActive(false);
        }

        if(zoomMap != null && zoomMap.activeSelf){
            Bounds zoombb = zoomMap.GetComponent<DataHandler>().boundingBox;
            zoombb.min = new Vector3(zoombb.min.x,transform.position.y-0.1f,zoombb.min.z);
            if(zoombb.Contains(pos)){
                dataPointScript.muted = true;
            } else {
                dataPointScript.muted = false;
            }
        } else {
                dataPointScript.muted = false;
        }

        MapDataPoint parameters = datapoint.GetComponent<MapDataPoint>();
        var secondaryField = DataLoader.secondaryField;
        var primaryField = DataLoader.primaryField;
        TimeValue currentValue = dataPointScript.point.values.Find(el => el.timestamp == date);

        if(primaryVizualizer != null){
            if(currentValue != null){
                FieldValue val = currentValue.fields.Find(el => el.id == primaryField.id);
                if(val != null){
                    parameters.height = primaryVizualizer.getVizualization(float.Parse(val.value,CultureInfo.InvariantCulture));
                } else {
                    parameters.height = 0;
                }
            } else {
                    parameters.height = 0;
            }
        } else {
            parameters.height = 1;
        }

        if(secondaryDiscreteVizualizer != null && currentValue != null){
            FieldValue val = currentValue.fields.Find(el => el.id == secondaryField.id);
            if(val != null){
                parameters.color = secondaryDiscreteVizualizer.getVizualization(val.value);
            }
        }
        else if(rangeColorVizualizer != null && currentValue!= null) {
            FieldValue val = currentValue.fields.Find(el => el.id == secondaryField.id);
            if(val != null){
                parameters.color = rangeColorVizualizer.getVizualization(float.Parse(val.value,CultureInfo.InvariantCulture));
            }
        }
    }

    public void UpdateAllPoints(){
        foreach (Transform child in pointsGroup.transform) {
            if(child.GetComponent<MapDataPoint>() != null){
                UpdatePoint(child.gameObject);
            }
        }
    }

    public void ShowInfo(MapDataPoint dataPoint) {
        //Show panel with info
        PointInfoPrefab.SetActive(true);
        PointInfoPrefab.transform.position =  dataPoint.top;
        var mapData = DataLoader.data;

        TimeValue currentValue = dataPoint.point.values.Find(el => el.timestamp == date);

        if(currentValue == null)
            return;
        
        PointInfoPrefab.transform.LookAt(Camera.main.transform.position);
        float yAxe = PointInfoPrefab.transform.eulerAngles.y;
        PointInfoPrefab.transform.eulerAngles = new Vector3(0, yAxe+180, 0);

        shownInfo = dataPoint;

        updateInfoPanel(dataPoint);

    }

    void updateInfoPanel(MapDataPoint dataPoint){
        var mapData = DataLoader.data;
        TimeValue currentValue = dataPoint.point.values.Find(el => el.timestamp == date);

        List<string[]> lstValue =  currentValue.fields.Select( el => {
            var def = mapData.dataset.fields.Find( f => f.id == el.id);
            if(def != null) {
                return new string[] {def.displayname,el.value};
            } else {
                return new string[] {el.id,el.value};
            }
        }).ToList();

        foreach(Transform text in PanelTransform) {
            Destroy(text.gameObject); 
        }

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

    public void UpdateUi(){
         Bounds bb = boundingBox;
        if(canClose){
            closeCanvas.SetActive(true);
           
            Vector3 closePos = bb.min;
            closePos.y = transform.position.y;
            closeCanvas.transform.position = closePos;
        }

        Vector3 tiequarPos = bb.min;
        tiequarPos.y = transform.position.y;
        tiequarPos.z = bb.max.z;
        tiequarCanvas.transform.position = tiequarPos;
        
        BorderObject.transform.position = bb.center + new Vector3(0,-0.0001f ,0);
        BorderObject.transform.localScale = bb.size / 10f + new Vector3(0.003f,1,0.003f);
    }

    public void Hide(){
        gameObject.SetActive(false);
        pointsGroup.SetActive(false);
        if(parentMap != null){
            parentMap.UpdateAllPoints();
        }
    }

    public void Show(){
        gameObject.SetActive(true);
        pointsGroup.SetActive(true);
    }

    public void UpdateMap(Vector2d latLng, float zoom){
        AbstractMap zoomedMap = GetComponentInChildren<AbstractMap>();
        zoomedMap.UpdateMap(latLng,zoom);
        if(parentMap != null) {
            StartCoroutine(GetTiequarName(latLng));
        }
        UpdateAllPoints();
    }

    public void HideInfo() {
         PointInfoPrefab.SetActive(false);
         shownInfo = null;
    }

    public void Zoom(Vector3 mapPos, float zoom = -1){

        if(zoom < 0){
            zoom = map.Options.locationOptions.zoom + 3;
        }

        mapPos.y += zoomedHeight;
        zoomMap.transform.position = mapPos;

        DataHandler handler = zoomMap.GetComponent<DataHandler>();

        handler.Show();

        Vector2d center = map.WorldToGeoPosition(mapPos);
        handler.UpdateMap(center,zoom);
        handler.displayedDate = date;
        UpdateAllPoints();
    }

    private IEnumerator GetTiequarName(Vector2d LatLng) {
        tiequarCanvas.SetActive(false);
        UnityWebRequest request = UnityWebRequest.Get("https://nominatim.openstreetmap.org/reverse?lat="+LatLng.x.ToString().Replace(',','.')+"&lon="+LatLng.y.ToString().Replace(',','.')+"&format=json&zoom=14");
        request.SetRequestHeader("User-Agent","DataViz CESI Lyon A4");
        yield return request.SendWebRequest();

        if(request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
        }
        else {
            Tiequar.RootObject tiequar = JsonUtility.FromJson<Tiequar.RootObject>(request.downloadHandler.text);
            if(tiequar.display_name != null){
                string[] displayName = tiequar.display_name.Split(',');
                SuburbText.text = displayName[0];
                CityDistrictText.text = displayName[1];
                tiequarCanvas.SetActive(true);
            }
        }
    }
}


