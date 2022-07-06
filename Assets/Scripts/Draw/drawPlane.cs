using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawPlane : MonoBehaviour
{
//     public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
//     public Dictionary<string, Toggle> Toggles = new Dictionary<string, Toggle>();
//     Draw draw;
//     public Transform content;
//     void Start()
//     {
//         draw = FindObjectOfType<Draw>();
//         Inspector();
//     }
//     public void Inspector(){
//         Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên mặt phẳng", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("name_1", new List<INPUT>(){draw.uiobj.Value("Tên điểm 1", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_1", draw.uiobj.Vec3("Toạ độ", content));
//         Inputs.Add("name_2", new List<INPUT>(){draw.uiobj.Value("Tên điểm 2", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_2", draw.uiobj.Vec3("Toạ độ", content));
//         Inputs.Add("name_3", new List<INPUT>(){draw.uiobj.Value("Tên điểm 3", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
//         Inputs.Add("pos_3", draw.uiobj.Vec3("Toạ độ", content));
//         content.gameObject.SetActive(false);
//         Toggles.Add("expand", draw.uiobj.Togle("Mở rộng", false, content));
//     }
//     public IEnumerator Okay(){
//         content.gameObject.SetActive(true);
//         draw.mouse.UnselectAll();
//         while (true){
//             ResetInputsList();
//             bool[] overlapsed = new bool[3]{false,false,false};
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

//             var plane = draw.obj.Plane(draw.hierContent);
//             Hierarchy.currentObjects["plane"].Add(plane);
//             var vertices = new Vector3[]{v1.position, v2.transform.position, Vector3.zero};
//             var triangles = new int[]{0,1,2,2,1,0};
//             var filter = plane.GetComponent<MeshFilter>();

//             yield return new WaitForSeconds(0.01f);
//             StartCoroutine(draw.point.PointInit(()=>{
//                 draw.point.whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["pos_3"]);
//                 vertices[2] = Hierarchy.currentObjects["point"][(overlapsed[0] && overlapsed[1]) ? 0 : ((!overlapsed[0] && !overlapsed[1]) ? 2 : 1)].transform.position;
//                 filter.mesh.Clear();
//                 filter.mesh.vertices = vertices;
//                 filter.mesh.triangles = triangles;
//             }, ()=>{
//                 draw.point.doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
//                 draw.point.pointing = false;
//             }, Cancel));
//             yield return new WaitUntil(() => !draw.point.pointing);
            
//             var v3 = Hierarchy.currentObjects["point"][(overlapsed[0] && overlapsed[1]) ? 0 : ((!overlapsed[0] && !overlapsed[1]) ? 2 : 1)].transform;

//             plane.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
//             var vertices_ = new List<string>(){v1.name, v2.name, v3.name};
//             var children = new List<string>(){v1.name, v2.name, v3.name};
//             var rotation = draw.calc.RM_Plane_XY(draw.calc.swapYZ(v2.position), draw.calc.swapYZ(v1.position), draw.calc.swapYZ(v3.position));
//             var equation = draw.calc.plane_equation(draw.calc.swapYZ(v2.position), draw.calc.swapYZ(v1.position), draw.calc.swapYZ(v3.position));
//             draw.hier.AddPlane("", false, vertices_, children, rotation, plane, equation);

//             draw.hier.ResetCurrentObjects();
//             yield return new WaitForSeconds(0.015f);
//         }
//     }
//     public IEnumerator OnSelect(GameObject plane){
//         draw.mouse.Select(plane.transform);
//         RealtimeInput(plane.name);
//         yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

//         Cancel();
//         draw.mouse.Unselect(plane.transform);
//     }
//     public void RealtimeInput(string ID){
//         content.gameObject.SetActive(true);
//         var plane = Hierarchy.Planes[ID];
//         var v1 = Hierarchy.Points[plane.vertices[0]];
//         var v2 = Hierarchy.Points[plane.vertices[1]];
//         var v3 = Hierarchy.Points[plane.vertices[2]];
//         Inputs["name"][0].text = plane.name;
//         Inputs["name_1"][0].text = v1.name;
//         Inputs["name_2"][0].text = v2.name;
//         Inputs["name_3"][0].text = v3.name;
//         draw.inputhandler.Vec2Input(Inputs["pos_1"], draw.calc.swapYZ(v1.go.transform.position));
//         draw.inputhandler.Vec2Input(Inputs["pos_2"], draw.calc.swapYZ(v2.go.transform.position));
//         draw.inputhandler.Vec2Input(Inputs["pos_3"], draw.calc.swapYZ(v3.go.transform.position));
//         draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Plane_Name(ID, Inputs["name"][0].text));
//         draw.listener.Add_Input(Inputs["name_1"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[0], Inputs["name_1"][0].text));
//         draw.listener.Add_Input(Inputs["name_2"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[1], Inputs["name_2"][0].text));
//         draw.listener.Add_Input(Inputs["name_3"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[2], Inputs["name_3"][0].text));

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

//         draw.listener.Add_Toggle(Toggles["expand"], () => {
//             var obj = Hierarchy.Planes[ID];
//             obj.expand = Toggles["expand"].isOn;
//             Hierarchy.Planes[ID] = obj;
//         });
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
