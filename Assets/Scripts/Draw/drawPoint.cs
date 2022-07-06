using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using Photon.Pun;
public class drawPoint : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public GameObject current_point = null;
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
    public IEnumerator PointInit(Action while_drawing, Action done_drawing, Action cancel){
        current_point = draw.obj.Point(Vector3.zero, draw.hier.current);
        while (true){
            while_drawing();
            if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                done_drawing();
                if (current_point == null) break;
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                cancel();
                break;
            }
            yield return null;
        }
    }
    public void whileDrawing(GameObject point, List<INPUT> Pos){
        var hit = draw.raycast.Hit();
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            switch (Hierarchy.Objs[hit.ID].type){
                case "point":
                    if (point.activeSelf) point.SetActive(false);
                    point.transform.position = Hierarchy.Objs[hit.ID].go.transform.position;
                    break;
                case "line":
                    if (!point.activeSelf) point.SetActive(true);
                    var start = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[0]].go.transform.position;
                    var end = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[1]].go.transform.position;
                    point.transform.position = draw.calc.HC_diem_len_duong_thang(hit.point, new KeyValuePair<Vector3, Vector3>(start, end));
                    break;
                case "plane":
                case "circle":
                    if (!point.activeSelf) point.SetActive(true);
                    point.transform.position = hit.point;
                    break;
            }
        }
        else{
            if (!point.activeSelf) point.SetActive(true);
            draw.mouse.Follow(point);
            SnapOnAxis(point, hit);
        }
        draw.inputhandler.Vec2Input(Pos, draw.calc.swapYZ(point.transform.position));
    }
    public void doneDrawing(GameObject point, string name, string vertexof){
        var hit = draw.raycast.Hit();
        var parent = "";
        bool overlapsed = false;
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            if (Hierarchy.Objs[hit.ID].type == "point"){
                Destroy(point);
                point = Hierarchy.Objs[hit.ID].go;
                overlapsed = true;
            }
            else{
                parent = hit.ID;
                Hierarchy.Objs[parent].children.Add(point.name);
            }
        }
        if (!overlapsed){
            point.GetComponent<SphereCollider>().enabled = true;
            draw.hier.Add(name, parent, vertexof, point);
            point.transform.SetParent(draw.hier.created);
        }
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            StartCoroutine(PointInit(()=>{
                whileDrawing(current_point, Inputs["pos"]);
            }, ()=>{
                doneDrawing(current_point, Inputs["name"][0].text, "");
                current_point = null;
            }, Cancel));
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
                if (hit.ID == point.name && holdTime > 0.1f && Vector3.Distance(startPosition, Input.mousePosition) > 11f){
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
        Cancel();
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
                    var line = new KeyValuePair<Vector3, Vector3>(draw.calc.swapYZ(Hierarchy.Objs[start].go.transform.position), draw.calc.swapYZ(Hierarchy.Objs[end].go.transform.position));
                    point.transform.position = draw.calc.swapYZ(draw.calc.Duong_vuong_goc_chung(line, mouseRay).Key);
                    break;
                case "plane":
                    var mouseray = draw.raycast.MouseToRay();
                    var plane = Hierarchy.Objs[parent].equation;
                    point.transform.position = draw.calc.swapYZ(draw.calc.intersect_line_plane(mouseray, plane).Value);
                    break;
                case "circle":
                case "polygon":
                    var hit = draw.raycast.Hit();
                    if (hit.ID == parent) point.transform.position = hit.point;
                    break;
            }
        }
        draw.inputhandler.Vec2Input(Inputs["pos"], draw.calc.swapYZ(point.transform.position));
        if (RoomManager.inRoom){
            this.GetComponent<PhotonView>().RPC("SyncPos", RpcTarget.OthersBuffered, point.name, point.transform.position);
        }
    }
    public void SnapOnAxis(GameObject point, RaycastHandler.MouseHit hit){
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            if (hit.ID == "X"){
                point.transform.position = new Vector3(hit.point.x, 0, 0);
            }
            else if (hit.ID == "Y"){
                point.transform.position = new Vector3(0, 0, hit.point.z);
            }
            else if (hit.ID == "Z"){
                point.transform.position = new Vector3(0, hit.point.y, 0);
            }
            else if (hit.ID == "Centre"){
                point.transform.position = Vector3.zero;
            }
            else if (hit.ID.Contains("point")){
                if (hit.transform.Find("dot").gameObject.activeSelf) point.transform.position = hit.transform.position;
                else{
                    if (hit.ID.Contains("X")){
                        point.transform.position = new Vector3(hit.point.x, 0, 0);
                    }
                    else if (hit.ID.Contains("Y")){
                        point.transform.position = new Vector3(0, 0, hit.point.z);
                    }
                    else if (hit.ID.Contains("Z")){
                        point.transform.position = new Vector3(0, hit.point.y, 0);
                    }
                }
            }
        }
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Objs[ID];
        var point = obj.go;
        Inputs["name"][0].text = obj.name;
        draw.inputhandler.Vec2Input(Inputs["pos"], draw.calc.swapYZ(point.transform.position));
        
        draw.listener.Add_Input(Inputs["name"][0], () => draw.inputhandler.Update_GeoObj_Name(ID, Inputs["name"][0].text));

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
