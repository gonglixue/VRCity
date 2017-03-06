using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Scripts.Utilities;
using Mapbox.MeshGeneration.Data;
using Mapbox.Utils;


public class InstanceBuilding : MonoBehaviour {

    public GameObject GeoListController;

    private List<buildingInfo> _buildings;

    // 参考原点tile
    private Vector2 _referenceTileMeter;
    private Vector2 _tms;
    private Rect _referenceTileRect;

    // temp
    public double _north = 52.542316087433;  // lat
    public double _south = 52.54121947358615;
    public double _west = 13.396366385489324;  // lon
    public double _east = 13.398223088795774;

    private double _worldScaleFactor;  // 墨卡托坐标到unity坐标
    private double _scaleFator;   // 数据库tileSize / MapboxTileSize

    private GameObject _root;  // 建筑物组父元素

    // Use this for initialization
    void Start () {
        _buildings = GeoListController.GetComponent<BuildingGeoList>().buildingList[0];

        _referenceTileMeter = Mapbox.Conversions.LatLonToMeters(Config.latitude, Config.longitude);
        _tms = Mapbox.Conversions.MetersToTile(_referenceTileMeter, Config.zoom);
        _referenceTileRect = Mapbox.Conversions.TileBounds(_tms, Config.zoom);

        _scaleFator = calTileScaleFactor(_north, _south, _west, _east, _referenceTileRect);
        _worldScaleFactor = Config.tileSize / _referenceTileRect.width;
        Debug.Log("instance.cs:" + _scaleFator + "," + _worldScaleFactor);
        InitRoot();
        CreatBuilding();
	}
	
	// Update is called once per frame
	void Update () {

	}

    // 计算数据库tileSize/MapboxTileSize
    double calTileScaleFactor(double north, double south, double west, double east, Rect referenceTileRect)
    {
        var north_east = Mapbox.Conversions.LatLonToMeters(north, east);
        var south_west = Mapbox.Conversions.LatLonToMeters(south, west);
        double factor = (north_east.y - south_west.y) / referenceTileRect.width;
        return factor;

    }

    void InitRoot()
    {
        _root = new GameObject("Tile88Root");
        _root.transform.localScale = Vector3.one * (float)_scaleFator;
    }

    void CreatBuilding()
    {
        Debug.Log("create building. The list length is: " + _buildings.Count);
        foreach (buildingInfo buildingItem in _buildings)
        {
            Vector2 v2 = Mapbox.Conversions.LatLonToMeters(buildingItem.latitude, buildingItem.longitude);  // 该建筑的经纬度转墨卡托坐标
            // 建筑物距离参考tile中点的墨卡托距离
            double deltax = v2.x - _referenceTileRect.center.x;
            double deltay = v2.y - _referenceTileRect.center.y;

            Vector3 position = new Vector3((float)(deltax * _worldScaleFactor), 0, (float)(deltay * _worldScaleFactor));
            Quaternion rotate = Quaternion.AngleAxis(-89.8f, Vector3.right)*(Quaternion.AngleAxis(180,Vector3.forward));

            string path = buildingItem.modelHref.Split('.')[0];
            path = path.Replace('\\', '/');
            //Debug.Log("resource path:" + path);
            Debug.Log(position);
            GameObject buildingInstance = Instantiate(Resources.Load(path, typeof(GameObject)), position, rotate, _root.transform) as GameObject;

        }
    }
}

