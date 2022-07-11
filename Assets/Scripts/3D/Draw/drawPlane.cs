using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawPlane : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public string current_plane = "";
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên mặt phẳng", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        draw.InspectorVector(ref Inputs, 3, content);
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            ResetInputsList();
            var objs = new List<GameObject>(new GameObject[4]);

            StartCoroutine(draw.makingPoint(2, objs, draw.plane, Inputs));
            yield return new WaitUntil(() => draw.point_ing == false);

            objs[3] = draw.obj.Plane(draw.hier.current);
            var vp = new Vector3[]{objs[0].transform.position, objs[1].transform.position, Vector3.zero};
            var triangles = new int[]{0,1,2,2,1,0};
            var filter = objs[3].GetComponent<MeshFilter>();

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.makePoint(()=>{
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
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            objs[3].GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            var vertices = new List<string>();
            for (int i=0;i<3;i++){
                vertices.Add(objs[i].name);
                vp[i] = draw.calc.ztoy(vp[i]);
            }
            var rotation = draw.calc.rm_plane_xy(vp[0], vp[1], vp[2]);
            var equation = draw.calc.pt_mp(vp[0], vp[1], vp[2]);
            draw.hier.AddPlane(Inputs["name"][0].text, vertices, objs[3], rotation, equation, false);
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public void RealtimeInput(string ID){
        draw.RealtimeInput(ID, content, Inputs, null);
    }
    public void ResetInputsList(){
        draw.input.ResetInput(Inputs["name"][0]);
        for (int i=0;i<3;i++){
            draw.input.ResetInput(Inputs[$"name_{i}"][0]);
            draw.input.ResetInputs(Inputs[$"pos_{i}"]);
        }
    }
    public void ToggleExpand(string ID, bool toggle){
        var obj = Hierarchy.Objs[ID];
        obj.expand = toggle;
        if (toggle){
            foreach (var obj_ in Hierarchy.Objs){
                if (!obj.children.Contains(obj_.Key) && !obj.vertices.Contains(obj_.Key) && obj_.Key != ID){
                    obj_.Value.go.SetActive(false);
                }
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
