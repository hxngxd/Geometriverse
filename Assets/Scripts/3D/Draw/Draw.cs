using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class Draw : MonoBehaviour
{
    public Hierarchy hier;
    public Create3DObjects obj;
    public CreateUIObjects uiobj;
    public RaycastHandler raycast;
    public MouseHandler mouse;
    public InputFieldHandler input;
    public MenuManager menu;
    public PanelManager panel;
    public Listener listener;
    public Calculate calc;
    public CameraController cam;


    public drawPoint point;
    public drawLine line;
    public drawPlane plane;
    public drawCircle circle;
    public drawSphere sphere;
    public drawPolygon polygon;
    public Dictionary<string, dynamic> dts = new Dictionary<string, dynamic>();


    public bool isDrawing = false, point_ing = false;
    public ScrollRect scroll;
    public List<Coroutine> Coroutines = new List<Coroutine>();
    public void Awake(){
        foreach (var type in new List<dynamic>(){point, line, plane, circle, sphere, polygon}){
            string name = type.GetType().ToString();
            name = name.Substring(4, name.Length - 4).ToLower();
            dts.Add(name, type);
            mouse.Selected.Add(name, new List<Transform>());
        }
    }
    public void StartC(IEnumerator coroutine){
        var c = StartCoroutine(coroutine);
        Coroutines.Add(c);
    }
    public void StopAllCs(){
        foreach (var c in Coroutines){
            StopCoroutine(c);
        }
        Coroutines.Clear();
    }
    public void letDraw(dynamic type){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartC(type.Okay());
    }
    public void letDraw(dynamic type, bool condition){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartC(type.Okay(condition));
    }
    public void Refresh(){
        Cancel();
        foreach (var type in dts) Cancel(type.Value);

        if (plane.current_plane != ""){
            plane.ToggleExpand(plane.current_plane, false);
            plane.current_plane = "";
        }
    }
    public void Cancel(){
        StopAllCs();
        hier.RemoveCurrentObjects();
        isDrawing = point_ing = false;
    }
    public void Cancel(dynamic type){
        type.content.gameObject.SetActive(false);
        type.ResetInputsList();
        Cancel();
    }
    public IEnumerator OnSelect(GameObject go, dynamic type){
        type.RealtimeInput(go.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        mouse.Unselect(go.transform);
        Cancel();
    }
    public void InspectorVector(ref Dictionary<string, List<INPUT>> Inputs, int t, Transform parent){
        for (int i=0;i<t;i++){
            Inputs.Add(t < 2 ? "name" : $"name_{i}", new List<INPUT>(){uiobj.Value(t < 2 ? "Tên điểm" : $"Tên điểm {i+1}", "Tên...", "", INPUT.ContentType.Alphanumeric, parent)});
            Inputs.Add(t < 2 ? "pos" : $"pos_{i}", uiobj.Vec3("Toạ độ", parent));
        }
    }
    public IEnumerator makingPoint(int t, List<GameObject> objs, dynamic type, Dictionary<string, List<INPUT>> Inputs){
        point_ing = true;
        for (int i=0;i<t;i++){
            yield return new WaitForSeconds(0.01f);
            StartC(point.makePoint(()=>{
                point.onMove(Inputs[$"pos_{i}"]);
            }, ()=>{
                point.onClick(Inputs[$"name_{i}"][0]);
                objs[i] = point.current_point;
                point.current_point = null;
            }, () => {
                Cancel(type);
            }));
            yield return new WaitUntil(() => point.current_point==null);
        }
        point_ing = false;
    }
    public bool OnPlane(string ID){
        var obj = Hierarchy.Objs[ID];
        var isplane = obj.type == "plane";
        var ispoint = obj.type == "point" && (true ||
            (
                obj.parent != "" && Hierarchy.Objs[obj.parent].parent == plane.current_plane
            ) 
        );
        return isplane || ispoint;
    }
    public IEnumerator makingPointOnPlane(int id, List<GameObject> objs, dynamic type, Dictionary<string, List<INPUT>> Inputs){
        point.current_point = obj.Point(Vector3.zero, hier.current);
        while (point.current_point != null){
            point.onMove(Inputs[$"pos_{id}"]);
            if (Input.GetMouseButtonDown(0) && !raycast.isMouseOverUI()){
                var hit = raycast.Hit();
                var isContained = Hierarchy.Objs.ContainsKey(hit.ID);
                if (plane.current_plane == ""){
                    if (isContained && Hierarchy.Objs[hit.ID].type == "plane"){
                        plane.current_plane = hit.ID;
                        plane.ToggleExpand(hit.ID, true);
                    }
                }
                else{
                    if (isContained && OnPlane(hit.ID)){
                        point.onClick(Inputs[$"name_{id}"][0]);
                        objs[id] = point.current_point;
                        point.current_point = null;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                if (plane.current_plane == ""){
                    Cancel(type);
                }
                else{
                    plane.ToggleExpand(plane.current_plane, false);
                    plane.current_plane = "";
                }
            }
            yield return null;
        }
    }
    public void RealtimeInput(string ID, Transform content, Dictionary<string, List<INPUT>> Inputs, dynamic type){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        listener.Add(Inputs["name"][0], () => input.Update_Name(ID, Inputs["name"][0].text));
        int numOfVertices = obj.vertices.Count;
        Vector3[] vpos = new Vector3[numOfVertices];
        for (int i=0;i<numOfVertices;i++){
            int id = i;
            string name = $"name_{id}", pos = $"pos_{id}";
            var v = Hierarchy.Objs[obj.vertices[id]];
            vpos[id] = v.go.transform.position;
            Inputs[name][0].text = v.name;
            listener.Add(Inputs[name][0], () => input.Update_Name(obj.vertices[id], Inputs[name][0].text));
            input.Vec2Input(Inputs[pos], calc.ztoy(vpos[id]));
            if (v.parent == ""){
                listener.Add(Inputs[pos], () => {
                    input.Update_Position(v.go, input.Input2Vec(Inputs[pos]));
                    if (type != null){
                        var vp = new Vector3[numOfVertices];
                        for (int j=0;j<numOfVertices;j++){
                            vp[j] = Hierarchy.Objs[obj.vertices[j]].go.transform.position;
                        }
                        type.Update_Properties(vp);
                    }
                });
            }
        }
        if (type != null) type.Update_Properties(vpos);
    }
}
