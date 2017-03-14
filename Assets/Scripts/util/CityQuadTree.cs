using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityQuadTree{
    static int childCount = 4;
    static int maxNodeCount = 100;
    static int sideLength = 8;  // 一边有8个tiles
    static int maxDepth = 3; // log8
    static int sampleCount = 40;


    private CityQuadTree nodeParent;
    private CityQuadTree[] childNodes;

    private Rect nodeBounds = new Rect();  // 墨卡托坐标
    private int currentDepth = 0;
    private Vector2 nodeCenter;  // 墨卡托坐标
    private float nodeSize; // 边长，墨卡托坐标
    private bool isLeaf;

    // 每个leaf都有以下属性
    public Mesh aMesh;  
    public Texture2D aTexture;
    public Texture2D aHeightMap;
    private GameObject LODTile;

    // 每个leaf都有以下属性
    // private List<GameObject> mapTile;

    //public GameObject planeMeshPrefab;

    // 构造函数
    public CityQuadTree(float size, int depth, Vector2 center, CityQuadTree parent)
    {
        this.nodeSize = size;
        this.currentDepth = depth;
        this.nodeCenter = center;
        this.childNodes = new CityQuadTree[4];
        this.isLeaf = true;
        this.LODTile = null;
        //this.mapTile = new List<GameObject>();
        if (parent != null)
            this.nodeParent = parent;
        // else is root
        this.nodeBounds = new Rect(center.x - size / 2.0f, center.y - size / 2.0f, size, size);  // 左上角 + 正height
    }

    public CityQuadTree(Rect worldRect, int depth, CityQuadTree parent)
    {
        this.nodeSize = worldRect.width;
        this.nodeCenter = worldRect.center;
        this.currentDepth = depth;
        this.childNodes = new CityQuadTree[4];
        this.isLeaf = true;
        this.LODTile = null;
        //this.mapTile = new List<GameObject>();
        if (parent != null)
            this.nodeParent = parent;

        this.nodeBounds = worldRect;
    }

    private void GenerateChildNodes()
    {
        // children index
        // ------
        //  2 | 3
        // ------ 
        //  0 | 1  
        // ------
        if(this.currentDepth >= CityQuadTree.maxDepth)
        {
            //Debug.Log("maxDepth node can not generate children");
            return;
        }

        float childNodeSize = this.nodeSize / 2.0f;
        for(int i=0;i<4;i++)
        {
            float deltaX = childNodeSize * ((i == 0 || i == 2) ? 0 : 1);
            float deltaY = childNodeSize * ((i == 0 || i == 1) ? 0 : 1);
            Vector2 childLeftBottom = new Vector2(this.nodeBounds.x + deltaX, this.nodeBounds.y + deltaY);
            Rect childRect = new Rect(childLeftBottom, new Vector2(childNodeSize, childNodeSize));
            CityQuadTree child = new CityQuadTree(childRect, this.currentDepth + 1, this);
            this.childNodes[i] = child;
        }

        this.isLeaf = false;
    }

    public void SearchTarget(Vector2 myPos)  // 参数为墨卡托坐标
    {
        if(this.currentDepth < CityQuadTree.maxDepth)
        {
            int index = (myPos.x < this.nodeCenter.x ? 0 : 1)
                    + (myPos.y < this.nodeCenter.y ? 0 : 2);
            this.GenerateChildNodes();
            if (this.childNodes[index].nodeBounds.Contains(myPos))
                this.childNodes[index].SearchTarget(myPos);
            else
                Debug.Log("ERROR! THIS IS A BUG! INDEX CALCULATION EXISTS ERROR");  
        }
        else
        {
            Debug.Log("Search Operation Reaches A Leaf");
        }
    }

    public void SearchTarget(Rect myPos)  // 参数为墨卡托坐标下的一个Rect
    {
        if(this.currentDepth < CityQuadTree.maxDepth)
        {
            this.GenerateChildNodes();
            foreach(CityQuadTree child in this.childNodes)
            {
                if(TestRectInter(child.nodeBounds,myPos))
                {
                    child.SearchTarget(myPos);
                }
                else
                {
                    // TODO: 不相交的节点是叶子节点(default)， 为子节点child创建GameObject，清除原来的GameObject;
                    // ...
                }
            }
        }
        else
        {
            Debug.Log("search operation reaches a leaf");
        }
    }

    public void InitSearchTarget(Rect myPos, GameObject root, GameObject planeMeshPrefab)
    {
        if(this.currentDepth < CityQuadTree.maxDepth)
        {
            this.GenerateChildNodes();
            foreach(CityQuadTree child in this.childNodes)
            {
                if (TestRectInter(child.nodeBounds, myPos))  // 相交
                {
                    child.InitSearchTarget(myPos, root, planeMeshPrefab);
                }
                else  // 不相交,直接作为叶子节点处理
                {
                    child.AddLODTile(root, planeMeshPrefab);
                    //child.DestroyGameObject();
                    child.isLeaf = true;                   
                }
            }
        }
        else   //达到最大深度不再细分，当前节点作为叶子节点处理
        {
            this.AddLODTile(root, planeMeshPrefab);
            this.isLeaf = true;
        }
    }

    public void UpdateSearchTarget(Rect myPos, GameObject root, GameObject planeMeshPrefab)
    {
        if (this.currentDepth < CityQuadTree.maxDepth)
        {
            //Debug.Log("update search target");
            //this.GenerateChildNodes();
            foreach (CityQuadTree child in this.childNodes)
            {
                if (TestRectInter(child.nodeBounds, myPos))  // 相交
                {
                    if (child.isLeaf)
                    {
                        child.GenerateChildNodes();
                        if (child.currentDepth == maxDepth - 1)
                        {
                            //当达到最深一层的时候，为child的每个child创建Mesh
                            foreach (CityQuadTree grandchild in child.childNodes)
                            {
                                grandchild.AddLODTile(root, planeMeshPrefab);
                            }
                        }
                    }
                        
                    child.UpdateSearchTarget(myPos, root, planeMeshPrefab);
                    // 完成重新剖分后，Destroy原来的Tile, myPos未改变的情况也适用
                    if(child.LODTile != null && child.currentDepth!=maxDepth)
                        GameObject.Destroy(child.LODTile);
                }
                else  // 不相交
                {
                    // TODO: 不相交的节点是叶子节点(default), 为子节点child创建GameObject, 创建完成后Destroy child下所有leaf原来的GameObject
                    // 如果原来就是leaf且是不是新generate出来的（即它是叶子节点且原先就存在一个LODTile），那么无需做更新??????
                    if (child.isLeaf && child.LODTile!=null)
                    {
                        // do nothing
                    }
                    else if(child.isLeaf && child.LODTile == null)  // 是新generate的节点
                    {
                        child.AddLODTile(root, planeMeshPrefab);
                    }
                    else
                    {
                        child.AddLODTile(root, planeMeshPrefab);
                        child.DestroyGameObject();
                        child.isLeaf = true;
                    }


                }
            }
        }
    }

    // 判断两矩形是否相交
    public bool TestRectInter(Rect r1, Rect r2)
    {
        return r1.Overlaps(r2,true);
    }

    public void Traversal(GameObject root, GameObject planeMeshPrefab)  // 遍历整棵树，为叶子节点创建Mesh实例,Mesh实例作为root的子元素
    {
        //Debug.Log("depth " + this.currentDepth + " size:" + this.nodeBounds.width + "or:" + this.nodeSize);
        if (!this.isLeaf)
        {
            foreach(CityQuadTree treeNode in this.childNodes)
            {
                treeNode.Traversal(root, planeMeshPrefab);
            }
        }
        else
        {
            //this.CreateMesh(root, planeMeshPrefab);
        }
    }

    private void CreateMesh(GameObject root, GameObject planeMeshPrefab)
    {
        Vector2 referenceO = BuildingGeoList.GetReferenceCenterInMeters();

        Vector3 position = new Vector3(this.nodeCenter.x-referenceO.x, 0, this.nodeCenter.y-referenceO.y);
        //GameObject leafPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObject leafPlane = GameObject.Instantiate(planeMeshPrefab);
        leafPlane.name = "depth-" + this.currentDepth;
        leafPlane.transform.position = position;
        leafPlane.transform.localScale = (new Vector3(1,0,1)) * this.nodeSize * 0.1f;  // 墨卡托坐标. 一个plan primitive本身的unity size是10*10
        leafPlane.transform.SetParent(root.transform);
        leafPlane.AddComponent<MeshCollider>().sharedMesh = leafPlane.GetComponent<MeshFilter>().mesh;
    }

    private void AddLODTile(GameObject root, GameObject planeMeshPrefab)
    {
        /*
        this.LODTile = GameObject.Instantiate(planeMeshPrefab);

        Vector2 reference0 = BuildingGeoList.GetReferenceCenterInMeters();
        Vector3 position = new Vector3(this.nodeCenter.x - reference0.x, 0, this.nodeCenter.y - reference0.y);  // 墨卡托坐标下的位置
        this.LODTile.name = "depth-" + this.currentDepth;
        
        
        this.LODTile.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
        //this.LODTile.transform.localScale = (new Vector3(1, 0, 1)) * this.nodeSize * 0.1f;
        this.LODTile.transform.localScale = root.transform.localScale * this.nodeSize * 0.1f;
        this.LODTile.transform.position = position * root.transform.localScale.x;
        this.LODTile.transform.SetParent(root.transform);
        this.LODTile.AddComponent<MeshCollider>().sharedMesh = this.LODTile.GetComponent<MeshFilter>().mesh;
        */
    }

    private void DestroyGameObject()
    {
        foreach(CityQuadTree child in this.childNodes)
        {
            if (child == null)
                return;
            if(child.isLeaf)//如果是叶子节点，那么就Destroy它的GameObject
            {
                if(child.LODTile != null)
                {
                    Debug.Log("destroy");
                    GameObject.Destroy(child.LODTile);
                }
            }
            else
            {
                child.DestroyGameObject();
            }
        }
        
    }

    public CityQuadTree SortTileIntoLeaf(Rect tile)
    {
        if(!TestRectInter(this.nodeBounds, tile))
        {
            Debug.Log("ERROR THIS QUADTREE NODE DOES NOT CONTAIN THE TILE RECT");
            return null;
        }
        if(!this.isLeaf)
        {
            Vector2 tileCenter = tile.center;
            int index = (tileCenter.x < this.nodeCenter.x ? 0 : 1) + (tileCenter.y < this.nodeCenter.y ? 0 : 2);
            return this.childNodes[index].SortTileIntoLeaf(tile);
        }
        else
        {
            // 把该Rect tile分配到某个叶节点，把Texture等渲染信息加入该叶节点
            // TODO: 把Tile的HighMap与Texture添加进该节点
            // ...
            return this;
        }
    }

    public int AssignMapTileIntoLeaf(GameObject tile)
    {
        Rect tileRect = tile.GetComponent<TileIntro>().tileRect;
        if(!TestRectInter(this.nodeBounds, tileRect))
        {
            Debug.Log("ERROR THIS QUADTREE NODE DOES NOT CONTAIN THE TILE RECT");
            return -1;
        }
        else
        {
            if (!this.isLeaf)  // 如果当前节点不是叶子节点，就在它的子节点中继续搜索，一个tileRect只能属于一个quadtree node
            {
                int index = (tileRect.center.x < this.nodeCenter.x ? 0 : 1) + (tileRect.center.y < this.nodeCenter.y ? 0 : 2);
                return this.childNodes[index].AssignMapTileIntoLeaf(tile);
            }
            else
            {
                // 分配到了叶子节点
                //this.mapTile.Add(tile);
                // 绘制
                return this.currentDepth;
            }
        }

    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
