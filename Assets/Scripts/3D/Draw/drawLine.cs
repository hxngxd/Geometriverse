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
        draw.InspectorVector(ref Inputs, 2, content);
        Inputs.Add("dist", new List<INPUT>(){draw.uiobj.Value("Độ dài", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
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
                if (ln.positionCount < 2){
                    ln.positionCount = 2;
                    ln.SetPosition(0, objs[0].transform.position);
                }
                ln.SetPosition(1, draw.point.current_point.transform.position);
                var camPos = Camera.main.transform.position;
                ln.startWidth = Vector3.Distance(camPos, ln.GetPosition(0)) * ratio;
                ln.endWidth = Vector3.Distance(camPos, ln.GetPosition(1)) * ratio;
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
}
