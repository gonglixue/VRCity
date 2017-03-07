using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Vector4 Range;   // 决定当前tile显示范围
    public Vector3 UpMax;
    public Vector3 DownMax;

    public GameObject MapController;

    private float _fieldOfView;
    private float _height;

    private double _worldScaleFactor;
    private double _scaleFactor;
    // 参考原点tile
    private Vector2 _referencTileMeter;
    private Vector2 _tms;
    private Rect _referenceTileRect;

    void Awake()
    {
        _fieldOfView = GetComponent<Camera>().fieldOfView;
        _height = transform.position.y;
        InitReference();
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
    }

    void SetRange(Vector4 myRange)
    {
        MapController.GetComponent<Mapbox.MeshGeneration.MapController>().Range = myRange;
    }
}
