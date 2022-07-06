using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class draw3PointCircle : MonoBehaviour
{
//     public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
//     Draw draw;
//     LineCollider linecollider;
//     public Transform content;
//     void Start()
//     {
//         linecollider = FindObjectOfType<LineCollider>();
//         draw = FindObjectOfType<Draw>();
//         Inspector();
//     }
//     public void Inspector(){
//         Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đường tròn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("name_1", new List<INPUT>(){draw.uiobj.Value("Tên điểm 1", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_1", draw.uiobj.Vec3("Toạ độ", content));
//         Inputs.Add("name_2", new List<INPUT>(){draw.uiobj.Value("Tên điểm 2", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_2", draw.uiobj.Vec3("Toạ độ", content));
//         Inputs.Add("name_3", new List<INPUT>(){draw.uiobj.Value("Tên điểm 3", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_3", draw.uiobj.Vec3("Toạ độ", content));
//         content.gameObject.SetActive(false);
//     }
//     public IEnumerator Okay(){
//         content.gameObject.SetActive(true);
//         draw.mouse.UnselectAll();
//         while (true){
//             ResetInputsList();
//             bool[] overlapsed = new bool[3]{false,false,false};
//             Matrix<double> rotation = DenseMatrix.OfArray(new double[,]{});
//             draw.drawing = true;
//             StartCoroutine(draw.point.PointInit(()=>{
//                 draw.point.whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["pos_1"]);
//             }, ()=>{
//                 draw.point.doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
//                 draw.point.pointing = false;
//             }, Cancel));
//             yield return new WaitUntil(() => !draw.point.pointing);
            
//             var v1 = Hierarchy.currentObjects["point"][0].transform;
//             overlapsed[0] = draw.point.overlapsed;
//             if (overlapsed[0]) Hierarchy.currentObjects["point"].RemoveAt(0);

//             yield return new WaitForSeconds(0.01f);
//             StartCoroutine(draw.point.PointInit(()=>{
//                 draw.point.whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["pos_2"]);
//             }, ()=>{
//                 draw.point.doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
//                 draw.point.pointing = false;
//             }, Cancel));
//             yield return new WaitUntil(() => !draw.point.pointing);

//             var v2 = Hierarchy.currentObjects["point"][overlapsed[0] ? 0 : 1].transform;
//             overlapsed[1] = draw.point.overlapsed;
//             if (overlapsed[1]) Hierarchy.currentObjects["point"].RemoveAt(0);

//             var circle = draw.obj.Polygon(draw.hierContent);
//             Hierarchy.currentObjects["3pointcircle"].Add(circle.gameObject);

//             yield return new WaitForSeconds(0.01f);
//             StartCoroutine(draw.point.PointInit(()=>{
//                 draw.point.whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["pos_3"]);
//                 var p1 = draw.calc.swapYZ(v1.position);
//                 var p2 = draw.calc.swapYZ(v2.position);
//                 var p3 = draw.calc.swapYZ(Hierarchy.currentObjects["point"][(overlapsed[0] && overlapsed[1]) ? 0 : ((!overlapsed[0] && !overlapsed[1]) ? 2 : 1)].transform.position);
//                 if (p1 != p2 && p1 != p3 && p2 != p3){
//                     var center = draw.calc.tam_duong_tron_ngoai_tiep(p1, p2, p3);
//                     rotation = draw.calc.RM_Plane_XY(p1, p2, p3);
//                     draw.calc.PolygonVertex(circle, center, p3, 90, rotation);
//                 }
//             }, ()=>{
//                 draw.point.doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
//                 draw.point.pointing = false;
//             }, Cancel));
//             yield return new WaitUntil(() => !draw.point.pointing);
            
//             var v3 = Hierarchy.currentObjects["point"][(overlapsed[0] && overlapsed[1]) ? 0 : ((!overlapsed[0] && !overlapsed[1]) ? 2 : 1)].transform;

//             var plane = "";
//             var parent1 = Hierarchy.Points[v1.name].parent;
//             var parent2 = Hierarchy.Points[v2.name].parent;
//             var parent3 = Hierarchy.Points[v3.name].parent;
//             if (parent1 == parent2 && parent2 == parent3 && parent3 == parent1 && parent1 != "" && Hierarchy.Planes.ContainsKey(parent1)){
//                 plane = parent1;
//             }
//             var vertices_ = new List<string>(){v1.name, v2.name, v3.name};
//             var children = new List<string>(){v1.name, v2.name, v3.name};
//             draw.hier.AddCircle("", "", "", plane, circle, vertices_, children, rotation, new Dictionary<string, float>());
//             linecollider.AddCollider(circle);
//             circle.gameObject.AddComponent<DynamicCircle>();
//             draw.hier.ResetCurrentObjects();
//             yield return new WaitForSeconds(0.015f);
//         }
//     }
//     public IEnumerator OnSelect(LineRenderer circle){
//         draw.mouse.Select(circle.transform);
//         RealtimeInput(circle.name);
//         yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

//         Cancel();
//         draw.mouse.Unselect(circle.transform);
//     }
//     public void RealtimeInput(string ID){
//         content.gameObject.SetActive(true);
//         var circle = Hierarchy.Circles[ID];
//         var v1 = Hierarchy.Points[circle.vertices[0]];
//         var v2 = Hierarchy.Points[circle.vertices[1]];
//         var v3 = Hierarchy.Points[circle.vertices[2]];
//         Inputs["name"][0].text = circle.name;
//         Inputs["name_1"][0].text = v1.name;
//         Inputs["name_2"][0].text = v2.name;
//         Inputs["name_3"][0].text = v3.name;
//         draw.inputhandler.Vec2Input(Inputs["pos_1"], draw.calc.swapYZ(v1.go.transform.position));
//         draw.inputhandler.Vec2Input(Inputs["pos_2"], draw.calc.swapYZ(v2.go.transform.position));
//         draw.inputhandler.Vec2Input(Inputs["pos_3"], draw.calc.swapYZ(v3.go.transform.position));
//         draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Circle_Name(ID, Inputs["name"][0].text));
//         draw.listener.Add_Input(Inputs["name_1"][0], () => draw.inputhandler.Update_Point_Name(circle.vertices[0], Inputs["name_1"][0].text));
//         draw.listener.Add_Input(Inputs["name_2"][0], () => draw.inputhandler.Update_Point_Name(circle.vertices[1], Inputs["name_2"][0].text));
//         draw.listener.Add_Input(Inputs["name_3"][0], () => draw.inputhandler.Update_Point_Name(circle.vertices[2], Inputs["name_3"][0].text));

//         if (v1.parent == ""){
//             draw.listener.Add_Inputs(Inputs["pos_1"], () => {
//                 draw.inputhandler.Update_Position(v1.go, draw.inputhandler.Input2Vec(Inputs["pos_1"]));
//             });
//         }
//         if (v2.parent == ""){
//             draw.listener.Add_Inputs(Inputs["pos_2"], () => {
//                 draw.inputhandler.Update_Position(v2.go, draw.inputhandler.Input2Vec(Inputs["pos_2"]));
//             });
//         }
//         if (v3.parent == ""){
//             draw.listener.Add_Inputs(Inputs["pos_3"], () => {
//                 draw.inputhandler.Update_Position(v3.go, draw.inputhandler.Input2Vec(Inputs["pos_3"]));
//             });
//         }
//     }
//     public void Cancel(){
//         content.gameObject.SetActive(false);
//         ResetInputsList();
//         draw.Cancel();
//     }
//     public void ResetInputsList(){
//         draw.inputhandler.ResetInput(Inputs["name"][0]);
//         draw.inputhandler.ResetInput(Inputs["name_1"][0]);
//         draw.inputhandler.ResetInputs(Inputs["pos_1"]);
//         draw.inputhandler.ResetInput(Inputs["name_2"][0]);
//         draw.inputhandler.ResetInputs(Inputs["pos_2"]);
//         draw.inputhandler.ResetInput(Inputs["name_3"][0]);
//         draw.inputhandler.ResetInputs(Inputs["pos_3"]);
//     }
}
