using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour {
    // http://www.ceeger.com/forum/read.php?tid=20296

    public Vector3 point = Vector3.up;
    public int numberOfPoints = 10;

    private Vector3[] vertices;
    private int[] triangles; // index
    private Mesh mesh;

    // Use this for initialization
    void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "starMesh";

        vertices = new Vector3[numberOfPoints + 1];
        triangles = new int[numberOfPoints * 3];

        float angle = -360f / numberOfPoints;
        for(int i=1,t=1;i<vertices.Length;i++,t+=3)
        {
            vertices[i] = Quaternion.Euler(0, 0, angle * (i - 1)) * point;
            triangles[t] = i;
            triangles[t + 1] = i + 1;
        }
        triangles[triangles.Length - 1] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
