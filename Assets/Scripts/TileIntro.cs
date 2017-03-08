using UnityEngine;
using System.Collections;

public class TileIntro : MonoBehaviour {
    public Vector2 tileID;
    public Rect tileRect;  //墨卡托坐标下的Rect
    public int zoom;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setTileInfo(Vector2 _tileID, Rect _tileRect, int _zoom)
    {
        tileID = _tileID;
        tileRect = _tileRect;
        zoom = _zoom;
    }
}
