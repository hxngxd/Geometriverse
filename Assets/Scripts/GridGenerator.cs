using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int gridSize = 50;
    public float cellSize = 1.0f;
    public Material gridMaterial;

    private Mesh mesh;

    private void Start()
    {
        GenerateGrid();
        transform.localPosition = new Vector3(-gridSize/2f, 0, -gridSize/2f);
    }

    private void GenerateGrid()
    {
        mesh = new Mesh();

        int numLines = gridSize + 1;
        float gridWidth = gridSize * cellSize;

        Vector3[] vertices = new Vector3[numLines * 4];
        int[] indices = new int[numLines * 4];

        for (int i = 0; i < numLines; i++)
        {
            float position = i * cellSize;

            // Vertical lines
            vertices[i * 4] = new Vector3(position, 0, 0);
            vertices[i * 4 + 1] = new Vector3(position, 0, gridWidth);

            // Horizontal lines
            vertices[i * 4 + 2] = new Vector3(0, 0, position);
            vertices[i * 4 + 3] = new Vector3(gridWidth, 0, position);

            indices[i * 4] = i * 4;
            indices[i * 4 + 1] = i * 4 + 1;
            indices[i * 4 + 2] = i * 4 + 2;
            indices[i * 4 + 3] = i * 4 + 3;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = gridMaterial;
    }
}
