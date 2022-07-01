using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawPlane : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Dictionary<string, Toggle> Toggles = new Dictionary<string, Toggle>();
    Draw draw;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên mặt phẳng", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("name_1", new List<INPUT>(){draw.uiobj.Value("Tên điểm 1", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("pos_1", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("name_2", new List<INPUT>(){draw.uiobj.Value("Tên điểm 2", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("pos_2", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("name_3", new List<INPUT>(){draw.uiobj.Value("Tên điểm 3", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("pos_3", draw.uiobj.Vec3("Toạ độ", content));
        content.gameObject.SetActive(false);
        Toggles.Add("expand", draw.uiobj.Togle("Mở rộng", false, content));
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            draw.drawing = true;
            StartCoroutine(draw.point.PointInit(()=>{}, ()=>{}, Cancel, Inputs["pos_1"]));
            yield return new WaitUntil(() => !draw.point.pointing);
            
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.PointInit(()=>{}, ()=>{}, Cancel, Inputs["pos_2"]));
            yield return new WaitUntil(() => !draw.point.pointing);

            var plane = draw.obj.Plane(draw.hierContent);
            Hierarchy.currentObjects["plane"].Add(plane);
            var vertices = new Vector3[]{Hierarchy.currentObjects["point"][0].transform.position, Hierarchy.currentObjects["point"][1].transform.position, Vector3.zero};
            var triangles = new int[]{0,1,2,2,1,0};
            var filter = plane.GetComponent<MeshFilter>();

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.PointInit(()=>{
                vertices[2] = Hierarchy.currentObjects["point"][2].transform.position;
                filter.mesh.Clear();
                filter.mesh.vertices = vertices;
                filter.mesh.triangles = triangles;
            }, ()=>{}, Cancel, Inputs["pos_3"]));
            yield return new WaitUntil(() => !draw.point.pointing);

            plane.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            var p1 = Hierarchy.currentObjects["point"][0];
            var p2 = Hierarchy.currentObjects["point"][1];
            var p3 = Hierarchy.currentObjects["point"][2];
            var vertices_ = new List<string>(){p1.name, p2.name, p3.name};
            var children = new List<string>(){p1.name, p2.name, p3.name};
            var rotation = draw.calc.RM_Plane_XY(draw.calc.swapYZ(p2.transform.position), draw.calc.swapYZ(p1.transform.position), draw.calc.swapYZ(p3.transform.position));
            var equation = draw.calc.plane_equation(draw.calc.swapYZ(p2.transform.position), draw.calc.swapYZ(p1.transform.position), draw.calc.swapYZ(p3.transform.position));
            draw.hier.AddPlane("", false, vertices_, children, rotation, plane, equation);

            draw.hier.ResetCurrentObjects();
            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator OnSelect(GameObject plane){
        draw.mouse.Select(plane.transform);
        RealtimeInput(plane.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(plane.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var plane = Hierarchy.Planes[ID];
        var v1 = Hierarchy.Points[plane.vertices[0]];
        var v2 = Hierarchy.Points[plane.vertices[1]];
        var v3 = Hierarchy.Points[plane.vertices[2]];
        Inputs["name"][0].text = plane.name;
        Inputs["name_1"][0].text = v1.name;
        Inputs["name_2"][0].text = v2.name;
        Inputs["name_3"][0].text = v3.name;
        draw.inputhandler.Vec2Input(Inputs["pos_1"], draw.calc.swapYZ(v1.go.transform.position));
        draw.inputhandler.Vec2Input(Inputs["pos_2"], draw.calc.swapYZ(v2.go.transform.position));
        draw.inputhandler.Vec2Input(Inputs["pos_3"], draw.calc.swapYZ(v3.go.transform.position));
        draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Plane_Name(ID, Inputs["name"][0].text));
        draw.listener.Add_Input(Inputs["name_1"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[0], Inputs["name_1"][0].text));
        draw.listener.Add_Input(Inputs["name_2"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[1], Inputs["name_2"][0].text));
        draw.listener.Add_Input(Inputs["name_3"][0], () => draw.inputhandler.Update_Point_Name(plane.vertices[2], Inputs["name_3"][0].text));

        if (v1.parent == ""){
            draw.listener.Add_Inputs(Inputs["pos_1"], () => {
                draw.inputhandler.Update_Position(v1.go, draw.inputhandler.Input2Vec(Inputs["pos_1"]));
            });
        }
        if (v2.parent == ""){
            draw.listener.Add_Inputs(Inputs["pos_2"], () => {
                draw.inputhandler.Update_Position(v2.go, draw.inputhandler.Input2Vec(Inputs["pos_2"]));
            });
        }
        if (v3.parent == ""){
            draw.listener.Add_Inputs(Inputs["pos_3"], () => {
                draw.inputhandler.Update_Position(v3.go, draw.inputhandler.Input2Vec(Inputs["pos_3"]));
            });
        }

        draw.listener.Add_Toggle(Toggles["expand"], () => {
            var obj = Hierarchy.Planes[ID];
            obj.expand = Toggles["expand"].isOn;
            Hierarchy.Planes[ID] = obj;
        });
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.inputhandler.ResetInput(Inputs["name"][0]);
        draw.inputhandler.ResetInput(Inputs["name_1"][0]);
        draw.inputhandler.ResetInputs(Inputs["pos_1"]);
        draw.inputhandler.ResetInput(Inputs["name_2"][0]);
        draw.inputhandler.ResetInputs(Inputs["pos_2"]);
        draw.inputhandler.ResetInput(Inputs["name_3"][0]);
        draw.inputhandler.ResetInputs(Inputs["pos_3"]);
    }
}
