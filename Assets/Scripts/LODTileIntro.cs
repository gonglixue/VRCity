using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LODTileIntro : MonoBehaviour {
    Rect tileRect;  // 墨卡托坐标
    int depth;      // 在quadTree下的深度
    List<Texture2D> imageData;  // 纹理贴图
    List<Texture2D> heightData;  // 高度图
    float tileSize;   // 墨卡托坐标下的size

    [SerializeField]
    float unitySize;

	// Use this for initialization
	void Start () {
        imageData = new List<Texture2D>();
        heightData = new List<Texture2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int AddTextureImage(Texture2D image)
    {
        imageData.Add(image);
        return imageData.Count;
    }

    public int AddHeightImage(Texture2D heightMap)
    {
        heightData.Add(heightMap);
        return heightData.Count;
    }
}
