using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Photon.Pun;
public class drawPolygon : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Transform content;
    public Draw draw;

    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên hình", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(Inputs, 2, content);
        Inputs.Add("step", new List<INPUT>(){draw.uiobj.Value("Số đỉnh", "3", "3", INPUT.ContentType.IntegerNumber, content)});
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("perimeter", new List<INPUT>(){draw.uiobj.Value("Chu vi", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("area", new List<INPUT>(){draw.uiobj.Value("Diện tích", "0", "", INPUT.ContentType.DecimalNumber, content)});
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.ResetInputs(Inputs);
            var points = new List<GameObject>(new GameObject[1]);

            yield return draw.StartC(draw.makingPointOnPlane(0, points, draw.polygon, Inputs));

            yield return new WaitForSeconds(0.01f);

            int step = draw.input.toInt(Inputs["step"][0]);
            if (step < 3){
                step = 3;
                Inputs["step"][0].text = "3";
            }
            points.AddRange(new List<GameObject>(new GameObject[step]));
            for (int i=0;i<step;i++){
                points[i+1] = draw.obj.Point(Vector3.zero, draw.hier.current);
                points[i+1].SetActive(false);
            }
            var line = draw.obj.Line(draw.hier.current, true);
            var ln = line.GetComponent<LineRenderer>();
            ln.positionCount = step;
            yield return draw.StartC(draw.point.makePoint(()=>{
                var hit = draw.raycast.Hit();
                if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID)){
                    draw.point.onMove(Inputs["pos_1"]);
                    var vp = new Vector3[3]{points[0].transform.position, draw.point.current_point.transform.position, Vector3.zero};
                    if (vp[0] != vp[1]){
                        var center = draw.calc.ztoy(vp[0]);
                        var current_pos = draw.calc.ztoy(vp[1]);
                        var current_rot = Hierarchy.Objs[draw.plane.current_plane].rotation;
                        var positions = draw.calc.dinh_da_giac(center, current_pos, 1, current_rot);
                        var rotation = draw.calc.rm_vectors(center, draw.calc.ztoy(positions[0]), current_pos);
                        positions = draw.calc.dinh_da_giac(center, current_pos, step, rotation.Multiply(current_rot));
                        vp[2] = positions[1];
                        for (int i=0;i<step;i++){
                            points[i+1].SetActive(true);
                            points[i+1].transform.position = positions[i];
                        }
                        line.SetActive(true);
                        ln.SetPositions(positions);
                        Update_Properties(vp, step);
                    }
                    else{
                        Update_Properties(new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero}, 0);
                        for (int i=0;i<step;i++){
                            points[i+1].SetActive(false);
                        }
                        line.SetActive(false);
                    }
                }
                else{
                    for (int i=0;i<step;i++){
                        points[i+1].SetActive(false);
                    }
                    line.SetActive(false);
                }
            }, ()=>{
                var hit = draw.raycast.Hit();
                if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID) && points[0].transform.position != draw.point.current_point.transform.position){
                    draw.point.onClick(Inputs["name_1"][0]);
                    Destroy(points[1]);
                    points[1] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, ()=>{
                draw.plane.ToggleExpand(draw.plane.current_plane, false);
                draw.plane.current_plane = "";
                draw.Cancel(draw.polygon);
            }));

            var vertices = new List<string>();
            for (int i=0;i<=step;i++){
                if (i > 1){
                    draw.hier.AddPoint("", points[i], new List<string>(){draw.plane.current_plane}, false, draw.inRoom());
                }
                vertices.Add(points[i].name);
            }
            draw.hier.AddPolygon(Inputs["name"][0].text, line, new List<string>(){draw.plane.current_plane}, vertices, draw.inRoom());

            draw.hier.FinishedCurrentObjects();
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        var obj = Hierarchy.Objs[ID];
        var vp = new Vector3[3];
        for (int i=0;i<3;i++) vp[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
        Update_Properties(vp, obj.vertices.Count-1);
        draw.RealtimeInput(ID, content, 2, Inputs, null);
    }
    public void Update_Properties(Vector3[] vp, int step){
        var r = Vector3.Distance(vp[0], vp[1]);
        var s = Vector3.Distance(vp[1], vp[2]);
        var a = Mathf.Sqrt(r*r - Mathf.Pow(s/2f, 2f));
        Inputs["step"][0].text = step.ToString();
        Inputs["radius"][0].text = r.ToString();
        Inputs["perimeter"][0].text = (s * (float)step).ToString();
        Inputs["area"][0].text = ((a * s * (float)step)/2f).ToString();
    }
}
