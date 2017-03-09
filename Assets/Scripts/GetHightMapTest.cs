using UnityEngine;
using System.Collections;
using Mapbox.MeshGeneration.Data;
using Mapbox.Scripts.Utilities;
using Mapbox.Scripts.UI;

public class GetHightMapTest : MonoBehaviour {
    GameObject worldRoot;
    bool findWorldRoot = false;
    public Texture2D heightData;

    private Vector3[] vertices;
    private int[] indices;
    private Mesh mesh;
    private Vector2[] uv;
    private double latitude = 52.55131;

    public GameObject MeshPrefab;

    // reshow mapvisualization test

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(!findWorldRoot)
        {
            worldRoot = GameObject.Find("worldRoot");
            if(worldRoot)
            {
                
                GameObject firstTile = worldRoot.transform.GetChild(0).gameObject;
                Texture2D hightMap = firstTile.GetComponent<Mapbox.MeshGeneration.Data.UnityTile>().HeightData;
                Texture2D ImageData = firstTile.GetComponent<Mapbox.MeshGeneration.Data.UnityTile>().ImageData;
                if(hightMap && ImageData)
                {
                    Debug.Log("find root! and find hightmap");
                    findWorldRoot = true;
                    GetComponent<Renderer>().material.mainTexture = ImageData;
                    heightData = hightMap;
                    ChangeHeight();
                }
                else
                {
                    //Debug.Log("find root but not find hight map");
                }

                
            }
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 更改MeshFileter的mesh
            GameObject firstTile = worldRoot.transform.GetChild(0).gameObject;
            firstTile.GetComponent<MeshFilter>().sharedMesh = MeshPrefab.GetComponent<MeshFilter>().sharedMesh;

            Debug.Log("change mesh");
        }
	}

    void ChangeHeight()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        indices = mesh.triangles;
        uv = mesh.uv;
        transform.localScale = new Vector3(10,1,10);
        // 遍历每个点的y坐标
        for(int i=0;i<vertices.Length;i++)
        {
            float oldY = vertices[i].y;
            Vector2 uv_cor = uv[i];
            float RelativeScale = Mapbox.Conversions.GetTileScaleInMeters(0, Config.zoom) / Mapbox.Conversions.GetTileScaleInMeters((float)latitude, Config.zoom);

            float height = Mapbox.Conversions.GetRelativeHeightFromColor(heightData.GetPixel(
                        (int)Mathf.Clamp((uv_cor.x * 256), 0, 255),
                        (int)Mathf.Clamp((uv_cor.y * 256), 0, 255)), RelativeScale);
            vertices[i].y = (float)(oldY + Config.worldFactor*height);
            //Debug.Log("config factor:" + Config.worldFactor);
            if (i < 5)
                Debug.Log(vertices[i]);
        }

        mesh.vertices = vertices;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
