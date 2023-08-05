using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawPlane : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    public Transform content;
    public Draw draw;
    public string current_plane = "";
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên mặt phẳng", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(Inputs, 3, content);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.ResetInputs(Inputs);
            var objs = new List<GameObject>(new GameObject[4]);

            yield return draw.StartC(draw.makingPoints(2, objs, draw.plane, Inputs));

            objs[3] = draw.obj.Plane(draw.hier.current);
            var vp = new Vector3[]{objs[0].transform.position, objs[1].transform.position, Vector3.zero};
            var triangles = new int[]{0,1,2,2,1,0};
            var filter = objs[3].GetComponent<MeshFilter>();

            yield return new WaitForSeconds(0.01f);
            yield return draw.StartC(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_2"]);
                vp[2] = draw.point.current_point.transform.position;
                if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                    filter.mesh.Clear();
                    filter.mesh.vertices = vp;
                    filter.mesh.triangles = triangles;
                }
            }, ()=>{
                if (vp[0] != vp[1] && vp[1] != vp[2] && vp[2] != vp[0] && Mathf.Abs(Vector3.Dot((vp[1]-vp[0]).normalized, (vp[2]-vp[0]).normalized)) != 1){
                    draw.point.onClick(Inputs["name_2"][0]);
                    objs[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, () => {draw.Cancel(draw.plane);}));
            
            objs[3].GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            var vertices = new List<string>();
            for (int i=0;i<3;i++){
                vertices.Add(objs[i].name);
                vp[i] = draw.calc.ztoy(vp[i]);
            }
            draw.hier.AddPlane(Inputs["name"][0].text, objs[3], vertices, false, draw.inRoom());
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, 3, Inputs, null);
    }
    public void ToggleExpand(string ID, bool toggle){
        var obj = Hierarchy.Objs[ID];
        obj.expand = toggle;
        if (toggle){
            foreach (var obj_ in Hierarchy.Objs){
                if (!draw.OnPlane(obj_.Key)) obj_.Value.go.SetActive(false);
            }
        }
        else{
            foreach (var obj_ in Hierarchy.Objs){
                obj_.Value.go.SetActive(true);
            }   
        }
        Hierarchy.Objs[ID] = obj;
    }
}
