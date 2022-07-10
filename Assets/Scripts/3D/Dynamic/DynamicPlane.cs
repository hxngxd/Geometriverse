using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPlane : MonoBehaviour
{
    Draw draw;
    Mesh mesh;
    Vector3[] prev = new Vector3[3], v = new Vector3[3];
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
            var obj = Hierarchy.Objs[this.name];
            bool diff = false;
            for (int i=0;i<3;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
            }
            if (diff){
                var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev[2] - prev[0], prev[1] - prev[0]), Vector3.Cross(v[2] - v[0], v[1] - v[0]));
                foreach (string child in obj.children){
                    if (Hierarchy.Objs[child].type == "point"){
                        var point = Hierarchy.Objs[child].go;
                        var pos = point.transform.position;
                        var rotateLine = new KeyValuePair<Vector3, Vector3>();
                        if (prev[0] != v[0]){
                            rotateLine = new KeyValuePair<Vector3, Vector3>(prev[1], prev[2]);
                        }
                        else if (prev[1] != v[1]){
                            rotateLine = new KeyValuePair<Vector3, Vector3>(prev[0], prev[2]);
                        }
                        else if (prev[2] != v[2]){
                            rotateLine = new KeyValuePair<Vector3, Vector3>(prev[0], prev[1]);
                        }
                        point.transform.position = draw.calc.matrix_rotate(draw.calc.hc_diem_dt(pos, rotateLine), pos, rotation);

                    }
                }
                var vertices = new Vector3[]{v[0], v[1], v[2]};
                for (int i=0;i<3;i++){
                    prev[i] = v[i];
                    v[i] = draw.calc.ztoy(v[i]);
                }
                obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
                obj.equation = draw.calc.pt_mp(v[0], v[1], v[2]);
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
                Hierarchy.Objs[this.name] = obj;
            }
        }
    }
}
