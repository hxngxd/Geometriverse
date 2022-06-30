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
    public bool pointing = false, overlapsed = false;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên điểm", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("pos", draw.uiobj.Vec3("Toạ độ", content));
        content.gameObject.SetActive(false);
    }
    public IEnumerator PointInit(Action subActionWhileDrawing, Action subActionDoneDrawing, Action onCancel, List<INPUT> Pos){
        pointing = true;
        var point = draw.obj.Point(Vector3.zero, draw.hierContent);
        Hierarchy.currentObjects["point"].Add(point);
        while (true){
            subActionWhileDrawing();
            whileDrawing(point, Pos);
            if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                subActionWhileDrawing();
                doneDrawing(point, "");
                pointing = false;
                break;
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                onCancel();
                break;
            }
            yield return null;
        }
    }
    public void whileDrawing(GameObject point, List<INPUT> Pos){
        if (point==null) return;
        var hit = draw.raycast.Hit();
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){

        }
        else{
            if (hit.ID == "Background"){
                if (!point.activeSelf) point.SetActive(true);
                draw.mouse.Follow(point);
            }
            else{
                switch (Hierarchy.Types[hit.ID]){
                    case "point":
                        if (point.activeSelf) point.SetActive(false);
                        point.transform.position = Hierarchy.Points[hit.ID].go.transform.position;
                        break;
                    case "line":
                        if (!point.activeSelf) point.SetActive(true);
                        var start = Hierarchy.Points[Hierarchy.Lines[hit.ID].start].go.transform.position;
                        var end = Hierarchy.Points[Hierarchy.Lines[hit.ID].end].go.transform.position;
                        point.transform.position = draw.calc.HC_diem_len_duong_thang(hit.point, new KeyValuePair<Vector3, Vector3>(start, end));
                        break;
                }
            }
        }
        draw.inputhandler.Vec2Input(Pos, draw.calc.swapYZ(point.transform.position));
    }
    public void doneDrawing(GameObject point, string name){
        if (point==null) return;
        var hit = draw.raycast.Hit();
        var parent = "";
        overlapsed = false;
        if (hit.ID != "Background"){
            switch (Hierarchy.Types[hit.ID]){
                case "point":
                    int index = Hierarchy.currentObjects["point"].IndexOf(point);
                    Destroy(point);
                    Hierarchy.currentObjects["point"][index] = point = Hierarchy.Points[hit.ID].go;
                    overlapsed = true;
                    break;
                case "line":
                    parent = hit.ID;
                    Hierarchy.Lines[hit.ID].children.Add(point.name);
                    break;
            }
        }
        point.GetComponent<SphereCollider>().enabled = true;
        if (overlapsed) return;
        draw.hier.AddPoint(name, parent, point);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.drawing = true;
            StartCoroutine(PointInit(()=>{}, ()=>{}, Cancel, Inputs["pos"]));
            yield return new WaitUntil(() => !pointing);
            draw.hier.ResetCurrentObjects();
            yield return new WaitForSeconds(0.015f);
        }
    }
    public IEnumerator OnSelect(GameObject point){
        draw.mouse.Select(point.transform);
        RealtimeInput(point.name);
        var hit = new RaycastHandler.MouseHit();
        float startTime = 0f, holdTime = 0f;
        var startPosition = new Vector3();
        while (!Input.GetKeyDown(KeyCode.Escape)){
            if (Input.GetMouseButtonDown(0)){
                startTime = Time.time;
                startPosition = Input.mousePosition;
                hit = draw.raycast.Hit();
            }
            if (Input.GetMouseButton(0)){
                holdTime = Time.time - startTime;
                if (hit.ID == point.name && holdTime > 0.1f && Vector3.Distance(startPosition, Input.mousePosition) > 11f){
                    Drag(point);
                }
            }
            yield return null;
        }

        Cancel();
        draw.mouse.Unselect(point.transform);
    }
    public void Drag(GameObject point){
        var parent = Hierarchy.Points[point.name].parent;
        if (parent == ""){
            draw.mouse.Follow(point);
        }
        else{
            switch (Hierarchy.Types[parent]){
                case "line":
                    var mouseRay = draw.raycast.MouseToRay();
                    var start = Hierarchy.Lines[parent].start;
                    var end = Hierarchy.Lines[parent].end;
                    var line = new KeyValuePair<Vector3, Vector3>(draw.calc.swapYZ(Hierarchy.Points[start].go.transform.position), draw.calc.swapYZ(Hierarchy.Points[end].go.transform.position));
                    point.transform.position = draw.calc.swapYZ(draw.calc.Duong_vuong_goc_chung(line, mouseRay).Key);
                    break;
            }
        }
        draw.inputhandler.Vec2Input(Inputs["pos"], draw.calc.swapYZ(point.transform.position));
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Points[ID];
        var point = obj.go;
        Inputs["name"][0].text = obj.name;
        draw.inputhandler.Vec2Input(Inputs["pos"], draw.calc.swapYZ(point.transform.position));
        
        draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_Point_Name(ID, Inputs["name"][0].text));

        if (obj.parent == ""){
            draw.listener.Add_Inputs(Inputs["pos"], () => draw.inputhandler.Update_Position(point, draw.inputhandler.Input2Vec(Inputs["pos"])));
        }
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        draw.inputhandler.ResetInput(Inputs["name"][0]);
        draw.inputhandler.ResetInputs(Inputs["pos"]);
        draw.Cancel();
    }
}
