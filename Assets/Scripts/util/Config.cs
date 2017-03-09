using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Config{
    public static double longitude = 13.3905676;  // 用该经纬度来定义参考tile
    public static double latitude = 52.5387557;
    public static int zoom = 16;
    public static float tileSize = 100;

    public static Vector3 myPos = new Vector3(5, 5, 5);
    public static Dictionary<Vector2, GameObject> tilesDic = new Dictionary<Vector2, GameObject>();

    public static double worldFactor;
    public static int maxTreeDepth = 3;

    public static Rect referenceRect;
    public static Vector2 referenceTileMeter;   // 给定参考原点的经纬度对应的墨卡托坐标
    public static Vector2 tms;                  // 参考原点所在tile的ID

    public static void setZoom(int newZoom)
    {
        zoom = newZoom;
    }

    public static void AddImageDataForTile(Vector2 tileID, Texture2D ImageData)
    {
        GameObject tile = tilesDic[tileID];
        tile.GetComponent<TileIntro>().imageData = ImageData;
    }

    public static void AddHeightDataForTile(Vector2 tileID, Texture2D HeightData)
    {
        GameObject tile = tilesDic[tileID];
        tile.GetComponent<TileIntro>().heightData = HeightData;
    }

    public static void AddVectorDataForTile(Vector2 tileID, Texture2D vectorData)
    {
        GameObject tile = tilesDic[tileID];
        tile.GetComponent<TileIntro>().vectorData = vectorData;
    }

}
