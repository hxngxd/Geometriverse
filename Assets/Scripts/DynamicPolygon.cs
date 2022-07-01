using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicPolygon : MonoBehaviour
{
    LineRenderer polygon;
    LineCollider linecollider;
    Draw draw;
    Vector3 preCenter, preVertex;
    string preName;
    float preStep;
    Transform hierItem = null;
    void Start()
    {
        polygon = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
        linecollider.AddCollider(polygon);
    }

    // Update is called once per frame
    void Update()
    {
        if (hierItem == null) hierItem = draw.hier.content.Find(this.name);
        else{
            if (Hierarchy.Polygons.ContainsKey(this.name)){
                var center = Hierarchy.Points[Hierarchy.Polygons[this.name].center].go.transform.position;
                var vertex = Hierarchy.Points[Hierarchy.Polygons[this.name].vertex].go.transform.position;
                var P = Hierarchy.Polygons[this.name];
                if (preName != P.name){
                    preName = P.name;
                    string s = (P.type ? $"Đường tròn: {P.name}" : $"Đoạn thẳng: {P.name}");
                    if (P.plane != ""){
                        s += $"\n(Thuộc mặt phẳng: {Hierarchy.Planes[P.plane].name})";
                    }
                    hierItem.Find("Text").GetComponent<TextMeshProUGUI>().text = s;
                }
                if (preCenter != center || preVertex != vertex || preStep != P.step){
                    preCenter = center;
                    preVertex = vertex;
                    preStep = P.step;

                    draw.calc.PolygonVertex(polygon, draw.calc.swapYZ(center), draw.calc.swapYZ(center + Vector3.right), P.step, Hierarchy.Planes[P.plane].rotation);
                    Vector3 origin = polygon.GetPosition(0);
                    // draw.calc.PolygonVertex(polygon, draw.calc.swapYZ(center), draw.calc.swapYZ(vertex), P.step, draw.calc.RM_Vectors(draw.calc.swapYZ(center), draw.calc.swapYZ(origin), draw.calc.swapYZ(vertex)).Multiply(Hierarchy.Planes[P.plane].rotation));
                    draw.calc.PolygonVertex(polygon, draw.calc.swapYZ(center), draw.calc.swapYZ(vertex), P.step, Hierarchy.Planes[P.plane].rotation);
                }
                linecollider.RebuildCollider(polygon);
            }
        }
    }
}
