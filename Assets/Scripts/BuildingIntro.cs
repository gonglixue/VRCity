using UnityEngine;
using System.Collections;

public class BuildingIntro : MonoBehaviour {
    public string buildingName;
    public double latitude;
    public double longitude;
    public double altitude;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setBuildingInfo(string _bname, double _latitude, double _longitude, double _altitude)
    {
        buildingName = _bname;
        latitude = _latitude;
        longitude = _longitude;
        altitude = _altitude;
    }
}
