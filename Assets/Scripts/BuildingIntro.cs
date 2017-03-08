using UnityEngine;
using System.Collections;

public class BuildingIntro : MonoBehaviour {
    public string buildingName;
    public double latitude;
    public double longitude;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setBuildingInfo(string _bname, double _latitude, double _longitude)
    {
        buildingName = _bname;
        latitude = _latitude;
        longitude = _longitude;
    }
}
