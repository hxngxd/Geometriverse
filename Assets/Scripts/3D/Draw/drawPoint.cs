using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawPoint : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public GameObject current_point = null;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        draw.InspectorVector(ref Inputs, 1, content);
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.StartC(makePoint(()=>{
                onMove(Inputs["pos"]);
            }, ()=>{
                onClick(Inputs["name"][0]);
                current_point.transform.SetParent(draw.hier.created);
                current_point = null;
            }, () => {draw.Cancel(draw.point);}));
            yield return new WaitUntil(() => current_point==null);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator OnSelect(GameObject point){
        draw.mouse.Select(point.transform);
        RealtimeInput(point.name);
        float startTime = 0f, holdTime = 0f;
        var hit = new RaycastHandler.MouseHit();
        var startPosition = new Vector3();
        while (!Input.GetKeyDown(KeyCode.Escape)){
            if (Input.GetMouseButtonDown(0)){
                startTime = Time.time;
                startPosition = Input.mousePosition;
                hit = draw.raycast.Hit();
            }
            if (Input.GetMouseButton(0)){
                holdTime = Time.time - startTime;
                if (holdTime > 0.11f && Vector3.Distance(startPosition, Input.mousePosition) > 11f){
                    point.GetComponent<SphereCollider>().enabled = false;
                    Drag(point);
                }
            }
            if (Input.GetMouseButtonUp(0)){
                point.GetComponent<SphereCollider>().enabled = true;
            }
            yield return null;
        }
        point.GetComponent<SphereCollider>().enabled = true;
        draw.mouse.Unselect(point.transform);
        draw.Cancel(draw.point);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        draw.listener.Add(Inputs["name"][0], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        draw.input.Vec2Input(Inputs["pos"], draw.calc.ztoy(obj.go.transform.position));
        if (obj.parent == ""){
            draw.listener.Add(Inputs["pos"], () => draw.input.Update_Position(obj.go, draw.input.Input2Vec(Inputs["pos"])));
        }
    }
    public void ResetInputsList(){
        draw.input.ResetInput(Inputs["name"][0]);
        draw.input.ResetInputs(Inputs["pos"]);
    }
    public IEnumerator makePoint(Action move, Action click, Action cancel){
        current_point = draw.obj.Point(Vector3.zero, draw.hier.current);
        while (current_point != null){
            move();
            if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                click();
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                cancel();
            }
            yield return null;
        }
    }
    public void onMove(List<INPUT> PositionInput){
        var hit = draw.raycast.Hit();
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            switch (Hierarchy.Objs[hit.ID].type){
                case "point":
                    if (current_point.activeSelf) current_point.SetActive(false);
                    current_point.transform.position = Hierarchy.Objs[hit.ID].go.transform.position;
                    break;
                case "line":
                    if (!current_point.activeSelf) current_point.SetActive(true);
                    var start = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[0]].go.transform.position;
                    var end = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[1]].go.transform.position;
                    current_point.transform.position = draw.calc.hc_diem_dt(hit.point, new KeyValuePair<Vector3, Vector3>(start, end));
                    break;
                case "plane":
                case "sphere":
                    if (!current_point.activeSelf) current_point.SetActive(true);
                    current_point.transform.position = hit.point;
                    break;
                case "circle":
                    break;
            }
        }
        else{
            if (!current_point.activeSelf) current_point.SetActive(true);
            draw.mouse.Follow(current_point);
            SnapOnAxis(current_point, hit);
        }
        draw.input.Vec2Input(PositionInput, draw.calc.ztoy(current_point.transform.position));
    }
    public void onClick(INPUT Name){
        var hit = draw.raycast.Hit();
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            if (Hierarchy.Objs[hit.ID].type == "point"){
                Destroy(current_point);
                current_point = Hierarchy.Objs[hit.ID].go;
            }
            else{
                draw.hier.AddPoint(Name.text, hit.ID, current_point);
            }
        }
        else{
            draw.hier.AddPoint(Name.text, "", current_point);
        }
    }
    public void Drag(GameObject point){
        var parent = Hierarchy.Objs[point.name].parent;
        if (parent == ""){
            draw.mouse.Follow(point);
            SnapOnAxis(point, draw.raycast.Hit());
        }
        else{
            switch (Hierarchy.Objs[parent].type){
                case "line":
                    var mouseRay = draw.raycast.MouseToRay();
                    var start = Hierarchy.Objs[parent].vertices[0];
                    var end = Hierarchy.Objs[parent].vertices[1];
                    var line = new KeyValuePair<Vector3, Vector3>(draw.calc.ztoy(Hierarchy.Objs[start].go.transform.position), draw.calc.ztoy(Hierarchy.Objs[end].go.transform.position));
                    point.transform.position = draw.calc.ztoy(draw.calc.duong_vgc(line, mouseRay).Key);
                    break;
                case "plane":
                    var mouseray = draw.raycast.MouseToRay();
                    var plane = Hierarchy.Objs[parent].equation;
                    point.transform.position = draw.calc.ztoy(draw.calc.gd_dt_mp(mouseray, plane).Value);
                    break;
                case "circle":
                    break;
                case "sphere":
                    break;
            }
        }
        draw.input.Vec2Input(Inputs["pos"], draw.calc.ztoy(point.transform.position));
    }
    public void SnapOnAxis(GameObject point, RaycastHandler.MouseHit hit){
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            if (hit.ID.Contains("line")){
                switch (hit.ID[0]){
                    case 'X':
                        point.transform.position = new Vector3(hit.point.x, 0, 0);
                        break;
                    case 'Y':
                        point.transform.position = new Vector3(0, 0, hit.point.z);
                        break;
                    case 'Z':
                        point.transform.position = new Vector3(0, hit.point.y, 0);
                        break;
                }
            }
            else if (hit.ID.Contains("point")){
                switch (hit.ID[0]){
                    case 'X':
                        point.transform.position = new Vector3(hit.transform.position.x, 0, 0);
                        break;
                    case 'Y':
                        point.transform.position = new Vector3(0, 0, hit.transform.position.z);
                        break;
                    case 'Z':
                        point.transform.position = new Vector3(0, hit.transform.position.y, 0);
                        break;
                }
            }
            else if (hit.ID == "Centre"){
                point.transform.position = Vector3.zero;
            }
        }
    }
}
