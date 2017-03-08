using UnityEngine;
using System.Collections;

public static class Config{
    public static double longitude = 13.3905676;  // 用该经纬度来定义参考tile
    public static double latitude = 52.5387557;
    public static int zoom = 16;
    public static float tileSize = 100;

    public static Vector3 myPos = new Vector3(5, 5, 5);

    public static void setZoom(int newZoom)
    {
        zoom = newZoom;
    }

}
