using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int Grid_Size;
    public float Step;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;
    List<Vector3> Vertices;
    List<int> Indicies;

    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        meshFilter = this.GetComponent<MeshFilter>();
        GenerateGrid();
    }

    void Update()
    {
        var CameraPosition = Camera.main.transform.position;
        var currentPosition = this.transform.parent.position;
        currentPosition.x = (int)CameraPosition.x;
        currentPosition.z = (int)CameraPosition.z;
        this.transform.parent.position = currentPosition;
    }

    void GenerateGrid(){
        Vertices = new List<Vector3>();
        Indicies = new List<int>();
        mesh = new Mesh();
        int j = 0;
        for (float i=0;i<=Grid_Size;i+=Step){
            Vertices.Add(new Vector3(i, 0, 0));
            Vertices.Add(new Vector3(i, 0, Grid_Size));

            Indicies.Add(4 * j + 0);
            Indicies.Add(4 * j + 1);

            Vertices.Add(new Vector3(0, 0, i));
            Vertices.Add(new Vector3(Grid_Size, 0, i));

            Indicies.Add(4 * j + 2);
            Indicies.Add(4 * j + 3);

            j++;
        }
        mesh.vertices = Vertices.ToArray();
        mesh.SetIndices(Indicies.ToArray(), MeshTopology.Lines, 0);
        meshFilter.mesh = mesh;
        
        var currentPosition = this.transform.position;
        currentPosition.x = currentPosition.z = -(int)Grid_Size/2;
        this.transform.position = currentPosition;
    }
}
