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
    public Transform content;
    public Draw draw;
    public GameObject current_point = null;
    public PhotonView photon;
    public void Inspector(){
        draw.InspectorVector(Inputs, 1, content);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        while (true){
            draw.ResetInputs(Inputs);
            yield return draw.StartC(makePoint(()=>{
                onMove(Inputs["pos"]);
            }, ()=>{
                onClick(Inputs["name"][0]);
                current_point = null;
            }, () => {draw.Cancel(draw.point);}));
            draw.hier.FinishedCurrentObjects();
            yield return new WaitForSeconds(0.01f);
        }
    }
    [PunRPC]
    void SyncPosition(string ID, Vector3 position){
        LeanTween.cancel(Hierarchy.Objs[ID].go);
        LeanTween.move(Hierarchy.Objs[ID].go, position, 0.075f).setEase(LeanTweenType.linear);
    }
    public IEnumerator OnSelect(GameObject point){
        draw.mouse.Select(point.transform);
        RealtimeInput(point.name);
        
        if (Hierarchy.Objs[point.name].movable && draw.allowDrawing){
            var startPosition = new Vector3();
            var hit = draw.raycast.Hit();
            var current_pos = new Vector3();
            while (!Input.GetKeyDown(KeyCode.Escape) && draw.allowDrawing){
                if (Input.GetMouseButtonDown(0)){
                    startPosition = Input.mousePosition;
                    hit = draw.raycast.Hit();
                    current_pos = point.transform.position;
                }
                if (Input.GetMouseButton(0) && !draw.raycast.isMouseOverUI()){
                    if (Vector3.Distance(startPosition, Input.mousePosition) > 11f && hit.transform == point.transform){
                        point.GetComponent<SphereCollider>().enabled = false;
                        Drag(point);
                        if (draw.inRoom()) photon.RPC("SyncPosition", RpcTarget.OthersBuffered, point.name, point.transform.position);
                    }
                }
                if (Input.GetMouseButtonUp(0)){
                    point.GetComponent<SphereCollider>().enabled = true;
                    if (point.transform.position != current_pos){
                        ActionManager.UndoActions.Add(new KeyValuePair<string, object[]>("setpos", new object[]{point.name, draw.calc.ztoy(current_pos)}));
                        ActionManager.RedoActions.Clear();
                    }
                    if (draw.inRoom()) photon.RPC("SyncPosition", RpcTarget.OthersBuffered, point.name, point.transform.position);
                }
                yield return null;
            }
            point.GetComponent<SphereCollider>().enabled = true;
        }
        else{
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        }

        draw.mouse.Unselect(point.transform);
        draw.Cancel(draw.point);
    }
    public void RealtimeInput(string ID){
        content.gameObject.SetActive(true);
        var obj = Hierarchy.Objs[ID];
        Inputs["name"][0].text = obj.name;
        if (draw.allowDrawing) draw.listener.Add(Inputs["name"], () => draw.input.Update_Name(ID, Inputs["name"][0].text));
        draw.input.Vec2Input(Inputs["pos"], draw.calc.ztoy(obj.go.transform.position));
        if (obj.parents.Count == 0 && obj.movable && draw.allowDrawing){
            draw.listener.Add(Inputs["pos"], () => draw.input.Update_Position(obj.go, draw.input.Input2Vec(Inputs["pos"])));
        }
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
    public void onMove(List<INPUT> PosInputs){
        var hit = draw.raycast.Hit();
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            var type = Hierarchy.Objs[hit.ID].type;

            if (type == "point"){
                if (current_point.activeSelf) current_point.SetActive(false);
            } 
            else{
                if (!current_point.activeSelf) current_point.SetActive(true);
            }
            var obj = Hierarchy.Objs[hit.ID];
            switch (type){
                case "point":
                    current_point.transform.position = hit.transform.position;
                    break;
                case "line":
                    var start = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[0]].go.transform.position;
                    var end = Hierarchy.Objs[Hierarchy.Objs[hit.ID].vertices[1]].go.transform.position;
                    current_point.transform.position = draw.calc.hc_diem_dt(hit.point, new KeyValuePair<Vector3, Vector3>(start, end));
                    break;
                case "plane":
                case "sphere":
                    current_point.transform.position = hit.point;
                    break;
                case "circle":
                    var vertices = new Vector3[obj.vertices.Count];
                    for (int i=0;i<obj.vertices.Count;i++){
                        vertices[i] = draw.calc.ztoy(Hierarchy.Objs[obj.vertices[i]].go.transform.position);
                    }
                    var center = new Vector3();
                    if (obj.vertices.Count==2) center = vertices[0];
                    else center = draw.calc.tam_dg_tron_ngtiep(vertices[0], vertices[1], vertices[2]);
                    current_point.transform.position = draw.calc.ztoy(draw.calc.kc_sang_toa_do(center, draw.calc.ztoy(hit.point), Vector3.Distance(center, vertices[1])));
                    break;
                case "polygon":
                    float dist(Vector3 point){
                        return Vector3.Distance(hit.point, point);
                    }
                    start = Vector3.positiveInfinity;
                    end = new Vector3();
                    var ln = obj.go.GetComponent<LineRenderer>();
                    var index = -1;
                    for (int i=0;i<ln.positionCount;i++){
                        if (dist(ln.GetPosition(i)) <= dist(start)){
                            start = ln.GetPosition(i);
                            index = i;
                        }
                    }
                    if (index == 0) end = (dist(ln.GetPosition(1)) < dist(ln.GetPosition(ln.positionCount-1)) ? ln.GetPosition(1) : ln.GetPosition(ln.positionCount-1));
                    else if (index == ln.positionCount-1) end = (dist(ln.GetPosition(0)) < dist(ln.GetPosition(ln.positionCount-2)) ? ln.GetPosition(0) : ln.GetPosition(ln.positionCount-2));
                    else end = (dist(ln.GetPosition(index-1)) < dist(ln.GetPosition(index+1)) ? ln.GetPosition(index-1) : ln.GetPosition(index+1));
                    current_point.transform.position = draw.calc.hc_diem_dt(hit.point, new KeyValuePair<Vector3, Vector3>(start, end));
                    break;
            }
        }
        else{
            if (!current_point.activeSelf) current_point.SetActive(true);
            draw.mouse.Follow(current_point);
            SnapOnAxis(current_point, hit);
        }
        draw.input.Vec2Input(PosInputs, draw.calc.ztoy(current_point.transform.position));
    }
    public void onClick(INPUT Name){
        var hit = draw.raycast.Hit();
        var parents = new List<string>();
        if (Hierarchy.Objs.ContainsKey(hit.ID)){
            if (Hierarchy.Objs[hit.ID].type == "point"){
                Destroy(current_point);
                current_point = Hierarchy.Objs[hit.ID].go;
            }
            else{
                parents.Add(hit.ID);
                parents.AddRange(Hierarchy.Objs[hit.ID].parents);
                draw.hier.AddPoint(Name.text, current_point, parents, true, draw.inRoom());
            }
        }
        else{
            draw.hier.AddPoint(Name.text, current_point, parents, true, draw.inRoom());
        }
    }
    public void Drag(GameObject point){
        var parents = Hierarchy.Objs[point.name].parents;
        var hit = draw.raycast.Hit();
        if (parents.Count == 0){
            draw.mouse.Follow(point);
            SnapOnAxis(point, draw.raycast.Hit());
        }
        else{
            var mouseRay = draw.raycast.MouseToRay();
            var obj = Hierarchy.Objs[parents[0]];
            switch (Hierarchy.Objs[parents[0]].type){
                case "line":
                    var start = Hierarchy.Objs[parents[0]].vertices[0];
                    var end = Hierarchy.Objs[parents[0]].vertices[1];
                    var line = new KeyValuePair<Vector3, Vector3>(draw.calc.ztoy(Hierarchy.Objs[start].go.transform.position), draw.calc.ztoy(Hierarchy.Objs[end].go.transform.position));
                    point.transform.position = draw.calc.ztoy(draw.calc.duong_vgc(line, mouseRay).Key);
                    break;
                case "plane":
                    var plane = Hierarchy.Objs[parents[0]].equation;
                    point.transform.position = draw.calc.ztoy(draw.calc.gd_dt_mp(mouseRay, plane).Value);
                    break;
                case "circle":
                    var vertices = new Vector3[obj.vertices.Count];
                    for (int i=0;i<obj.vertices.Count;i++){
                        vertices[i] = draw.calc.ztoy(Hierarchy.Objs[obj.vertices[i]].go.transform.position);
                    }
                    var pos = new Vector3();
                    var center = new Vector3();
                    if (obj.vertices.Count == 2){
                        pos = draw.calc.gd_dt_mp(mouseRay, Hierarchy.Objs[obj.parents[0]].equation).Value;
                        center = vertices[0];
                    }
                    else{
                        center = draw.calc.tam_dg_tron_ngtiep(vertices[0], vertices[1], vertices[2]);
                        pos = draw.calc.gd_dt_mp(mouseRay, draw.calc.pt_mp(vertices[0], vertices[1], vertices[2])).Value;
                    }
                    point.transform.position = draw.calc.ztoy(draw.calc.kc_sang_toa_do(center, pos, Vector3.Distance(center, vertices[1])));
                    break;
                case "sphere":
                    draw.mouse.Follow(point);
                    if (hit.ID == parents[0]){
                        point.transform.position = hit.point;
                    }
                    else{
                        center = Hierarchy.Objs[obj.vertices[0]].go.transform.position;
                        var vertex = Hierarchy.Objs[obj.vertices[1]].go.transform.position;
                        point.transform.position = draw.calc.kc_sang_toa_do(center, point.transform.position, Vector3.Distance(center, vertex));
                    }
                    break;
                case "polygon":
                    plane = Hierarchy.Objs[obj.parents[0]].equation;
                    pos = draw.calc.ztoy(draw.calc.gd_dt_mp(mouseRay, plane).Value);
                    var ln = obj.go.GetComponent<LineRenderer>();
                    var bestpos = Vector3.positiveInfinity;
                    int beststart=0, bestend = 1;
                    for (int i=0;i<ln.positionCount;i++){
                        int startid = (i==0 ? ln.positionCount-1 : i-1), endid = i;
                        var startpos = ln.GetPosition(startid);
                        var endpos = ln.GetPosition(endid);
                        var intersect = draw.calc.duong_vgc(new KeyValuePair<Vector3, Vector3>(Hierarchy.Objs[obj.vertices[0]].go.transform.position, pos), new KeyValuePair<Vector3, Vector3>(startpos, endpos));
                        var maxdist = Vector3.Distance(startpos, endpos);
                        if (Vector3.Distance(startpos, intersect.Key) <= maxdist && Vector3.Distance(endpos, intersect.Key) <= maxdist){
                            if (Vector3.Distance(pos, intersect.Key) <= Vector3.Distance(pos, bestpos)){
                                bestpos = intersect.Key;
                                beststart = startid;
                                bestend = endid;
                            }
                        }
                    }
                    obj.go.GetComponent<DynamicPolygon>().ChildrenOnSide[point.name] = new KeyValuePair<int, int>(beststart, bestend);
                    point.transform.position = bestpos;
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
