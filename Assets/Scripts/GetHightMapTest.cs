using UnityEngine;
using System.Collections;

public class GetHightMapTest : MonoBehaviour {
    GameObject worldRoot;
    bool findWorldRoot = false;
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
                Texture2D hightMap = firstTile.GetComponent<Mapbox.MeshGeneration.Data.UnityTile>().ImageData;
                if(hightMap)
                {
                    Debug.Log("find root! and find hightmap");
                    findWorldRoot = true;
                    GetComponent<Renderer>().material.mainTexture = hightMap;
                }
                else
                {
                    //Debug.Log("find root but not find hight map");
                }

                
            }
        }
	}
}
