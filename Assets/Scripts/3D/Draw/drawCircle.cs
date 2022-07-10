using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawCircle : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đường", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(ref Inputs, 3, content);
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("perimeter", new List<INPUT>(){draw.uiobj.Value("Chu vi", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("area", new List<INPUT>(){draw.uiobj.Value("Diện tích", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var objs = new GameObject[4];

            StartCoroutine(draw.makingPoint(2, objs, draw.circle, Inputs));
            yield return new WaitUntil(() => draw.point_ing == false);

            yield return new WaitForSeconds(0.01f);
            objs[3] = draw.obj.Line(draw.hier.current, true);
            var vp = new Vector3[]{draw.calc.ztoy(objs[0].transform.position), draw.calc.ztoy(objs[1].transform.position), Vector3.zero};
            StartCoroutine(draw.point.makePoint(()=>{
                var ln = objs[3].GetComponent<LineRenderer>();
                draw.point.onMove(Inputs["pos_2"]);
                vp[2] = draw.calc.ztoy(draw.point.current_point.transform.position);
                if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                    var center = draw.calc.tam_dg_tron_ngtiep(vp[0],vp[1],vp[2]);
                    var current_rotation = draw.calc.rm_plane_xy(vp[0],vp[1],vp[2]);
                    draw.calc.dinh_da_giac(ln, center, vp[2], 90, current_rotation);
                    Update_Properties(vp);
                }
            }, ()=>{
                if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                    draw.point.onClick(Inputs["name_2"][0]);
                    objs[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, () => {draw.Cancel(draw.circle);}));
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            var vertices_ = new List<string>();
            for (int i=0;i<3;i++) vertices_.Add(objs[i].name);
            var rotation = draw.calc.rm_plane_xy(vp[0], vp[1], vp[2]);
            draw.hier.AddCircle(Inputs["name"][0].text, "", vertices_, objs[3], rotation, new Dictionary<string, float>());
            objs[3].AddComponent<DynamicCircle>();
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, Inputs, draw.circle);
    }
    public void Update_Properties(Vector3[] vp){
        var r = Vector3.Distance(draw.calc.tam_dg_tron_ngtiep(vp[0], vp[1], vp[2]), vp[0]);
        Inputs["radius"][0].text = r.ToString();
        Inputs["perimeter"][0].text = (2f*Mathf.PI*r).ToString();
        Inputs["area"][0].text = (r*r*Mathf.PI).ToString();
    }
    public void ResetInputsList(){
        draw.input.ResetInputs(new List<INPUT>(){Inputs["name"][0], Inputs["radius"][0], Inputs["perimeter"][0], Inputs["area"][0]});
        for (int i=0;i<3;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
}
