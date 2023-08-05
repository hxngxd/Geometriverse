using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using Photon.Pun;
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
    public Notification noti;
    public IDHandler id;
    public DockAutoHide dockah;
    public DockManager dockmanager;
    public ActionManager action;

    public drawPoint point;
    public drawLine line;
    public drawPlane plane;
    public drawCircle circle;
    public drawSphere sphere;
    public drawPolygon polygon;
    public Dictionary<string, dynamic> Types = new Dictionary<string, dynamic>();

    public bool isDrawing = false, allowDrawing = true;
    public ScrollRect scroll;
    public void Start(){
        foreach (var type in new List<dynamic>(){point, line, plane, circle, sphere, polygon}){
            string name = type.GetType().ToString();
            name = name.Substring(4, name.Length - 4).ToLower();
            Types.Add(name, type);
            mouse.Selected.Add(name, new List<Transform>());
            type.content = uiobj.InspectorContent(name);
            type.draw = FindObjectOfType<Draw>();
            type.Inspector();
            type.content.gameObject.SetActive(false);
        }
    }
    public Coroutine StartC(IEnumerator coroutine){
        return StartCoroutine(coroutine);
    }
    public void StopAllCs(){
        StopAllCoroutines();
    }
    public void letDraw(dynamic type){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartC(type.Okay());
    }
    public void letDraw(dynamic type, bool[] properties){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartC(type.Okay(properties));
    }
    public void Refresh(){
        Cancel();
        foreach (var type in Types) Cancel(type.Value);

        if (plane.current_plane != ""){
            plane.ToggleExpand(plane.current_plane, false);
            plane.current_plane = "";
        }
    }
    public void Cancel(){
        StopAllCs();
        hier.RemoveCurrentObjects();
        isDrawing = false;
    }
    public void Cancel(dynamic type){
        Cancel();
        type.content.gameObject.SetActive(false);
        ResetInputs(type.Inputs);
    }
    public IEnumerator OnSelect(GameObject go, dynamic type){
        type.RealtimeInput(go.name);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        mouse.Unselect(go.transform);
        Cancel();
    }
    public void InspectorVector(Dictionary<string, List<INPUT>> Inputs, int t, Transform parent){
        for (int i=0;i<t;i++){
            Inputs.Add(t < 2 ? "name" : $"name_{i}", new List<INPUT>(){uiobj.Value(t < 2 ? "Tên điểm" : $"Tên điểm {i+1}", "Tên...", "", INPUT.ContentType.Alphanumeric, parent)});
            Inputs.Add(t < 2 ? "pos" : $"pos_{i}", uiobj.Vec3("Toạ độ", parent));
        }
    }
    public IEnumerator makingPoints(int t, List<GameObject> objs, dynamic type, Dictionary<string, List<INPUT>> Inputs){
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
    }
    public bool OnPlane(string ID){
        var obj = Hierarchy.Objs[ID];
        var isplane = obj.type == "plane" && ID == plane.current_plane;
        var ispoint = obj.type == "point" && (obj.parents.Contains(plane.current_plane) || obj.vertexof.Contains(plane.current_plane));
        var isothers = obj.parents.Contains(plane.current_plane);
        return isplane || ispoint || isothers;
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
                    else{
                        noti.Show("Hãy chọn một mặt phẳng để vẽ!");
                    }
                }
                else{
                    if (isContained && OnPlane(hit.ID)){
                        point.onClick(Inputs[$"name_{id}"][0]);
                        objs[id] = point.current_point;
                        point.current_point = null;
                    }
                    else{
                        noti.Show("Vui lòng vẽ trên mặt phẳng đã chọn!");
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
    public void RealtimeInput(string ID, Transform content, int numOfVec3, Dictionary<string, List<INPUT>> Inputs, dynamic type){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        if (allowDrawing) listener.Add(Inputs["name"][0], () => input.Update_Name(ID, Inputs["name"][0].text));
        Vector3[] vpos = new Vector3[numOfVec3];
        for (int i=0;i<numOfVec3;i++){
            int id = i;
            string name = $"name_{id}", pos = $"pos_{id}";
            var v = Hierarchy.Objs[obj.vertices[id]];
            vpos[id] = v.go.transform.position;
            Inputs[name][0].text = v.name;
            if (allowDrawing) listener.Add(Inputs[name][0], () => input.Update_Name(obj.vertices[id], Inputs[name][0].text));
            input.Vec2Input(Inputs[pos], calc.ztoy(vpos[id]));
            if (v.parents.Count == 0 && v.movable && allowDrawing){
                listener.Add(Inputs[pos], () => {
                    input.Update_Position(v.go, input.Input2Vec(Inputs[pos]));
                    if (type != null){
                        var vp = new Vector3[numOfVec3];
                        for (int j=0;j<numOfVec3;j++){
                            vp[j] = Hierarchy.Objs[obj.vertices[j]].go.transform.position;
                        }
                        type.Update_Properties(vp);
                    }
                });
            }
        }
        if (type != null) type.Update_Properties(vpos);
    }
    public void ResetInputs(Dictionary<string, List<INPUT>> Inputs){
        foreach (var inputs in Inputs){
            if (inputs.Value.Count == 1) input.ResetInput(inputs.Value[0]);
            else input.ResetInputs(inputs.Value);
        }
    }
    public List<string> getPlaneFromVertices(List<string> vertices){
        var parents = new List<string>();
        var planes = new Dictionary<string, int>();
        foreach (var vertex in vertices){
            var list = new List<string>(Hierarchy.Objs[vertex].parents);
            list.AddRange(Hierarchy.Objs[vertex].vertexof);
            foreach (var parent in list){
                if (Hierarchy.Objs[parent].type == "plane"){
                    if (!planes.ContainsKey(parent)) planes.Add(parent, 1);
                    else{
                        planes[parent]++;
                        if (planes[parent]==vertices.Count){
                            parents.Add(parent);
                        }
                    }
                }
            }
        }
        return parents;
    }
    public bool inRoom(){
        return PhotonNetwork.InRoom;
    }
    public void AllowDraw(bool allow){
        allowDrawing = allow;
        dockah.dockCanvas.gameObject.SetActive(allow);
        var wdcv = panel.canvas.GetComponent<RectTransform>();
        wdcv.offsetMin = new Vector2(wdcv.offsetMin.x, !allow ? 0 : 90);
        foreach (var cmd in menu.firstCmds){
            Destroy(menu.itemsContainer.Find(cmd).gameObject);
        }
        menu.AddFirstCmds();
    }
}
