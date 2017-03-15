using UnityEngine;
using System.Collections;

public class textureTest : MonoBehaviour {
    public Texture2D[] atlasTexture;
    public GameObject[] objList;
    public Rect[] rects;

    public Shader standardShader;
	// Use this for initialization
	void Start () {
        Texture2D atlas = new Texture2D(256, 256);
        rects = atlas.PackTextures(atlasTexture, 0, 256, false);
        this.GetComponent<MeshRenderer>().material.mainTexture = atlas;
        Material standardMaterial = new Material(standardShader);

        for (int i = 0; i < objList.Length; i++)
        {
            GameObject obj = objList[i];
            Rect r = rects[i];

            obj.GetComponent<MeshRenderer>().sharedMaterial = standardMaterial;
            Material m = obj.GetComponent<MeshRenderer>().material;
            Debug.Log(m.name);
            m.mainTexture = atlas;
            m.SetTexture("_MainTex", atlas);
            m.SetTextureOffset("_MainTex", new Vector2(r.x, r.y));
            m.SetTextureScale("_MainTex", new Vector2(r.width, r.height));

            //m.mainTextureOffset = new Vector2(r.x, r.y);
            //m.mainTextureScale = new Vector2(r.width, r.height);


        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
