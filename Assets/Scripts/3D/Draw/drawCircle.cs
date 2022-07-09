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
        draw.point.InspectorVector(ref Inputs, 3, content);
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
            var gameobjs = new List<GameObject>(new GameObject[4]);

            for (int i=0;i<2;i++){
                yield return new WaitForSeconds(0.01f);
                StartCoroutine(draw.point.makePoint(()=>{
                    draw.point.onMove(Inputs[$"pos_{i}"]);
                }, ()=>{
                    draw.point.onClick(Inputs[$"name_{i}"][0]);
                    gameobjs[i] = draw.point.current_point;
                    draw.point.current_point = null;
                }, Cancel));
                yield return new WaitUntil(() => draw.point.current_point==null);
            }

            yield return new WaitForSeconds(0.01f);
            gameobjs[3] = draw.obj.Line(draw.hier.current, true);
            StartCoroutine(draw.point.makePoint(()=>{
                var ln = gameobjs[3].GetComponent<LineRenderer>();
                draw.point.onMove(Inputs["pos_2"]);
                var v1 = draw.calc.ztoy(gameobjs[0].transform.position);
                var v2 = draw.calc.ztoy(gameobjs[1].transform.position);
                var v3 = draw.calc.ztoy(draw.point.current_point.transform.position);
                if (v1 != v2 && v2 != v3 && v3 != v1 && Mathf.Abs(Vector3.Dot((v2-v1).normalized, (v3-v1).normalized)) != 1){
                    var center = draw.calc.tam_dg_tron_ngtiep(v1,v2,v3);
                    var current_rotation = draw.calc.rm_plane_xy(v1,v2,v3);
                    draw.calc.dinh_da_giac(ln, center, v3, 90, current_rotation);
                    Update_Properties(new Vector3[]{v1,v2,v3});
                }
            }, ()=>{
                if (Mathf.Abs(Vector3.Dot((gameobjs[1].transform.position-gameobjs[0].transform.position).normalized, (draw.point.current_point.transform.position-gameobjs[0].transform.position).normalized)) != 1){
                    draw.point.onClick(Inputs["name_2"][0]);
                    gameobjs[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            var vertices_ = new List<string>();
            for (int i=0;i<3;i++) vertices_.Add(gameobjs[i].name);
            var rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(gameobjs[0].transform.position), draw.calc.ztoy(gameobjs[1].transform.position), draw.calc.ztoy(gameobjs[2].transform.position));
            draw.hier.AddCircle(Inputs["name"][0].text, "", vertices_, gameobjs[3], rotation, new Dictionary<string, float>());
            gameobjs[3].AddComponent<DynamicCircle>();
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator OnSelect(GameObject go){
        draw.mouse.Select(go.transform);
        RealtimeInput(go.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        Cancel();
        draw.mouse.Unselect(go.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var circle = Hierarchy.Objs[ID];
        Inputs["name"][0].text = circle.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        Vector3[] vp = new Vector3[3];
        for (int i=0;i<3;i++){
            vp[i] = Hierarchy.Objs[circle.vertices[i]].go.transform.position;
            int current_index = i;
            string name = $"name_{current_index}", pos = $"pos_{current_index}";
            var v = Hierarchy.Objs[circle.vertices[current_index]];
            vp[current_index] = v.go.transform.position;
            Inputs[name][0].text = v.name;
            draw.input.Vec2Input(Inputs[pos], draw.calc.ztoy(vp[current_index]));
            draw.listener.Add(Inputs[name][0], () => {
                draw.input.Update_Name(circle.vertices[current_index], Inputs[name][0].text);
            });
            if (v.parent == ""){
                draw.listener.Add(Inputs[pos], () => {
                    draw.input.Update_Position(v.go, draw.input.Input2Vec(Inputs[pos]));
                    Update_Properties(vp);
                });
            }
        }
        Update_Properties(vp);
    }
    void Update_Properties(Vector3[] vp){
        var r = Vector3.Distance(draw.calc.tam_dg_tron_ngtiep(vp[0], vp[1], vp[2]), vp[0]);
        Inputs["radius"][0].text = r.ToString();
        Inputs["perimeter"][0].text = (2f*Mathf.PI*r).ToString();
        Inputs["area"][0].text = (r*r*Mathf.PI).ToString();
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.input.ResetInputs(new List<INPUT>(){Inputs["name"][0], Inputs["radius"][0], Inputs["perimeter"][0], Inputs["area"][0]});
        for (int i=0;i<3;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
}
