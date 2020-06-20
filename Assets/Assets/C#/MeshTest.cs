using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshTest : MonoBehaviour
{
    [SerializeField]
    Transform point1;
    [SerializeField]
    Transform point2;
    [SerializeField]
    Transform point3;
    [SerializeField]
    MeshFilter meshFilter;

    Mesh mesh;
    Vector3[] points;
    int[] triangles;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Test Mesh";
        mesh.Clear();
        triangles = new int[] { 0, 1, 2 };
        meshFilter.mesh = mesh;

    }

    // Update is called once per frame
    void Update()
    {
        points = new Vector3[] { point1.position, point2.position, point3.position };
        mesh.vertices = points;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
