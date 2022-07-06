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
    public IEnumerator PointInit(Action subActionWhileDrawing, Action subActionDoneDrawing, Action onCancel){
        pointing = true;
        var point = draw.obj.Point(Vector3.zero, draw.hierContent);
        Hierarchy.currentObjects["point"].Add(point);
        while (true){
            subActionWhileDrawing();
            if (Input.GetMouseButtonDown(0) && !draw.raycast.isMouseOverUI()){
                subActionDoneDrawing();
                if (pointing == false) break;
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                onCancel();
                break;
            }
            yield return null;
        }
    }
    [PunRPC]
    void SyncPos(string ID, Vector3 pos){
        Hierarchy.Points[ID].go.transform.position = pos;
    }
    public void whileDrawing(GameObject point, List<INPUT> Pos){
        if (point==null) return;
        var hit = draw.raycast.Hit();
        if (Hierarchy.Types.ContainsKey(hit.ID)){
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
                case "plane":
                case "3pointcircle":
                case "polygon":
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
    [PunRPC]
    public void RPC_AddChildren(string parent, string child){
        switch (Hierarchy.Types[parent]){
            case "line":
                Hierarchy.Lines[parent].children.Add(child);
                break;
            case "plane":
                Hierarchy.Planes[parent].children.Add(child);
                break;
            case "3pointcircle":
                Hierarchy.Circles[parent].children.Add(child);
                break;
            case "polygon":
                Hierarchy.Polygons[parent].children.Add(child);
                break;
        }
    }
    public void doneDrawing(GameObject point, string name){
        if (point==null) return;
        var hit = draw.raycast.Hit();
        var parent = "";
        overlapsed = false;
        if (Hierarchy.Types.ContainsKey(hit.ID)){
            if (Hierarchy.Types[hit.ID] == "point"){
                int index = Hierarchy.currentObjects["point"].IndexOf(point);
                Destroy(point);
                Hierarchy.currentObjects["point"][index] = point = Hierarchy.Points[hit.ID].go;
                overlapsed = true;
            }
            else{
                parent = hit.ID;
                RPC_AddChildren(hit.ID, point.name);
                if (RoomManager.inRoom) this.GetComponent<PhotonView>().RPC("RPC_AddChildren", RpcTarget.OthersBuffered, hit.ID, point.name);
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
            StartCoroutine(PointInit(()=>{
                whileDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1].gameObject, Inputs["pos"]);
            }, ()=>{
                doneDrawing(Hierarchy.currentObjects["point"][Hierarchy.currentObjects["point"].Count-1], "");
                draw.point.pointing = false;
            }, Cancel));
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
        var parent = Hierarchy.Points[point.name].parent;
        if (parent == ""){
            draw.mouse.Follow(point);
            SnapOnAxis(point, draw.raycast.Hit());
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
                case "plane":
                    var mouseray = draw.raycast.MouseToRay();
                    var plane = Hierarchy.Planes[parent].equation;
                    point.transform.position = draw.calc.swapYZ(draw.calc.intersect_line_plane(mouseray, plane).Value);
                    break;
                case "3pointcircle":
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
