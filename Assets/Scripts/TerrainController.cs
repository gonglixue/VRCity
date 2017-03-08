﻿using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {
    public Rect worldRect;  // 8*8 tile rect, 墨卡托坐标
    public CityQuadTree qTree;
    private int tileNum = 8;  // 现场景中每边tile数
    

    private GameObject terrainRoot;

	// Use this for initialization
	void Start () {
        InitWorldRect();
        terrainRoot = new GameObject("terrain-root");
        terrainRoot.transform.position = new Vector3(0, 0, 0);
        //terrainRoot.transform.localScale = Vector3.one * BuildingGeoList.GetWorldScaleFactor();
        // 构造
        qTree = new CityQuadTree(worldRect, 0, null);
        qTree.SearchTarget(Mapbox.Conversions.LatLonToMeters(Config.latitude,Config.longitude));
        qTree.Traversal(terrainRoot);
        terrainRoot.transform.localScale = Vector3.one * BuildingGeoList.GetWorldScaleFactor();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void InitWorldRect()
    {
        Vector2 worldCenter = BuildingGeoList.GetRerenceLeftBottomInMeters();  // worldRect center，墨卡托坐标
        float tileSize = BuildingGeoList.tileSizeInMeters();
        float worldRectX = worldCenter.x - tileSize * tileNum/2;
        float worldRectY = worldCenter.y - tileSize * tileNum/2;
        worldRect = new Rect(worldRectX, worldRectY, tileSize * tileNum, tileSize * tileNum);
        
    }

}
