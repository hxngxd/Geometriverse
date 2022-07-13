using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawLine : MonoBehaviour
{
    public Transform content;
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public float ratio;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(ref Inputs, 2, content);
        Inputs.Add("dist", new List<INPUT>(){draw.uiobj.Value("Độ dài", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(bool continuous){
        draw.mouse.UnselectAll();
        if (!continuous){
            content.gameObject.SetActive(true);
            while (true){
                ResetInputsList();
                var objs = new List<GameObject>(new GameObject[3]);

                draw.StartC(draw.makingPoint(1, objs, draw.line, Inputs));
                yield return new WaitUntil(() => draw.point_ing == false);

                yield return new WaitForSeconds(0.01f);
                objs[2] = draw.obj.Line(draw.hier.current, false);
                draw.StartC(draw.point.makePoint(()=>{
                    draw.point.onMove(Inputs["pos_1"]);
                    var ln = objs[2].GetComponent<LineRenderer>();
                    SetLinePosition(ln, objs[0].transform.position, draw.point.current_point.transform.position);
                    Update_Properties(new Vector3[]{ln.GetPosition(0), ln.GetPosition(1)});
                }, ()=>{
                    draw.point.onClick(Inputs["name_1"][0]);
                    objs[1] = draw.point.current_point;
                    draw.point.current_point = null;
                }, () => {draw.Cancel(draw.line);}));
                yield return new WaitUntil(() => draw.point.current_point==null);

                draw.hier.AddLine(Inputs["name"][0].text, "", new List<string>(){objs[0].name, objs[1].name}, objs[2], new Dictionary<string, float>());
                objs[2].AddComponent<DynamicLine>();
                draw.hier.FinishedCurrentObjects();

                yield return new WaitForSeconds(0.01f);
            }
        }
        else{
            content.gameObject.SetActive(true);
            var points = new List<GameObject>();
            var lines = new List<GameObject>();
            while (true){
                draw.point.current_point = draw.obj.Point(Vector3.zero, draw.hier.current);
                while (true){
                    draw.point.onMove(draw.point.Inputs["pos"]);
                    draw.input.Vec2Input(Inputs[$"pos_{(points.Count==0 ? 0 : 1)}"], draw.calc.ztoy(draw.point.current_point.transform.position));
                    if (points.Count > 0){
                        var ln = lines[lines.Count-1].GetComponent<LineRenderer>();
                        SetLinePosition(ln, points[points.Count-1].transform.position, draw.point.current_point.transform.position);
                    }
                    if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                        draw.point.onClick(draw.point.Inputs["name"][0]);
                        points.Add(draw.point.current_point);
                        points[points.Count-1].transform.SetParent(draw.hier.created);
                        if (lines.Count > 0){
                            var ln = lines[lines.Count-1];
                            ln.transform.SetParent(draw.hier.created);
                            draw.hier.AddLine("", "", new List<string>(){points[points.Count-1].name, points[points.Count-2].name}, ln, new Dictionary<string, float>());
                            ln.AddComponent<DynamicLine>();
                        }
                        break;
                    }
                    if (Input.GetKeyDown(KeyCode.Escape)){
                        if (points.Count == 1) points[points.Count-1].transform.SetParent(draw.hier.current);
                        draw.Cancel(draw.point);
                        break;
                    }
                    yield return null;
                }
                if (points.Count > 0){
                    lines.Add(draw.obj.Line(draw.hier.current, false));
                    draw.input.Vec2Input(Inputs["pos_0"], draw.calc.ztoy(points[points.Count-1].transform.position));
                }
                yield return null;
            }
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, Inputs, draw.line);
    }
    public void Update_Properties(Vector3[] vp){
        Inputs["dist"][0].text = Vector3.Distance(vp[0], vp[1]).ToString();
    }
    public void ResetInputsList(){
        draw.input.ResetInputs(new List<INPUT>(){Inputs["name"][0], Inputs["dist"][0]});
        for (int i=0;i<2;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
    public void SetLinePosition(LineRenderer line, Vector3 start, Vector3 end){
        if (line.positionCount < 2) line.positionCount = 2;

        line.SetPositions(new Vector3[]{start,end});
        var camPos = Camera.main.transform.position;
        line.startWidth = Vector3.Distance(camPos, start) * ratio;
        line.endWidth = Vector3.Distance(camPos, end) * ratio;
    }
}
