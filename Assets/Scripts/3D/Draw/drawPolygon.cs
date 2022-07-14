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
                var points = new List<GameObject>(new GameObject[1]);
                Matrix<double> rotation = DenseMatrix.OfArray(new double[,]{});

                draw.StartC(draw.makingPointOnPlane(0, points, draw.polygon, Inputs));
                yield return new WaitUntil(() => draw.point.current_point == false);

                yield return new WaitForSeconds(0.01f);

                int step = draw.input.toInt(Inputs["step"][0]);
                points.AddRange(new List<GameObject>(new GameObject[step]));
                for (int i=0;i<step;i++){
                    points[i+1] = draw.obj.Point(Vector3.zero, draw.hier.current);
                    points[i+1].SetActive(false);
                }
                var line = draw.obj.Line(draw.hier.current, true);
                var ln = line.GetComponent<LineRenderer>();
                ln.positionCount = step;
                draw.StartC(draw.point.makePoint(()=>{
                    var hit = draw.raycast.Hit();
                    if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID)){
                        draw.point.onMove(Inputs["pos_1"]);
                        if (points[0].transform.position != draw.point.current_point.transform.position){
                            var center = draw.calc.ztoy(points[0].transform.position);
                            var current_pos = draw.calc.ztoy(draw.point.current_point.transform.position);
                            var current_rot = Hierarchy.Objs[draw.plane.current_plane].rotation;
                            var positions = draw.calc.dinh_da_giac(center, current_pos, 1, current_rot);
                            rotation = draw.calc.rm_vectors(center, draw.calc.ztoy(positions[0]), current_pos);
                            positions = draw.calc.dinh_da_giac(center, current_pos, step, rotation.Multiply(current_rot));

                            for (int i=0;i<step;i++){
                                points[i+1].SetActive(true);
                                points[i+1].transform.position = positions[i];
                            }
                            line.SetActive(true);
                            ln.SetPositions(positions);
                            Inputs["radius"][0].text = Vector3.Distance(center, current_pos).ToString();
                        }
                        else{
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

                yield return new WaitUntil(() => draw.point.current_point==null);

                var vertices = new List<string>();
                for (int i=0;i<=step;i++){
                    if (i > 1){
                        draw.hier.AddPoint("", draw.plane.current_plane, points[i]);
                    }
                    vertices.Add(points[i].name);
                }
                draw.hier.AddPolygon(Inputs["name"][0].text, draw.plane.current_plane, vertices, line, rotation);
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
