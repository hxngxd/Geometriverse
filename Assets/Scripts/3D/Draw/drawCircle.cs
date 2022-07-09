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
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên mặt phẳng", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        for (int i=0;i<3;i++){
            Inputs.Add($"name_{i}", new List<INPUT>(){draw.uiobj.Value($"Tên điểm {i+1}", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
            Inputs.Add($"pos_{i}", draw.uiobj.Vec3("Toạ độ", content));
        }
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var circle = new List<GameObject>(new GameObject[4]);

            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_0"]);
            }, ()=>{
                draw.point.onClick(Inputs["name_0"][0]);
                circle[0] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_1"]);
            }, ()=>{
                draw.point.onClick(Inputs["name_1"][0]);
                circle[1] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            yield return new WaitForSeconds(0.01f);
            circle[3] = draw.obj.Line(draw.hier.current, true);
            StartCoroutine(draw.point.makePoint(()=>{
                var ln = circle[3].GetComponent<LineRenderer>();
                draw.point.onMove(Inputs["pos_2"]);
                var v1 = draw.calc.ztoy(circle[0].transform.position);
                var v2 = draw.calc.ztoy(circle[1].transform.position);
                var v3 = draw.calc.ztoy(draw.point.current_point.transform.position);
                if (v1 != v2 && v2 != v3 && v3 != v1 && Mathf.Abs(Vector3.Dot((v2-v1).normalized, (v3-v1).normalized)) != 1){
                    var center = draw.calc.tam_dg_tron_ngtiep(v1,v2,v3);
                    var current_rotation = draw.calc.rm_plane_xy(v1,v2,v3);
                    draw.calc.dinh_da_giac(ln, center, v3, 90, current_rotation);
                }
            }, ()=>{
                if (Mathf.Abs(Vector3.Dot((circle[1].transform.position-circle[0].transform.position).normalized, (draw.point.current_point.transform.position-circle[0].transform.position).normalized)) != 1){
                    draw.point.onClick(Inputs["name_2"][0]);
                    circle[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            var vertices_ = new List<string>();
            for (int i=0;i<3;i++) vertices_.Add(circle[i].name);
            var rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(circle[0].transform.position), draw.calc.ztoy(circle[1].transform.position), draw.calc.ztoy(circle[2].transform.position));
            draw.hier.Add(Inputs["name"][0].text, "", vertices_, circle[3], rotation);
            circle[3].AddComponent<DynamicCircle>();
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator OnSelect(GameObject circle){
        draw.mouse.Select(circle.transform);
        RealtimeInput(circle.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(circle.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var circle = Hierarchy.Objs[ID];
        Inputs["name"][0].text = circle.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        for (int i=0;i<=2;i++){
            int current_index = i;
            string name = $"name_{current_index}", pos = $"pos_{current_index}";
            var v = Hierarchy.Objs[circle.vertices[current_index]];
            Inputs[name][0].text = v.name;
            draw.input.Vec2Input(Inputs[pos], draw.calc.ztoy(v.go.transform.position));
            draw.listener.Add(Inputs[name][0], () => {
                draw.input.Update_Name(circle.vertices[current_index], Inputs[name][0].text);
            });
            if (v.parent == ""){
                draw.listener.Add(Inputs[pos], () => {
                    draw.input.Update_Position(v.go, draw.input.Input2Vec(Inputs[pos]));
                });
            }
        }
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.input.ResetInput(Inputs["name"][0]);
        for (int i=0;i<3;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
}
