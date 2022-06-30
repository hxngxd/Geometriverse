using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawLine : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    LineCollider linecollder;
    public float ratio;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        linecollder = FindObjectOfType<LineCollider>();
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("start_name", new List<INPUT>(){draw.uiobj.Value("Tên điểm đầu", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("start_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("end_name", new List<INPUT>(){draw.uiobj.Value("Tên điểm cuối", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("end_pos", draw.uiobj.Vec3("Toạ độ", content));
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            draw.drawing = true;
            StartCoroutine(draw.point.PointInit(()=>{}, ()=>{}, Cancel, Inputs["start_pos"]));
            yield return new WaitUntil(() => !draw.point.pointing);
            
            var line = draw.obj.Line(draw.hierContent);
            Hierarchy.currentObjects["line"].Add(line.gameObject);
            line.positionCount++;
            line.SetPosition(0, Hierarchy.currentObjects["point"][0].transform.position);

            var start = Hierarchy.currentObjects["point"][0].name;
            var overlapsedStart = draw.point.overlapsed;
            if (overlapsedStart) Hierarchy.currentObjects["point"].RemoveAt(0);

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.PointInit(()=>{
                if (line.positionCount < 2) line.positionCount++;
                line.SetPosition(1, Hierarchy.currentObjects["point"][overlapsedStart ? 0 : 1].transform.position);
                line.startWidth = Vector3.Distance(Camera.main.transform.position, line.GetPosition(0))*ratio;
                line.endWidth = Vector3.Distance(Camera.main.transform.position, line.GetPosition(1))*ratio;
            }, ()=>{}, Cancel, Inputs["end_pos"]));
            yield return new WaitUntil(() => !draw.point.pointing);

            var end = Hierarchy.currentObjects["point"][overlapsedStart ? 0 : 1].name;

            bool overlapseLine = false;
            foreach (var l in Hierarchy.Lines){
                if ((l.Value.start == start && l.Value.end == end) || (l.Value.start == end && l.Value.end == start)){
                    overlapseLine = true;
                    Destroy(line.gameObject);
                    line = l.Value.go;
                    break;
                }
            }

            if (!overlapseLine){
                draw.hier.AddLine("", start, end, "", line, new List<string>());
                linecollder.AddCollider(line);
            }

            draw.hier.ResetCurrentObjects();
            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator OnSelect(LineRenderer line){
        draw.mouse.Select(line.transform);
        RealtimeInput(line.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(line.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var line = Hierarchy.Lines[ID];
        var start = Hierarchy.Points[line.start];
        var end = Hierarchy.Points[line.end];

        Inputs["name"][0].text = line.name;
        Inputs["start_name"][0].text = start.name;
        Inputs["end_name"][0].text = end.name;
        draw.inputhandler.Vec2Input(Inputs["start_pos"], draw.calc.swapYZ(start.go.transform.position));
        draw.inputhandler.Vec2Input(Inputs["end_pos"], draw.calc.swapYZ(end.go.transform.position));

        draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Line_Name(ID, Inputs["name"][0].text));
        draw.listener.Add_Input(Inputs["start_name"][0], () => draw.inputhandler.Update_Point_Name(line.start, Inputs["start_name"][0].text));
        draw.listener.Add_Input(Inputs["end_name"][0], () => draw.inputhandler.Update_Point_Name(line.end, Inputs["end_name"][0].text));
        
        if (start.parent == ""){
            draw.listener.Add_Inputs(Inputs["start_pos"], () => draw.inputhandler.Update_Position(start.go, draw.inputhandler.Input2Vec(Inputs["start_pos"])));
        }
        if (end.parent == ""){
            draw.listener.Add_Inputs(Inputs["end_pos"], () => draw.inputhandler.Update_Position(end.go, draw.inputhandler.Input2Vec(Inputs["end_pos"])));
        }
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.inputhandler.ResetInput(Inputs["name"][0]);
        draw.inputhandler.ResetInput(Inputs["start_name"][0]);
        draw.inputhandler.ResetInput(Inputs["end_name"][0]);
        draw.inputhandler.ResetInputs(Inputs["start_pos"]);
        draw.inputhandler.ResetInputs(Inputs["end_pos"]);
    }
    
}
