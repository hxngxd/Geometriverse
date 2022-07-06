// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// public class DynamicCircle : MonoBehaviour
// {
//     LineRenderer circle; LineCollider linecollider;
//     Draw draw;
//     Vector3 preCenter, preVertex;
//     Vector3 prev1, prev2, prev3;
//     string preName;
//     Transform hierItem = null;
//     void Start()
//     {
//         circle = this.GetComponent<LineRenderer>();
//         draw = FindObjectOfType<Draw>();
//         linecollider = FindObjectOfType<LineCollider>();
//         linecollider.AddCollider(circle);
//     }

//     void Update()
//     {
//         if (hierItem == null) hierItem = draw.hier.content.Find(this.name);
//         else{
//             if (Hierarchy.Circles.ContainsKey(this.name)){
//                 var C = Hierarchy.Circles[this.name];
//                 if (preName != C.name){
//                     preName = C.name;
//                     string s = $"Đường tròn: {C.name}";
//                     if (C.plane != ""){
//                         s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[C.plane].name})";
//                     }
//                     hierItem.Find("Text").GetComponent<TextMeshProUGUI>().text = s;
//                 }
//                 if (C.center == ""){
//                     var v1 = Hierarchy.Points[Hierarchy.Circles[this.name].vertices[0]].go.transform.position;
//                     var v2 = Hierarchy.Points[Hierarchy.Circles[this.name].vertices[1]].go.transform.position;
//                     var v3 = Hierarchy.Points[Hierarchy.Circles[this.name].vertices[2]].go.transform.position;
//                     if (prev1 != v1 || prev2 != v2 || prev3 != v3){
//                         prev1 = v1;
//                         prev2 = v2;
//                         prev3 = v3;
//                         C.rotation = draw.calc.RM_Plane_XY(draw.calc.swapYZ(v1), draw.calc.swapYZ(v2), draw.calc.swapYZ(v3));
//                         var center = draw.calc.tam_duong_tron_ngoai_tiep(draw.calc.swapYZ(v1), draw.calc.swapYZ(v2), draw.calc.swapYZ(v3));
//                         draw.calc.PolygonVertex(circle, center, draw.calc.swapYZ(v3), 90, C.rotation);
//                         Hierarchy.Circles[this.name] = C;
//                     }
//                 }
//                 linecollider.RebuildCollider(circle);
//             }
//         }
//     }
// }
