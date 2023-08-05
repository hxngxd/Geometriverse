using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawLine : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Transform content;
    public Draw draw;
    public float ratio;
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(Inputs, 2, content);
        Inputs.Add("dist", new List<INPUT>(){draw.uiobj.Value("Độ dài", "0", "", INPUT.ContentType.DecimalNumber, content)});
    }
    public IEnumerator Okay(bool[] properties){
        draw.mouse.UnselectAll();
        if (!properties[0]){
            content.gameObject.SetActive(true);
            while (true){
                draw.ResetInputs(Inputs);
                var objs = new List<GameObject>(new GameObject[3]);

                yield return draw.StartC(draw.makingPoints(1, objs, draw.line, Inputs));

                yield return new WaitForSeconds(0.01f);
                objs[2] = draw.obj.Line(draw.hier.current, false);
                Vector3[] v = new Vector3[2]{objs[0].transform.position, Vector3.zero};
                yield return draw.StartC(draw.point.makePoint(()=>{
                    draw.point.onMove(Inputs["pos_1"]);
                    var ln = objs[2].GetComponent<LineRenderer>();
                    v[1] = draw.point.current_point.transform.position;
                    if (properties[1]){
                        SetLinePosition(ln, v[0], v[1]);
                    }
                    else{
                        var center = (v[0] + v[1])/2;
                        SetLinePosition(ln, draw.calc.kc_sang_toa_do(center, v[0], 100000f), draw.calc.kc_sang_toa_do(center, v[1], 100000f));
                    }
                    Update_Properties(new Vector3[]{ln.GetPosition(0), ln.GetPosition(1)});
                }, ()=>{
                    draw.point.onClick(Inputs["name_1"][0]);
                    objs[1] = draw.point.current_point;
                    draw.point.current_point = null;
                }, () => {draw.Cancel(draw.line);}));

                var vertices = new List<string>(){objs[0].name, objs[1].name};
                draw.hier.AddLine(Inputs["name"][0].text, objs[2], vertices, draw.inRoom());
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
                    draw.point.onMove(Inputs[$"pos_{(points.Count==0 ? 0 : 1)}"]);
                    if (points.Count > 0){
                        Vector3[] v = new Vector3[2]{points[points.Count-1].transform.position, draw.point.current_point.transform.position};
                        var ln = lines[lines.Count-1].GetComponent<LineRenderer>();
                        if (properties[1]){
                            SetLinePosition(ln, v[0], v[1]);
                        }
                        else{
                            var center = (v[0] + v[1])/2;
                            SetLinePosition(ln, draw.calc.kc_sang_toa_do(center, v[0], 100000f), draw.calc.kc_sang_toa_do(center, v[1], 100000f));
                        }
                    }
                    if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                        draw.point.onClick(Inputs["name"][0]);
                        points.Add(draw.point.current_point);
                        if (lines.Count > 0){
                            var ln = lines[lines.Count-1];
                            ln.transform.SetParent(draw.hier.created);
                            var vertices = new List<string>(){points[points.Count-1].name, points[points.Count-2].name};
                            draw.hier.AddLine(Inputs["name"][0].text, ln, vertices, draw.inRoom());
                        }
                        break;
                    }
                    if (Input.GetKeyDown(KeyCode.Escape)){
                        if (points.Count > 1){
                            draw.hier.FinishedCurrentObjects();
                            draw.point.current_point.transform.SetParent(draw.hier.current);
                            lines[lines.Count-1].transform.SetParent(draw.hier.current);
                        }
                        draw.Cancel(draw.line);
                        points.Clear();
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
        draw.RealtimeInput(ID, content, 2, Inputs, draw.line);
    }
    public void Update_Properties(Vector3[] vp){
        Inputs["dist"][0].text = Vector3.Distance(vp[0], vp[1]).ToString();
    }
    public void SetLinePosition(LineRenderer line, Vector3 start, Vector3 end){
        if (line.positionCount < 2) line.positionCount = 2;

        line.SetPositions(new Vector3[]{start,end});
        var camPos = Camera.main.transform.position;
        var dist = draw.calc.kc_diem_dt(camPos, new KeyValuePair<Vector3, Vector3>(start, end));
        line.startWidth = line.endWidth = dist * ratio;
    }
}
