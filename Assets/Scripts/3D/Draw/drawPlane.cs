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
        draw.point.InspectorVector(ref Inputs, 3, content);
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

            gameobjs[3] = draw.obj.Plane(draw.hier.current);
            var _vertices = new Vector3[]{gameobjs[0].transform.position, gameobjs[1].transform.transform.position, Vector3.zero};
            var triangles = new int[]{0,1,2,2,1,0};
            var filter = gameobjs[3].GetComponent<MeshFilter>();

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
                    gameobjs[2] = draw.point.current_point;
                    draw.point.current_point = null;
                }
            }, Cancel));
            yield return new WaitUntil(() => draw.point.current_point==null);
            
            gameobjs[3].GetComponent<MeshCollider>().sharedMesh = filter.mesh;
            var vertices = new List<string>();
            for (int i=0;i<3;i++) vertices.Add(gameobjs[i].name);
            var rotation = draw.calc.rm_plane_xy(draw.calc.ztoy(gameobjs[0].transform.position), draw.calc.ztoy(gameobjs[1].transform.position), draw.calc.ztoy(gameobjs[2].transform.position));
            var equation = draw.calc.pt_mp(draw.calc.ztoy(gameobjs[0].transform.position), draw.calc.ztoy(gameobjs[1].transform.position), draw.calc.ztoy(gameobjs[2].transform.position));
            draw.hier.AddPlane(Inputs["name"][0].text, vertices, gameobjs[3], rotation, equation);
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
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        for (int i=0;i<3;i++){
            int current_index = i;
            string name = $"name_{current_index}", pos = $"pos_{current_index}";
            var v = Hierarchy.Objs[obj.vertices[current_index]];
            Inputs[name][0].text = v.name;
            draw.input.Vec2Input(Inputs[pos], draw.calc.ztoy(v.go.transform.position));
            draw.listener.Add(Inputs[name][0], () => {
                draw.input.Update_Name(obj.vertices[current_index], Inputs[name][0].text);
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
