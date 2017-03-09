using UnityEngine;
using System.Collections;

public class TileIntro : MonoBehaviour {
    public Vector2 tileID;
    public Rect tileRect;  //墨卡托坐标下的Rect
    public int zoom;
    [SerializeField]
    private bool isReferenceTile = false;
    public float relativeScale;

    public Texture2D heightData;
    public Texture2D imageData;
    public Texture2D vectorData;
    public Vector2 CenterLatLong;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setTileInfo(Vector2 _tileID, Rect _tileRect, int _zoom, float _relativeScale)
    {
        tileID = _tileID;
        tileRect = _tileRect;
        zoom = _zoom;
        relativeScale = _relativeScale;
        CenterLatLong = Mapbox.Conversions.TileIdToCenterLatitudeLongitude((int)_tileID.x, (int)_tileID.y, Config.zoom);
    }

    public void setRefernceTile()
    {
        isReferenceTile = true;
    }
}
