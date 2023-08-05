using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DynamicCircle : MonoBehaviour
{
    LineRenderer line;
    LineCollider linecollider;
    Draw draw;
    Vector3[] prev = new Vector3[3], v = new Vector3[3];
    string preName;
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));
        var obj = Hierarchy.Objs[this.name];
        for (int i=0;i<obj.vertices.Count;i++){
            prev[i] = v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
        }

        if (obj.vertices.Count == 3){
            var center = draw.calc.tam_dg_tron_ngtiep(draw.calc.ztoy(v[0]), draw.calc.ztoy(v[1]), draw.calc.ztoy(v[2]));
            if (obj.parents.Count == 0){
                obj.rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(v[0]), draw.calc.ztoy(v[1]), draw.calc.ztoy(v[2]));
            }
            else{
                obj.rotation = Hierarchy.Objs[obj.parents[0]].rotation;
            }
            line.SetPositions(draw.calc.dinh_da_giac(center, draw.calc.ztoy(v[0]), 180, obj.rotation));
        }
        else{
            line.SetPositions(draw.calc.dinh_da_giac(draw.calc.ztoy(v[0]), draw.calc.ztoy(v[1]), 180, Hierarchy.Objs[obj.parents[0]].rotation));
        }

        linecollider.AddCollider(line);
        while (true){
            obj = Hierarchy.Objs[this.name];
            bool diff = false;
            for (int i=0;i<obj.vertices.Count;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
                draw.hier.ItemLabeling(this.name);
            }
            if (diff){
                if (obj.vertices.Count == 3){
                    for (int i=0;i<3;i++){
                        prev[i] = draw.calc.ztoy(prev[i]);
                        v[i] = draw.calc.ztoy(v[i]);
                    }
                    if (v[0] != v[1] && v[1] != v[2] && v[2] != v[0] && Mathf.Abs(Vector3.Dot((v[1]-v[0]).normalized, (v[2]-v[0]).normalized)) != 1){
                        var center = draw.calc.tam_dg_tron_ngtiep(v[0], v[1], v[2]);
                        try{
                            var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev[2] - prev[0], prev[1] - prev[0]), Vector3.Cross(v[2] - v[0], v[1] - v[0]));
                            foreach (string child in obj.children){
                                if (Hierarchy.Objs[child].type == "point"){
                                    var point = Hierarchy.Objs[child].go;
                                    var pos = draw.calc.ztoy(point.transform.position);
                                    if (obj.parents.Count == 0){
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
                                        point.transform.position = draw.calc.ztoy(draw.calc.kc_sang_toa_do(center, draw.calc.matrix_rotate(draw.calc.hc_diem_dt(pos, rotateLine), pos, rotation), Vector3.Distance(center, v[0])));
                                    }
                                    else{
                                        point.transform.position = draw.calc.ztoy(draw.calc.kc_sang_toa_do(center, pos, Vector3.Distance(center, v[0])));
                                    }
                                }
                            }
                        }
                        catch{
                            
                        }
                        if (obj.parents.Count == 0){
                            obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
                        }
                        else{
                            obj.rotation = Hierarchy.Objs[obj.parents[0]].rotation;
                        }
                        line.SetPositions(draw.calc.dinh_da_giac(center, v[0], 180, obj.rotation));
                    }
                    for (int i=0;i<3;i++){
                        v[i] = draw.calc.ztoy(v[i]);
                        prev[i] = v[i];
                    }
                }
                else{
                    for (int i=0;i<2;i++){
                        v[i] = draw.calc.ztoy(v[i]);
                    }
                    if (v[0] != v[1]){
                        foreach (string child in obj.children){
                            if (Hierarchy.Objs[child].type == "point"){
                                var point = Hierarchy.Objs[child].go;
                                var pos = draw.calc.ztoy(point.transform.position);
                                point.transform.position = draw.calc.ztoy(draw.calc.kc_sang_toa_do(v[0], pos, Vector3.Distance(v[0], v[1])));
                            }
                        }
                        line.SetPositions(draw.calc.dinh_da_giac(v[0], v[1], 180, Hierarchy.Objs[obj.parents[0]].rotation));
                    }
                    for (int i=0;i<2;i++){
                        v[i] = draw.calc.ztoy(v[i]);
                        prev[i] = v[i];
                    }
                }
            }
            Hierarchy.Objs[this.name] = obj;
            linecollider.RebuildCollider(line);
            yield return null;
        }
    }
}
