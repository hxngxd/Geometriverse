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
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.point.InspectorVector(ref Inputs, 2, content);
        Inputs.Add("dist", new List<INPUT>(){draw.uiobj.Value("Độ dài", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var gameobjs = new List<GameObject>(new GameObject[3]);

            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_0"]);
            }, ()=>{
                draw.point.onClick(Inputs["name_0"][0]);
                gameobjs[0] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            yield return new WaitForSeconds(0.01f);
            gameobjs[2] = draw.obj.Line(draw.hier.current, false);
            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_1"]);
                var ln = gameobjs[2].GetComponent<LineRenderer>();
                if (ln.positionCount < 2){
                    ln.positionCount = 2;
                    ln.SetPosition(0, gameobjs[0].transform.position);
                }
                ln.SetPosition(1, draw.point.current_point.transform.position);
                ln.startWidth = Vector3.Distance(Camera.main.transform.position, ln.GetPosition(0))*ratio;
                ln.endWidth = Vector3.Distance(Camera.main.transform.position, ln.GetPosition(1))*ratio;
                Update_Properties(new Vector3[]{ln.GetPosition(0), ln.GetPosition(1)});
            }, ()=>{
                draw.point.onClick(Inputs["name_1"][0]);
                gameobjs[1] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            bool overlapseLine = false;
            if (gameobjs[0].transform.IsChildOf(draw.hier.created) && gameobjs[1].transform.IsChildOf(draw.hier.created)){
                foreach (var s in Hierarchy.Objs[gameobjs[0].name].vertexof){
                    if (Hierarchy.Objs[s].type == "line" && Hierarchy.Objs[gameobjs[1].name].vertexof.Contains(s)){
                        overlapseLine = true;
                        break;
                    }
                }
            }
            if (overlapseLine){
                Destroy(gameobjs[2].gameObject);
            }
            else{
                draw.hier.AddLine(Inputs["name"][0].text, "", new List<string>(){gameobjs[0].name, gameobjs[1].name}, gameobjs[2], new Dictionary<string, float>());
                gameobjs[2].AddComponent<DynamicLine>();
                draw.hier.FinishedCurrentObjects();
            }

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
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        Vector3[] vp = new Vector3[2];
        for (int i=0;i<2;i++){
            int current_index = i;
            string name = $"name_{current_index}", pos = $"pos_{current_index}";
            var v = Hierarchy.Objs[obj.vertices[current_index]];
            vp[current_index] = v.go.transform.position;
            Inputs[name][0].text = v.name;
            draw.input.Vec2Input(Inputs[pos], draw.calc.ztoy(vp[current_index]));
            draw.listener.Add(Inputs[name][0], () => {
                draw.input.Update_Name(obj.vertices[current_index], Inputs[name][0].text);
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
        Inputs["dist"][0].text = Vector3.Distance(vp[0], vp[1]).ToString();
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        ResetInputsList();
        draw.Cancel();
    }
    public void ResetInputsList(){
        draw.input.ResetInputs(new List<INPUT>(){Inputs["name"][0], Inputs["dist"][0]});
        for (int i=0;i<2;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
}
