﻿using UnityEngine;
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

    //public GameObject planeMeshPrefab;

    // 构造函数
    public CityQuadTree(float size, int depth, Vector2 center, CityQuadTree parent)
    {
        this.nodeSize = size;
        this.currentDepth = depth;
        this.nodeCenter = center;
        this.childNodes = new CityQuadTree[4];
        this.isLeaf = true;
        if (parent != null)
            this.nodeParent = parent;
        // else is root
        this.nodeBounds = new Rect(center.x - size / 2.0f, center.y - size / 2.0f, size, size);
    }

    public CityQuadTree(Rect worldRect, int depth, CityQuadTree parent)
    {
        this.nodeSize = worldRect.width;
        this.nodeCenter = worldRect.center;
        this.currentDepth = depth;
        this.childNodes = new CityQuadTree[4];
        this.isLeaf = true;
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
            Debug.Log("maxDepth node can not generate children");
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
            }
        }
        else
        {
            Debug.Log("search operation reaches a leaf");
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
            this.CreateMesh(root, planeMeshPrefab);
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
            this.childNodes[index].SortTileIntoLeaf(tile);
        }
        else
        {
            // 把该Rect tile分配到某个叶节点，把Texture等渲染信息加入该叶节点
            // TODO: 把Tile的HighMap与Texture添加进该节点
            // ...
            return this;
        }

        return null;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
