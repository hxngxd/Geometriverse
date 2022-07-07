using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawLine : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public float ratio;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("start_name", new List<INPUT>(){draw.uiobj.Value("Tên điểm đầu", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("start_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("end_name", new List<INPUT>(){draw.uiobj.Value("Tên điểm cuối", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("end_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("dist", new List<INPUT>(){draw.uiobj.Value("Độ dài", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var line = new List<GameObject>(new GameObject[3]);

            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["start_pos"]);
            }, ()=>{
                draw.point.onClick(Inputs["start_name"][0]);
                line[0] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            yield return new WaitForSeconds(0.01f);
            line[2] = draw.obj.Line(draw.hier.current, false);
            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["end_pos"]);
                var ln = line[2].GetComponent<LineRenderer>();
                if (ln.positionCount < 2){
                    ln.positionCount = 2;
                    ln.SetPosition(0, line[0].transform.position);
                }
                ln.SetPosition(1, draw.point.current_point.transform.position);
                ln.startWidth = Vector3.Distance(Camera.main.transform.position, ln.GetPosition(0))*ratio;
                ln.endWidth = Vector3.Distance(Camera.main.transform.position, ln.GetPosition(1))*ratio;
                Inputs["dist"][0].text = Vector3.Distance(ln.GetPosition(0), ln.GetPosition(1)).ToString();
            }, ()=>{
                draw.point.onClick(Inputs["end_name"][0]);
                line[1] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            bool overlapseLine = false;
            if (line[0].transform.IsChildOf(draw.hier.created) && line[1].transform.IsChildOf(draw.hier.created)){
                foreach (var s in Hierarchy.Objs[line[0].name].vertexof){
                    if (Hierarchy.Objs[s].type == "line" && Hierarchy.Objs[line[1].name].vertexof.Contains(s)){
                        overlapseLine = true;
                        break;
                    }
                }
            }
            if (overlapseLine){
                Destroy(line[2].gameObject);
            }
            else{
                var plane = "";
                var startp = Hierarchy.Objs[line[0].name].parent;
                var endp = Hierarchy.Objs[line[1].name].parent;
                if (startp == endp && Hierarchy.Objs.ContainsKey(startp) && Hierarchy.Objs[startp].type == "plane"){
                    plane = startp;
                }
                draw.hier.Add(Inputs["name"][0].text, plane, new List<string>(){line[0].name, line[1].name}, line[2]);
                line[2].AddComponent<DynamicLine>();
                draw.hier.FinishedCurrentObjects();
            }

            yield return new WaitForSeconds(0.01f);
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
        var line = Hierarchy.Objs[ID];
        var start = Hierarchy.Objs[line.vertices[0]];
        var end = Hierarchy.Objs[line.vertices[1]];

        Inputs["name"][0].text = line.name;
        Inputs["start_name"][0].text = start.name;
        Inputs["end_name"][0].text = end.name;
        Inputs["dist"][0].text = Vector3.Distance(start.go.transform.position, end.go.transform.position).ToString();
        draw.input.Vec2Input(Inputs["start_pos"], draw.calc.ztoy(start.go.transform.position));
        draw.input.Vec2Input(Inputs["end_pos"], draw.calc.ztoy(end.go.transform.position));

        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        draw.listener.Add(Inputs["start_name"][0], () => draw.input.Update_Name(line.vertices[0], Inputs["start_name"][0].text));
        draw.listener.Add(Inputs["end_name"][0], () => draw.input.Update_Name(line.vertices[1], Inputs["end_name"][0].text));
        
        if (start.parent == ""){
            draw.listener.Add(Inputs["start_pos"], () => {
                draw.input.Update_Position(start.go, draw.input.Input2Vec(Inputs["start_pos"]));
                Inputs["dist"][0].text = Vector3.Distance(start.go.transform.position, end.go.transform.position).ToString();
            });
        }
        if (end.parent == ""){
            draw.listener.Add(Inputs["end_pos"], () => {
                draw.input.Update_Position(end.go, draw.input.Input2Vec(Inputs["end_pos"]));
                Inputs["dist"][0].text = Vector3.Distance(start.go.transform.position, end.go.transform.position).ToString();
            });
        }
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.input.ResetInput(Inputs["name"][0]);
        draw.input.ResetInput(Inputs["start_name"][0]);
        draw.input.ResetInput(Inputs["end_name"][0]);
        draw.input.ResetInputs(Inputs["start_pos"]);
        draw.input.ResetInputs(Inputs["end_pos"]);
    }
}
