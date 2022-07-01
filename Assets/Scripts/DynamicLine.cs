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
    Transform hierItem = null;
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
        linecollider.AddCollider(line);
    }

    void Update()
    {
        if (hierItem == null) hierItem = draw.hier.content.Find(this.name);
        else{
            if (Hierarchy.Lines.ContainsKey(this.name)){
                var start = Hierarchy.Points[Hierarchy.Lines[this.name].start].go.transform.position;
                var end = Hierarchy.Points[Hierarchy.Lines[this.name].end].go.transform.position;
                var L = Hierarchy.Lines[this.name];
                if (preName != L.name){
                    preName = L.name;
                    string s = $"Đoạn thẳng: {L.name}";
                    if (L.plane != ""){
                        s += $"\n(Thuộc mặt phẳng: {Hierarchy.Planes[L.plane].name})";
                    }
                    hierItem.Find("Text").GetComponent<TextMeshProUGUI>().text = s;
                }
                if (preStart != start || preEnd != end){
                    foreach (string child in Hierarchy.Lines[this.name].children){
                        var pos = Hierarchy.Points[child].go.transform.position;
                        float dir = Vector3.Dot(Vector3.Normalize(preEnd - preStart), Vector3.Normalize(pos - preStart));
                        Vector3 result = new Vector3();
                        if (0.9f <= dir && dir <= 1.1f){
                            var ratio = Vector3.Distance(preStart, pos) / Vector3.Distance(preStart, preEnd);
                            result = draw.calc.KC_sang_toa_do(start, end, Vector3.Distance(start, end) * ratio);
                        }
                        else if (-1.1f <= dir && dir <= -0.9f){
                            var ratio = Vector3.Distance(preEnd, pos) / Vector3.Distance(preStart, preEnd);
                            result = draw.calc.KC_sang_toa_do(end, start, Vector3.Distance(start, end) * ratio);
                        }
                        Hierarchy.Points[child].go.transform.position = result;
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
}
