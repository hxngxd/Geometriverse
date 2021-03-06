using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawSphere : MonoBehaviour
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
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("circumference", new List<INPUT>(){draw.uiobj.Value("Chu vi đường tròn", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("surface_area", new List<INPUT>(){draw.uiobj.Value("Diện tích bề mặt", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("volume", new List<INPUT>(){draw.uiobj.Value("Thể tích", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var objs = new List<GameObject>(new GameObject[3]);

            draw.StartC(draw.makingPoint(1, objs, draw.sphere, Inputs));
            yield return new WaitUntil(() => draw.point_ing == false);

            yield return new WaitForSeconds(0.01f);
            objs[2] = draw.obj.Sphere(objs[0].transform.position, draw.hier.current);
            draw.StartC(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_1"]);
                var v1 = objs[0].transform.position;
                var v2 = draw.point.current_point.transform.position;
                Update_Properties(new Vector3[]{v1, v2});
                var r = Vector3.Distance(v1, v2)*2;
                objs[2].transform.localScale = new Vector3(r,r,r);
            }, ()=>{
                draw.point.onClick(Inputs["name_1"][0]);
                objs[1] = draw.point.current_point;
                draw.point.current_point = null;
            }, () => {draw.Cancel(draw.sphere);}));
            yield return new WaitUntil(() => draw.point.current_point==null);

            draw.hier.AddSphere(Inputs["name"][0].text, new List<string>(){objs[0].name, objs[1].name}, objs[2], new Dictionary<string, float>());
            objs[2].GetComponent<SphereCollider>().enabled = true;
            objs[2].AddComponent<DynamicSphere>();
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, Inputs, draw.sphere);
    }
    public void Update_Properties(Vector3[] vp){
        var r = Vector3.Distance(vp[0], vp[1]);
        Inputs["radius"][0].text = r.ToString();
        Inputs["circumference"][0].text = (2f*Mathf.PI*r).ToString();
        Inputs["surface_area"][0].text = (4f*r*r*Mathf.PI).ToString();
        Inputs["volume"][0].text = ((4f/3f)*Mathf.PI*r*r*r).ToString();
    }
    public void ResetInputsList(){
        draw.input.ResetInputs(new List<INPUT>(){Inputs["name"][0], Inputs["radius"][0], Inputs["circumference"][0], Inputs["surface_area"][0], Inputs["volume"][0]});
        for (int i=0;i<2;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
}
