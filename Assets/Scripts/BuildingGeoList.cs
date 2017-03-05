using UnityEngine;
using System.Collections;

public class BuildingGeoList : MonoBehaviour {
    public static ArrayList buildingGeoList = new ArrayList();
    
	// Use this for initialization
	void Start () {
        // longitude, latitude
        buildingGeoList.Add(new Vector2(13.3968027f, 52.5414223f));
        buildingGeoList.Add(new Vector2())
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    struct buidlingInfo
    {
        double latitude;
        double longitude;
        double altitude;
        double hading;
        string name;
        string modelHref;
    }
}
