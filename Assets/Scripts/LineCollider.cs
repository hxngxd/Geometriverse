using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
    Vector3 previousMousePos;
    public void AddCollider(LineRenderer line){
        Mesh mesh = new Mesh();
        line.GetComponent<MeshFilter>().mesh = mesh;
        line.BakeMesh(mesh, Camera.main, false);
        line.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void RebuildCollider(LineRenderer line){
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) previousMousePos = Input.mousePosition;
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)){
            if (previousMousePos != Input.mousePosition){
                AddCollider(line);
            }
        }
    }
}
