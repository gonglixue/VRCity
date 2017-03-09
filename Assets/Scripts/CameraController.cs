using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Vector4 Range;   // 决定当前tile显示范围
    public Vector3 UpMax;
    public Vector3 DownMax;
    public double longitude = 13.3905676;
    public double latitude = 52.5387557;

    public GameObject MapController;

    private float _fieldOfView;
    private float _height;

    private double _worldScaleFactor;
    private double _scaleFactor;
    // 参考原点tile
    private Vector2 _referencTileMeter;
    private Vector2 _tms;
    private Rect _referenceTileRect;

    private Vector2 positionMeter;  // 相机在墨卡托坐标下的位置
    private Rect cameraRect;    // 相机墨卡托坐标Rect
    private float RectWidth;    // 相机在墨卡托坐标下Rect的宽度


    void Awake()
    {
        _fieldOfView = GetComponent<Camera>().fieldOfView;
        _height = transform.position.y;
        InitReference();

        positionMeter = Mapbox.Conversions.LatLonToMeters(latitude, longitude);
        this.transform.position = MetersToUnity(positionMeter);
        //CalRange(_height, _fieldOfView);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void CalMaxVec()
    {

    }

    void CalRange(float height, float viewAngle)
    {
        float halfWidth = height * Mathf.Tan(viewAngle*Mathf.PI/180f);  // unity 坐标
        // unity 坐标转墨卡托坐标
        float halfWidthMeters = (float)(halfWidth / _worldScaleFactor);
        // 计算tiles个数     
        int tileNum = (int)(halfWidthMeters / _referenceTileRect.width);

        SetRange(new Vector4(tileNum, tileNum, tileNum, tileNum));

        Debug.Log("---------");
        Debug.Log("height:" + height);
        Debug.Log("reference width:" + _referenceTileRect.width);
        Debug.Log("half width:" + halfWidth);
        Debug.Log("view angle:" + viewAngle);
        Debug.Log("tile num: " + tileNum);
        Debug.Log("-------------");
    }

    void InitReference()
    {
        _referencTileMeter = Mapbox.Conversions.LatLonToMeters(Config.latitude, Config.longitude);
        _tms = Mapbox.Conversions.MetersToTile(_referencTileMeter, Config.zoom);
        _referenceTileRect = Mapbox.Conversions.TileBounds(_tms, Config.zoom);

        _worldScaleFactor = Config.tileSize / _referenceTileRect.width;  // 墨卡托坐标转unity

        RectWidth = _referenceTileRect.width;
    }

    void SetRange(Vector4 myRange)
    {
        MapController.GetComponent<Mapbox.MeshGeneration.MapController>().Range = myRange;
    }

    // 墨卡托坐标转unity坐标
    Vector3 MetersToUnity(Vector2 posInMeter)
    {
        Vector2 referenceCenterMeter = _referenceTileRect.center;
        Vector2 resultPos = new Vector2(posInMeter.x - referenceCenterMeter.x, posInMeter.y - referenceCenterMeter.y);
        resultPos = resultPos * (float)_worldScaleFactor;

        Vector3 posUnity = new Vector3(resultPos.x, 4, resultPos.y);
        return posUnity;
    }
}
