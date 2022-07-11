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
    public drawPoint point;
    public drawLine line;
    public drawPlane plane;
    public drawCircle circle;
    public drawSphere sphere;
    public CameraController cam;
    public bool isDrawing = false;
    public bool point_ing = false;
    public ScrollRect scroll;
    public void letDraw(dynamic type){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartCoroutine(type.Okay());
    }
    public void letDraw(dynamic type, bool condition){
        Refresh();
        isDrawing = true;
        scroll.content = type.content.GetComponent<RectTransform>();
        StartCoroutine(type.Okay(condition));
    }
    public void Refresh(){
        Cancel();
        foreach (var type in new List<dynamic>(){point, line, plane, circle, sphere}) Cancel(type);

        if (plane.current_plane != ""){
            plane.ToggleExpand(plane.current_plane, false);
            plane.current_plane = "";
        }
    }
    public void Cancel(){
        StopAllCoroutines();
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
        Cancel();
        mouse.Unselect(go.transform);
    }
    public void InspectorVector(ref Dictionary<string, List<INPUT>> Inputs, int t, Transform parent){
        for (int i=0;i<t;i++){
            Inputs.Add(t < 2 ? "name" : $"name_{i}", new List<INPUT>(){uiobj.Value(t < 2 ? "Tên điểm" : $"Tên điểm {i+1}", "Tên...", "", INPUT.ContentType.Alphanumeric, parent)});
            Inputs.Add(t < 2 ? "pos" : $"pos_{i}", uiobj.Vec3("Toạ độ", parent));
        }
    }
    public IEnumerator makingPoint(int t, List<GameObject> objs, dynamic type, Dictionary<string, List<INPUT>> Inputs){
        point_ing = true;
        for (int i=0;i<t && point_ing;i++){
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(point.makePoint(()=>{
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
