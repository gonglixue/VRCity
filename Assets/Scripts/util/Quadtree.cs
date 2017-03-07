using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Quadtree
{
    static int childCount = 4;
    static int maxObjectCount = 100;
    static int maxDepth;

    // used for visual debugging / demonstration
    private bool searched = false;
    private Quadtree nodeParent;
    private Quadtree[] childNodes;

    private List<GameObject> objects = new List<GameObject>();  // root node is going to contains a list of objects
    private int currentDepth = 0;
    private Vector2 nodeCenter;
    private Rect nodeBounds = new Rect();

    private float nodeSize = 0f;  // ? 在坐标系中的长宽的一半？

    public Quadtree(float worldSize, int maxNodeDepth, int maxNodeObjects, Vector2 center)
    {
        maxDepth = maxNodeDepth;
        maxObjectCount = maxNodeObjects;
    }

    private Quadtree(float size, int depth, Vector2 center, Quadtree parent)
    {
        this.nodeSize = size;
        this.currentDepth = depth;
        this.nodeCenter = center;
        this.nodeParent = parent;

        if(this.currentDepth == 0)  //是根节点
        {
            this.nodeBounds = new Rect(center.x - size, center.y - size, size * 2, size * 2);
        }
        else
        {
            this.nodeBounds = new Rect(center.x - (size / 2), center.y - (size / 2), size, size);
        }
    }

    public bool Add(GameObject go)
    {
        if(this.nodeBounds.Contains(go.transform.position))
        {
            //return this.Add(go, new Vector2(go.transform.position.x, go.transform.position.y));
            return true;
        }
        return false;
    }

    private Quadtree Add(GameObject obj, Vector2 objCenter)
    {
        if(this.childNodes != null)
        {
            // four nodes
            //  ^z plus
            // ------
            //  2 | 3
            // ------ > x plus
            //  0 | 1  
            // ------
            int index = (objCenter.x < this.nodeCenter.x ? 0 : 1)
                + (objCenter.y < this.nodeCenter.y ? 0 : 2);
            return this.childNodes[index].Add(obj, objCenter);  // 返回一个Quadtree节点
        }
        return this.childNodes[0].Add(obj, objCenter);
    }
}
