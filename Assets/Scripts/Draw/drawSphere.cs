using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawSphere : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Transform content;
    public Draw draw;
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên hình", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(Inputs, 2, content);
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("circumference", new List<INPUT>(){draw.uiobj.Value("Chu vi đường tròn", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("surface_area", new List<INPUT>(){draw.uiobj.Value("Diện tích bề mặt", "0", "", INPUT.ContentType.DecimalNumber, content)});
        Inputs.Add("volume", new List<INPUT>(){draw.uiobj.Value("Thể tích", "0", "", INPUT.ContentType.DecimalNumber, content)});
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.ResetInputs(Inputs);
            var objs = new List<GameObject>(new GameObject[3]);

            yield return draw.StartC(draw.makingPoints(1, objs, draw.sphere, Inputs));

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

            draw.hier.AddSphere(Inputs["name"][0].text, objs[2], new List<string>(){objs[0].name, objs[1].name}, draw.inRoom());
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, 2, Inputs, draw.sphere);
    }
    public void Update_Properties(Vector3[] vp){
        var r = Vector3.Distance(vp[0], vp[1]);
        Inputs["radius"][0].text = r.ToString();
        Inputs["circumference"][0].text = (2f*Mathf.PI*r).ToString();
        Inputs["surface_area"][0].text = (4f*r*r*Mathf.PI).ToString();
        Inputs["volume"][0].text = ((4f/3f)*Mathf.PI*r*r*r).ToString();
    }
}
