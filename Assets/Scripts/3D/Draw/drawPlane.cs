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
            var plane = new List<GameObject>(new GameObject[4]);

            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_0"]);
            }, ()=>{
                draw.point.onClick(Inputs["name_0"][0]);
                plane[0] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_1"]);
            }, ()=>{
                draw.point.onClick(Inputs["name_1"][0]);
                plane[1] = draw.point.current_point;
                draw.point.current_point = null;
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);

            plane[3] = draw.obj.Plane(draw.hier.current);
            var _vertices = new Vector3[]{plane[0].transform.position, plane[1].transform.transform.position, Vector3.zero};
            var triangles = new int[]{0,1,2,2,1,0};
            var filter = plane[3].GetComponent<MeshFilter>();

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(draw.point.makePoint(()=>{
                draw.point.onMove(Inputs["pos_2"]);
                _vertices[2] = draw.point.current_point.transform.position;
                if (_vertices[0] != _vertices[1] && _vertices[1] != _vertices[2] && _vertices[2] != _vertices[0] && Mathf.Abs(Vector3.Dot((_vertices[1]-_vertices[0]).normalized, (_vertices[2]-_vertices[0]).normalized)) != 1){
                    filter.mesh.Clear();
                    filter.mesh.vertices = _vertices;
                    filter.mesh.triangles = triangles;
                }
            }, ()=>{
                if (Mathf.Abs(Vector3.Dot((_vertices[1]-_vertices[0]).normalized, (_vertices[2]-_vertices[0]).normalized)) != 1){
                    draw.point.onClick(Inputs["name_2"][0]);
                    plane[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            plane[3].GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            var vertices = new List<string>();
            for (int i=0;i<3;i++) vertices.Add(plane[i].name);
            var rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(plane[0].transform.position), draw.calc.ztoy(plane[1].transform.position), draw.calc.ztoy(plane[2].transform.position));
            var equation = draw.calc.pt_mp(draw.calc.ztoy(plane[0].transform.position), draw.calc.ztoy(plane[1].transform.position), draw.calc.ztoy(plane[2].transform.position));
            draw.hier.Add(Inputs["name"][0].text, vertices, plane[3], rotation, equation);
            draw.hier.FinishedCurrentObjects();

            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator OnSelect(GameObject plane){
        draw.mouse.Select(plane.transform);
        RealtimeInput(plane.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        Cancel();
        draw.mouse.Unselect(plane.transform);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var plane = Hierarchy.Objs[ID];
        Inputs["name"][0].text = plane.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        for (int i=0;i<3;i++){
            int current_index = i;
            string name = $"name_{current_index}", pos = $"pos_{current_index}";
            var v = Hierarchy.Objs[plane.vertices[current_index]];
            Inputs[name][0].text = v.name;
            draw.input.Vec2Input(Inputs[pos], draw.calc.ztoy(v.go.transform.position));
            draw.listener.Add(Inputs[name][0], () => {
                draw.input.Update_Name(plane.vertices[current_index], Inputs[name][0].text);
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
