using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class drawCircle : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Transform content;
    public Draw draw;
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đường", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(Inputs, 3, content);
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("perimeter", new List<INPUT>(){draw.uiobj.Value("Chu vi", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("area", new List<INPUT>(){draw.uiobj.Value("Diện tích", "0", "", INPUT.ContentType.DecimalNumber, content)});
    }
    public IEnumerator Okay(bool[] properties){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.ResetInputs(Inputs);
            if (properties[0]){
                var objs = new List<GameObject>(new GameObject[3]);

                draw.StartC(draw.makingPointOnPlane(0, objs, draw.circle, Inputs));
                yield return new WaitUntil(() => draw.point.current_point==null);
                
                yield return new WaitForSeconds(0.01f);
                objs[2] = draw.obj.Line(draw.hier.current, true);
                var ln = objs[2].GetComponent<LineRenderer>();
                ln.positionCount = 180;
                draw.StartC(draw.point.makePoint(()=>{
                    var hit = draw.raycast.Hit();
                    if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID)){
                        draw.point.onMove(Inputs["pos_1"]);
                        var vp = new Vector3[]{objs[0].transform.position, draw.point.current_point.transform.position};
                        if (vp[0] != vp[1]){
                            objs[2].SetActive(true);
                            var center = draw.calc.ztoy(vp[0]);
                            var current_pos = draw.calc.ztoy(vp[1]);
                            ln.SetPositions(draw.calc.dinh_da_giac(center, current_pos, 180, Hierarchy.Objs[draw.plane.current_plane].rotation));
                            Update_Properties(vp);
                        }
                        else{
                            Update_Properties(new Vector3[]{Vector3.zero, Vector3.zero});
                            objs[2].SetActive(false);
                        }
                    }
                    else{
                        objs[2].SetActive(false);
                        draw.mouse.Follow(draw.point.current_point);
                        draw.input.Vec2Input(Inputs["pos_1"], draw.calc.ztoy(draw.point.current_point.transform.position));
                    }
                }, ()=>{
                    var hit = draw.raycast.Hit();
                    if (Hierarchy.Objs.ContainsKey(hit.ID) && draw.OnPlane(hit.ID) && objs[0].transform.position != draw.point.current_point.transform.position){
                        draw.point.onClick(Inputs["name_1"][0]);
                        objs[1] = draw.point.current_point;
                        draw.point.current_point = null;
                    }
                }, ()=>{
                    draw.plane.ToggleExpand(draw.plane.current_plane, false);
                    draw.plane.current_plane = "";
                    draw.Cancel(draw.circle);
                }));
                yield return new WaitUntil(() => draw.point.current_point==null);
                
                var vertices_ = new List<string>();
                for (int i=0;i<2;i++) vertices_.Add(objs[i].name);
                draw.hier.AddCircle(Inputs["name"][0].text, objs[2], new List<string>(){draw.plane.current_plane}, vertices_, draw.inRoom());
            }
            else{
                var objs = new List<GameObject>(new GameObject[4]);

                yield return draw.StartC(draw.makingPoints(2, objs, draw.circle, Inputs));

                yield return new WaitForSeconds(0.01f);
                objs[3] = draw.obj.Line(draw.hier.current, true);
                var vp = new Vector3[]{draw.calc.ztoy(objs[0].transform.position), draw.calc.ztoy(objs[1].transform.position), Vector3.zero};
                yield return draw.StartC(draw.point.makePoint(()=>{
                    var ln = objs[3].GetComponent<LineRenderer>();
                    draw.point.onMove(Inputs["pos_2"]);
                    vp[2] = draw.calc.ztoy(draw.point.current_point.transform.position);
                    if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                        var center = draw.calc.tam_dg_tron_ngtiep(vp[0],vp[1],vp[2]);
                        var current_rotation = draw.calc.rm_plane_xy(vp[0],vp[1],vp[2]);
                        ln.positionCount = 180;
                        ln.SetPositions(draw.calc.dinh_da_giac(center, vp[2], 180, current_rotation));
                        Update_Properties(vp);
                    }
                    else{
                        Update_Properties(new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero});
                    }
                }, ()=>{
                    if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                        draw.point.onClick(Inputs["name_2"][0]);
                        objs[2] = draw.point.current_point;
                        draw.point.current_point = null;
                    }
                }, () => {draw.Cancel(draw.circle);}));
                
                var vertices_ = new List<string>();
                for (int i=0;i<3;i++) vertices_.Add(objs[i].name);
                var rotation = draw.calc.rm_plane_xy(vp[0], vp[1], vp[2]);
                draw.hier.AddCircle(Inputs["name"][0].text, objs[3], vertices_, draw.inRoom());

            }
            draw.hier.FinishedCurrentObjects();
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, Hierarchy.Objs[ID].vertices.Count, Inputs, draw.circle);
    }
    public void Update_Properties(Vector3[] vp){
        var r = (vp.Length == 3 ? Vector3.Distance(draw.calc.tam_dg_tron_ngtiep(vp[0], vp[1], vp[2]), vp[0]) : Vector3.Distance(vp[0], vp[1]));
        Inputs["radius"][0].text = r.ToString();
        Inputs["perimeter"][0].text = (2f*Mathf.PI*r).ToString();
        Inputs["area"][0].text = (r*r*Mathf.PI).ToString();
    }
}
