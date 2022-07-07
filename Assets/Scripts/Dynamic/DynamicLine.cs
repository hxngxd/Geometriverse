using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicLine : MonoBehaviour
{
    LineRenderer line;
    LineCollider linecollider;
    Draw draw;
    Vector3 preStart, preEnd;
    string preName;
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
    }

    void Update()
    {
        if (Hierarchy.Objs.ContainsKey(this.name)){
            var obj = Hierarchy.Objs[this.name];
            var start = Hierarchy.Objs[obj.vertices[0]].go.transform.position;
            var end = Hierarchy.Objs[obj.vertices[1]].go.transform.position;
            if (preName != obj.name){
                preName = obj.name;
            }
            if (preStart != start || preEnd != end){
                foreach (string child in obj.children){
                    var pos = Hierarchy.Objs[child].go.transform.position;
                    float dir = Vector3.Dot(Vector3.Normalize(preEnd - preStart), Vector3.Normalize(pos - preStart));
                    Vector3 result = new Vector3();
                    if (0.9f <= dir && dir <= 1.1f){
                        var ratio = Vector3.Distance(preStart, pos) / Vector3.Distance(preStart, preEnd);
                        result = draw.calc.kc_sang_toa_do(start, end, Vector3.Distance(start, end) * ratio);
                    }
                    else if (-1.1f <= dir && dir <= -0.9f){
                        var ratio = Vector3.Distance(preEnd, pos) / Vector3.Distance(preStart, preEnd);
                        result = draw.calc.kc_sang_toa_do(end, start, Vector3.Distance(start, end) * ratio);
                    }
                    Hierarchy.Objs[child].go.transform.position = result;
                }
                preStart = start;
                preEnd = end;
                line.SetPosition(0, start);
                line.SetPosition(1, end);
            }
            linecollider.RebuildCollider(line);
            line.startWidth = Vector3.Distance(Camera.main.transform.position, start)*draw.line.ratio;
            line.endWidth = Vector3.Distance(Camera.main.transform.position, end)*draw.line.ratio;
        }
    }
}
