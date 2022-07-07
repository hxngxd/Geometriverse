using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPlane : MonoBehaviour
{
    Draw draw;
    Mesh mesh;
    Vector3 prev1, prev2, prev3;
    int[] triangles;
    string preName;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        mesh = this.GetComponent<MeshFilter>().mesh;
        triangles = new int[]{0,1,2,2,1,0};
    }
    void Update()
    {
        if (Hierarchy.Objs.ContainsKey(this.name)){
            var plane = Hierarchy.Objs[this.name];
            var v1 = Hierarchy.Objs[plane.vertices[0]].go.transform.position;
            var v2 = Hierarchy.Objs[plane.vertices[1]].go.transform.position;
            var v3 = Hierarchy.Objs[plane.vertices[2]].go.transform.position;
            if (preName != plane.name){
                preName = plane.name;
            }
            if (prev1 != v1 || prev2 != v2 || prev3 != v3){
                var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev3 - prev1, prev2 - prev1), Vector3.Cross(v3 - v1, v2 - v1));
                foreach (string child in plane.children){
                    if (Hierarchy.Objs[child].type == "point"){
                        var point = Hierarchy.Objs[child].go;
                        var pos = point.transform.position;
                        if (prev1 != v1){
                            point.transform.position = draw.calc.matrix_rotate(draw.calc.hc_diem_dt(pos, new KeyValuePair<Vector3, Vector3>(prev2, prev3)), pos, rotation);
                        }
                        else if (prev2 != v2){
                            point.transform.position = draw.calc.matrix_rotate(draw.calc.hc_diem_dt(pos, new KeyValuePair<Vector3, Vector3>(prev1, prev3)), pos, rotation);
                        }
                        else if (prev3 != v3){
                            point.transform.position = draw.calc.matrix_rotate(draw.calc.hc_diem_dt(pos, new KeyValuePair<Vector3, Vector3>(prev1, prev2)), pos, rotation);
                        }
                    }
                }
                prev1 = v1;
                prev2 = v2;
                prev3 = v3;
                plane.rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(v1), draw.calc.ztoy(v2), draw.calc.ztoy(v3));
                plane.equation = draw.calc.pt_mp(draw.calc.ztoy(v1), draw.calc.ztoy(v2), draw.calc.ztoy(v3));
                var vertices = new Vector3[]{v1, v2, v3};
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
                Hierarchy.Objs[this.name] = plane;
            }
        }
    }
}
