using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPlane : MonoBehaviour
{
    Draw draw;
    Mesh mesh;
    Vector3 prev1, prev2, prev3;
    bool preExpand;
    int[] triangles;
    string preName;
    Transform hierItem = null;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        mesh = this.GetComponent<MeshFilter>().mesh;
        triangles = new int[]{0,1,2,2,1,0};
    }
    void Update()
    {
        if (hierItem == null) hierItem = draw.hier.content.Find(this.name);
        else{
            if (Hierarchy.Planes.ContainsKey(this.name)){
                var plane = Hierarchy.Planes[this.name];
                var v1 = Hierarchy.Points[plane.vertices[0]].go.transform.position;
                var v2 = Hierarchy.Points[plane.vertices[1]].go.transform.position;
                var v3 = Hierarchy.Points[plane.vertices[2]].go.transform.position;
                if (preName != plane.name){
                    preName = plane.name;
                    hierItem.Find("Text").GetComponent<TextMeshProUGUI>().text = $"Mặt phẳng: {plane.name}";
                }
                if (prev1 != v1 || prev2 != v2 || prev3 != v3 || preExpand != plane.expand){
                    if (prev1 != v1){
                        var rotation = draw.calc.RM_Vectors(Vector3.zero, Vector3.Cross(prev3 - prev1, prev2 - prev1), Vector3.Cross(prev3 - v1, prev2 - v1));
                        foreach (string child in plane.children){
                            if (Hierarchy.Points.ContainsKey(child) && !plane.vertices.Contains(child)){
                                var point = Hierarchy.Points[child].go;
                                var pos = point.transform.position;
                                point.transform.position = draw.calc.MatrixRotate(draw.calc.HC_diem_len_duong_thang(pos, new KeyValuePair<Vector3, Vector3>(prev2, prev3)), pos, rotation);
                            }
                        }
                        prev1 = v1;
                    }
                    if (prev2 != v2){
                        var rotation = draw.calc.RM_Vectors(Vector3.zero, Vector3.Cross(prev3 - prev2, prev1 - prev2), Vector3.Cross(prev3 - v2, prev1 - v2));
                        foreach (string child in plane.children){
                            if (Hierarchy.Points.ContainsKey(child) && !plane.vertices.Contains(child)){
                                var point = Hierarchy.Points[child].go;
                                var pos = point.transform.position;
                                point.transform.position = draw.calc.MatrixRotate(draw.calc.HC_diem_len_duong_thang(pos, new KeyValuePair<Vector3, Vector3>(prev1, prev3)), pos, rotation);
                            }
                        }
                        prev2 = v2;
                    }
                    if (prev3 != v3){
                        var rotation = draw.calc.RM_Vectors(Vector3.zero, Vector3.Cross(prev2 - prev3, prev1 - prev3), Vector3.Cross(prev2 - v3, prev1 - v3));
                        foreach (string child in plane.children){
                            if (Hierarchy.Points.ContainsKey(child) && !plane.vertices.Contains(child)){
                                var point = Hierarchy.Points[child].go;
                                var pos = point.transform.position;
                                point.transform.position = draw.calc.MatrixRotate(draw.calc.HC_diem_len_duong_thang(pos, new KeyValuePair<Vector3, Vector3>(prev1, prev2)), pos, rotation);
                            }
                        }
                        prev3 = v3;
                    }
                    plane.rotation = draw.calc.RM_Plane_XY(draw.calc.swapYZ(v1), draw.calc.swapYZ(v2), draw.calc.swapYZ(v3));
                    plane.equation = draw.calc.plane_equation(draw.calc.swapYZ(v1), draw.calc.swapYZ(v2), draw.calc.swapYZ(v3));
                    var vertices = new Vector3[]{v1, v2, v3};
                    mesh.Clear();
                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    this.GetComponent<MeshCollider>().sharedMesh = mesh;
                    Hierarchy.Planes[this.name] = plane;
                }
            }
        }
    }
}
