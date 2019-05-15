using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSet;
using vizualizers;

public delegate void DataLoadedDelegate(DataSet.DataSet dataset, DataLoader loader);

public class DataLoader : MonoBehaviour
{

	public TextAsset datasetFile;

	private DataSet.DataSet mapData;
    private FieldDeclaration _primaryField;
    private FieldDeclaration _secondaryField;

    public DataLoadedDelegate onLoaded;

    public bool loaded {
    	get {
    		return mapData != null;
    	}
    }

    public DataSet.DataSet data {
    	get {
    		return mapData;
    	}
    }

    public FieldDeclaration primaryField {
    	get {
    		return _primaryField;
    	}
    }

    public FieldDeclaration secondaryField {
    	get {
    		return _secondaryField;
    	}
    }

    // Start is called before the first frame update
    void Start()
    {
        if(datasetFile) {
            LoadMapData(datasetFile);
            Debug.Log("Données chargées");
            if(onLoaded != null){
	            onLoaded(mapData, this);
	        }
        } else {
           Debug.LogError("Le fichier JSON est introuvable !");
        }
    }

    private void LoadMapData(TextAsset textAsset) {
        mapData = JsonUtility.FromJson<DataSet.DataSet>(textAsset.text);
        _primaryField = mapData.dataset.fields.Find(el => el.id == mapData.dataset.primaryField);
        _secondaryField = mapData.dataset.fields.Find(el => el.id == mapData.dataset.secondaryField);
    }
}
