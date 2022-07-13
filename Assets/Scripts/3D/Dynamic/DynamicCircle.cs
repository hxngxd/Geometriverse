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
        linecollider.AddCollider(line);
    }
    void Update()
    {
        if (Hierarchy.Objs.ContainsKey(this.name)){
            var obj = Hierarchy.Objs[this.name];
            bool diff = false;
            for (int i=0;i<obj.vertices.Count;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
            }
            if (diff){
                if (obj.vertices.Count == 3){
                    // var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev[2] - prev[0], prev[1] - prev[0]), Vector3.Cross(v[2] - v[0], v[1] - v[0]));
                    for (int i=0;i<3;i++){
                        prev[i] = v[i];
                        v[i] = draw.calc.ztoy(v[i]);
                    }
                    if (v[0] != v[1] && v[1] != v[2] && v[2] != v[0] && Mathf.Abs(Vector3.Dot((v[1]-v[0]).normalized, (v[2]-v[0]).normalized)) != 1){
                        var center = draw.calc.tam_dg_tron_ngtiep(v[0], v[1], v[2]);
                        obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
                        line.SetPositions(draw.calc.dinh_da_giac(center, v[2], 180, obj.rotation));
                    }
                    Hierarchy.Objs[this.name] = obj;
                }
                else{
                    for (int i=0;i<3;i++){
                        prev[i] = v[i];
                        v[i] = draw.calc.ztoy(v[i]);
                    }
                    if (v[0] != v[1]){
                        line.SetPositions(draw.calc.dinh_da_giac(v[0], v[1], 180, Hierarchy.Objs[obj.parent].rotation));
                    }
                }
            }
            linecollider.RebuildCollider(line);
        }
    }
}
