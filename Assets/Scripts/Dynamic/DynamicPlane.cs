using System.Collections;
using System;
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
    bool preExpand;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        mesh = this.GetComponent<MeshFilter>().mesh;
        triangles = new int[]{0,1,2,2,1,0};
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));
        var obj = Hierarchy.Objs[this.name];
        for (int i=0;i<3;i++){
            prev[i] = v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
        }
        mesh.Clear();
        mesh.vertices = v;
        mesh.triangles = triangles;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
        
        while (true){
            obj = Hierarchy.Objs[this.name];
            bool diff = false;
            for (int i=0;i<3;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
                draw.hier.ItemLabeling(this.name);
            }
            if (diff){
                if (v[0] != v[1] && v[1] != v[2] && v[2] != v[0] && Mathf.Abs(Vector3.Dot((v[1]-v[0]).normalized, (v[2]-v[0]).normalized)) != 1){
                    try{
                        var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev[2] - prev[0], prev[1] - prev[0]), Vector3.Cross(v[2] - v[0], v[1] - v[0]));
                        foreach (string child in obj.children){
                            if (Hierarchy.Objs[child].type == "point"){
                                var parent = Hierarchy.Objs[child].parents[0];
                                if (Hierarchy.Objs[parent].type != "polygon"){
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
                        }
                    }
                    catch {}
                }
                mesh.Clear();
                mesh.vertices = v;
                mesh.triangles = triangles;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
                for (int i=0;i<3;i++){
                    prev[i] = v[i];
                    v[i] = draw.calc.ztoy(v[i]);
                }
                obj.equation = draw.calc.pt_mp(v[0], v[1], v[2]);
                obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
                Hierarchy.Objs[this.name] = obj;
            }
            if (preExpand != obj.expand){
                preExpand = obj.expand;
                if (obj.expand){
                    var center = draw.calc.ztoy(draw.calc.tam_dg_tron_ngtiep(v[0], v[1], v[2]));
                    var dist = 10000;
                    v = draw.calc.dinh_da_giac(center, draw.calc.ztoy(v[0]), 3, obj.rotation);
                    for (int i=0;i<3;i++){
                        v[i] = draw.calc.kc_sang_toa_do(draw.calc.ztoy(center), v[i], dist);
                    }
                }
                mesh.Clear();
                mesh.vertices = v;
                mesh.triangles = triangles;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            yield return null;
        }
    }
}
