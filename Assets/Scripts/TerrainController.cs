using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {
    public Rect worldRect;  // 8*8 tile rect, 墨卡托坐标
    public CityQuadTree qTree;
    public GameObject planeMeshPrefab;

    private int tileNum = 8;  // 现场景中每边tile数
    

    private GameObject terrainRoot;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("update quadtree");
            UpDateTerrain();
        }
	}

    private void InitQuadTree()
    {
        InitWorldRect();
        terrainRoot = new GameObject("terrain-root");
        terrainRoot.transform.position = new Vector3(0, 0, 0);
        terrainRoot.transform.localScale = Vector3.one * BuildingGeoList.GetWorldScaleFactor();

        qTree = new CityQuadTree(worldRect, 0, null);
        qTree.InitSearchTarget(new Rect(BuildingGeoList.GetRerenceRect().x, BuildingGeoList.GetRerenceRect().y, BuildingGeoList.GetRerenceRect().width + 100, BuildingGeoList.GetRerenceRect().height - 100), terrainRoot, planeMeshPrefab);
        

    }

    void InitWorldRect()
    {
        Vector2 worldCenter = BuildingGeoList.GetRerenceLeftBottomInMeters();  // worldRect center，墨卡托坐标
        float tileSize = BuildingGeoList.tileSizeInMeters();
        float worldRectX = worldCenter.x - tileSize * tileNum/2;
        float worldRectY = worldCenter.y - tileSize * tileNum/2; // 左上角
        worldRect = new Rect(worldRectX, worldRectY, tileSize * tileNum, tileSize * tileNum);
        
    }

    public void UpDateTerrain()
    {
        //GameObject terrainRoot2 = new GameObject("terrainRoot2");
        //qTree.UpdateSearchTarget(new Rect(1487770, 6899512, 611.5f, 611.5f), terrainRoot, planeMeshPrefab);
        qTree.UpdateSearchTarget(new Rect(1488382, 6898289, 611.5f, 611.5f), terrainRoot, planeMeshPrefab);
        //terrainRoot.transform.localScale = Vector3.one * BuildingGeoList.GetWorldScaleFactor();
    }

    public void UpdateTerrain(Rect newRect)
    {
        //Debug.Log("terrain controller: update terrain");
        qTree.UpdateSearchTarget(newRect, terrainRoot, planeMeshPrefab);
        terrainRoot.transform.localScale = Vector3.one * BuildingGeoList.GetWorldScaleFactor(); // ?
    }

    public int getTheTileDepth(GameObject tile)
    {
        if(qTree == null)
        {
            InitQuadTree();
        }
        return qTree.AssignMapTileIntoLeaf(tile);
    }
}
