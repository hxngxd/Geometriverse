using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class drawPolygon : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    LineCollider linecollider;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đường tròn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("center_name", new List<INPUT>(){draw.uiobj.Value("Tên tâm", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("center_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("vertex_name", new List<INPUT>(){draw.uiobj.Value("Tên đỉnh", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("vertex_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "1", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("step", new List<INPUT>(){draw.uiobj.Value("Số đỉnh", "3", "3", INPUT.ContentType.IntegerNumber, content)});
        content.gameObject.SetActive(false);
        
    }
    public IEnumerator Okay(bool type){
        content.gameObject.SetActive(true);
        Inputs["step"][0].transform.parent.parent.gameObject.SetActive(!type);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            int step = draw.inputhandler.toInt(Inputs["step"][0]);
            if (step < 3){
                Inputs["step"][0].text = "3";
                step = 3;
            }
            draw.drawing = true;
            var plane = "";
            StartCoroutine(draw.point.PointInit(()=>{
                draw.point.whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["center_pos"]);
            }, ()=>{
                var hit = draw.raycast.Hit();
                if (Hierarchy.Planes.ContainsKey(hit.ID)){
                    plane = hit.ID;
                    draw.point.doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
                    draw.point.pointing = false;
                }
            }, Cancel));
            yield return new WaitUntil(() => !draw.point.pointing);
            
            var polygon = draw.obj.Polygon(draw.hierContent);
            Hierarchy.currentObjects["polygon"].Add(polygon.gameObject);

            var center = Hierarchy.currentObjects["point"][0].transform;
            var overlapsedCenter = draw.point.overlapsed;
            if (overlapsedCenter) Hierarchy.currentObjects["point"].RemoveAt(0);

            yield return new WaitForSeconds(0.01f);
            polygon.positionCount = (type ? 90 : (draw.inputhandler.toInt(Inputs["step"][0]) < 3 ? 3 : draw.inputhandler.toInt(Inputs["step"][0])));

            draw.calc.PolygonVertex(polygon, draw.calc.swapYZ(center.position), draw.calc.swapYZ(center.position + Vector3.right), polygon.positionCount, Hierarchy.Planes[plane].rotation);

            Vector3 origin = polygon.GetPosition(0);
            StartCoroutine(draw.point.PointInit(()=>{
                var vertex = Hierarchy.currentObjects["point"][overlapsedCenter ? 0 : 1].transform; 

                vertex.position = draw.calc.swapYZ(draw.calc.intersect_line_plane(draw.raycast.MouseToRay(), Hierarchy.Planes[plane].equation).Value);

                draw.calc.PolygonVertex(polygon, draw.calc.swapYZ(center.position), draw.calc.swapYZ(vertex.position), (type ? 90 : (draw.inputhandler.toInt(Inputs["step"][0]) < 3 ? 3 : draw.inputhandler.toInt(Inputs["step"][0]))), draw.calc.RM_Vectors(draw.calc.swapYZ(center.position), draw.calc.swapYZ(origin), draw.calc.swapYZ(vertex.position)).Multiply(Hierarchy.Planes[plane].rotation));

                draw.inputhandler.Vec2Input(Inputs["vertex_pos"], draw.calc.swapYZ(vertex.position));
                draw.inputhandler.Float2Input(Inputs["radius"][0], Vector3.Distance(vertex.position, center.position));
            }, ()=>{
                var p = Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1];
                p.GetComponent<SphereCollider>().enabled = true;
                draw.hier.AddPoint("", plane, p);
                draw.point.pointing = false;
            }, Cancel));
            yield return new WaitUntil(() => !draw.point.pointing);

            var vertex = Hierarchy.currentObjects["point"][overlapsedCenter ? 0 : 1].transform;

            Hierarchy.Planes[plane].children.Add(vertex.name);
            Hierarchy.Planes[plane].children.Add(polygon.name);
            draw.hier.AddPolygon("", center.name, vertex.name, plane, (type ? 90 : draw.inputhandler.toInt(Inputs["step"][0])), polygon, new List<string>(), new Dictionary<string, float>(), type);
            polygon.gameObject.AddComponent<DynamicPolygon>();
            linecollider.AddCollider(polygon);

            draw.hier.ResetCurrentObjects();
            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator OnSelect(LineRenderer polygon){
        draw.mouse.Select(polygon.transform);
        RealtimeInput(polygon.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(polygon.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var polygon = Hierarchy.Polygons[ID];
        var center = Hierarchy.Points[polygon.center];
        var vertex = Hierarchy.Points[polygon.vertex];

        Inputs["name"][0].text = polygon.name;
        Inputs["center_name"][0].text = center.name;
        Inputs["vertex_name"][0].text = vertex.name;
        draw.inputhandler.Int2Input(Inputs["step"][0], polygon.step);
        draw.inputhandler.Float2Input(Inputs["radius"][0], Vector3.Distance(center.go.transform.position, vertex.go.transform.position));
        draw.inputhandler.Vec2Input(Inputs["center_pos"], draw.calc.swapYZ(center.go.transform.position));
        draw.inputhandler.Vec2Input(Inputs["vertex_pos"], draw.calc.swapYZ(vertex.go.transform.position));

        draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Polygon_Name(ID, Inputs["name"][0].text));
        draw.listener.Add_Input(Inputs["center_name"][0], () => draw.inputhandler.Update_Point_Name(polygon.center, Inputs["center_name"][0].text));
        draw.listener.Add_Input(Inputs["vertex_name"][0], () => draw.inputhandler.Update_Point_Name(polygon.vertex, Inputs["vertex_name"][0].text));
        
        if (center.parent == ""){
            draw.listener.Add_Inputs(Inputs["center_pos"], () => draw.inputhandler.Update_Position(center.go, draw.inputhandler.Input2Vec(Inputs["center_pos"])));
        }
        if (vertex.parent == ""){
            draw.listener.Add_Inputs(Inputs["vertex_pos"], () => draw.inputhandler.Update_Position(vertex.go, draw.inputhandler.Input2Vec(Inputs["vertex_pos"])));
        }
        // draw.listener.Add_Input(Inputs["radius"][0], () => {
        //     vertex.go.transform.position = draw.calc.swapYZ(draw.calc.KC_sang_toa_do(center.go.transform.position, vertex.go.transform.position, draw.inputhandler.toFloat(Inputs["radius"][0])));
        // });
        if (!polygon.type){
            draw.listener.Add_Input(Inputs["step"][0], () => {
                int step = draw.inputhandler.toInt(Inputs["step"][0]);
                if (step < 3){
                    Inputs["step"][0].text = "3";
                    step = 3;
                }
                polygon.step = step;
                Hierarchy.Polygons[ID] = polygon;
            });
        }
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.inputhandler.ResetInput(Inputs["name"][0]);
        draw.inputhandler.ResetInput(Inputs["center_name"][0]);
        draw.inputhandler.ResetInput(Inputs["vertex_name"][0]);
        draw.inputhandler.ResetInput(Inputs["radius"][0]);
        draw.inputhandler.ResetInputs(Inputs["center_pos"]);
        draw.inputhandler.ResetInputs(Inputs["vertex_pos"]);
    }
}
