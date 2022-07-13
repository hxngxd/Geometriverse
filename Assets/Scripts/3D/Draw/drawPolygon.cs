using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class drawPolygon : MonoBehaviour
{
    public Transform content;
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }

    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên hình", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(ref Inputs, 2, content);
        Inputs.Add("step", new List<INPUT>(){draw.uiobj.Value("Số đỉnh", "3", "3", INPUT.ContentType.IntegerNumber, content)});
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
    }
    public IEnumerator Okay(bool isRegular){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            if (isRegular){
                int step = draw.input.toInt(Inputs["step"][0]);
                var points = new List<GameObject>(new GameObject[step+1]);
                var lines = new List<GameObject>(new GameObject[step]);

                draw.StartC(draw.makingPointOnPlane(0, points, draw.polygon, Inputs));
                yield return new WaitUntil(() => draw.point.current_point == false);

                yield return new WaitForSeconds(0.01f);

                for (int i=0;i<step;i++){
                    points[i+1] = draw.obj.Point(Vector3.zero, draw.hier.current);
                    points[i+1].SetActive(false);
                    lines[i] = draw.obj.Line(draw.hier.current, false);
                    lines[i].SetActive(false);
                }

                draw.StartC(draw.point.makePoint(()=>{
                    var hit = draw.raycast.Hit();
                    if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID)){
                        draw.point.onMove(Inputs["pos_1"]);
                        if (points[0].transform.position != draw.point.current_point.transform.position){
                            var rotation = Hierarchy.Objs[draw.plane.current_plane].rotation;
                            var center = draw.calc.ztoy(points[0].transform.position);
                            var current_pos = draw.calc.ztoy(draw.point.current_point.transform.position);
                            var positions = draw.calc.dinh_da_giac(center, current_pos, step, rotation);
                            positions = draw.calc.dinh_da_giac(center, current_pos, step, draw.calc.rm_vectors(center, draw.calc.ztoy(positions[0]), current_pos).Multiply(rotation));
                            for (int i=0;i<step;i++){
                                points[i+1].SetActive(true);
                                points[i+1].transform.position = positions[i];
                                lines[i].SetActive(true);
                                draw.line.SetLinePosition(lines[i].GetComponent<LineRenderer>(), positions[i], (i==0 ? positions[step-1] : positions[i-1]));
                            }
                        }
                        else{
                            for (int i=0;i<step;i++){
                                points[i+1].SetActive(false);
                                lines[i].SetActive(false);
                            }
                        }
                    }
                    else{
                        for (int i=0;i<step;i++){
                            points[i+1].SetActive(false);
                            lines[i].SetActive(false);
                        }
                    }
                }, ()=>{
                    var hit = draw.raycast.Hit();
                    if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID)){
                        draw.point.onClick(Inputs["name_1"][0]);
                        points[1] = draw.point.current_point;
                        draw.point.current_point = null;
                    }
                }, ()=>{
                    draw.plane.ToggleExpand(draw.plane.current_plane, false);
                    draw.plane.current_plane = "";
                    draw.Cancel(draw.polygon);
                }));
                yield return new WaitUntil(() => draw.point.current_point==null);
            }
            else{

            }
            draw.hier.FinishedCurrentObjects();
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){

    }
    public void Update_Properties(){

    }
    public void ResetInputsList(){

    }
}
