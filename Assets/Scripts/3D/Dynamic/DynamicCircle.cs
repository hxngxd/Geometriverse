using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCircle : MonoBehaviour
{
    LineRenderer line;
    LineCollider linecollider;
    Draw draw;
    Vector3 prev1, prev2, prev3;
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
            var circle = Hierarchy.Objs[this.name];
            var v1 = Hierarchy.Objs[circle.vertices[0]].go.transform.position;
            var v2 = Hierarchy.Objs[circle.vertices[1]].go.transform.position;
            var v3 = Hierarchy.Objs[circle.vertices[2]].go.transform.position;
            if (preName != circle.name){
                preName = circle.name;
            }
            if (prev1 != v1 || prev2 != v2 || prev3 != v3){
                // var rotation = draw.calc.rm_vectors(Vector3.zero, Vector3.Cross(prev3 - prev1, prev2 - prev1), Vector3.Cross(v3 - v1, v2 - v1));
                prev1 = v1;
                prev2 = v2;
                prev3 = v3;
                if (v1 != v2 && v2 != v3 && v3 != v1 && Mathf.Abs(Vector3.Dot((v2-v1).normalized, (v3-v1).normalized)) != 1){
                    var center = draw.calc.tam_dg_tron_ngtiep(draw.calc.ztoy(v1), draw.calc.ztoy(v2), draw.calc.ztoy(v3));
                    circle.rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(v1), draw.calc.ztoy(v2), draw.calc.ztoy(v3));
                    draw.calc.dinh_da_giac(line, center, draw.calc.ztoy(v3), 90, circle.rotation);
                }
                Hierarchy.Objs[this.name] = circle;
            }
            linecollider.RebuildCollider(line);
        }
    }
}
