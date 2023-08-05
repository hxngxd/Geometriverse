using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class MouseHandler : MonoBehaviour
{
    Draw draw;
    public Material[] trans, movable_point, default_, vertex, line;
    public Transform Highlighted = null;
    public Dictionary<string, List<Transform>> Selected = new Dictionary<string, List<Transform>>();
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void Follow(GameObject gameObject){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    void Update(){
        
        if (Highlighted != null) Unhighlight();

        int count = 0;
        foreach (var s in Selected){
            count += s.Value.Count;
            if (count != 0) break;
        }
        if (count != 0){
            if (Input.GetKeyDown(KeyCode.Escape)){
                UnselectAll();
                draw.Refresh();
            }
            else if (Input.GetKeyDown(KeyCode.Delete)){
                DeleteSelected();
            }
        }
        PickUp();
    }
    void PickUp(){
        if (draw.raycast.isMouseOverUI()) return;

        var hit = draw.raycast.Hit();
        var obj = hit.transform;
        if (Hierarchy.Objs.ContainsKey(obj.name) && !Selected[Hierarchy.Objs[obj.name].type].Contains(obj)) Highlight(obj);

        if (draw.isDrawing) return;

        if (Input.GetMouseButtonDown(0)){
            if (Hierarchy.Objs.ContainsKey(obj.name)){
                var type = Hierarchy.Objs[obj.name].type;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
                    if (!Selected[type].Contains(obj)){
                        if (Selected[type].Count==0){
                            OnSelect(obj);
                            foreach (var i in Hierarchy.Items) draw.hier.ToggleItem(i.Key, 0);
                        }
                        else{
                            Select(obj);
                            draw.Refresh();
                        }
                        draw.hier.ToggleItem(obj.name, 2);
                    }
                    else{
                        Unselect(obj);
                        draw.hier.ToggleItem(obj.name, 0);
                        if (Selected[type].Count==1){
                            draw.Refresh();
                            OnSelect(Selected[type][0]);
                            draw.hier.ToggleItem(Selected[type][0].name, 2);
                        }
                    }
                }
                else{
                    if (!Selected[type].Contains(obj)){
                        UnselectAll();
                        draw.Refresh();
                        OnSelect(obj);
                        foreach (var i in Hierarchy.Items) draw.hier.ToggleItem(i.Key, 0);
                        draw.hier.ToggleItem(obj.name, 2);
                    }
                }
            }
            else{
                UnselectAll();
                draw.Refresh();
            }
        }
    }
    public void OnSelect(Transform obj){
        Select(obj);
        var type = Hierarchy.Objs[obj.name].type;
        if (type == "point"){
            draw.StartC(draw.point.OnSelect(obj.gameObject));
        }
        else{
            draw.StartC(draw.OnSelect(obj.gameObject, draw.Types[type]));
        }
    }
    public void DeleteSelected(){
        foreach (var selection in Selected){
            foreach (var obj in selection.Value){
                if (draw.inRoom()){
                    draw.hier.photon.RPC("Remove", RpcTarget.AllBuffered, obj.name);
                }
                else{
                    draw.hier.Remove(obj.name);
                }
            }
            selection.Value.Clear();
        }
        draw.Refresh();
        OnSelectionsChange();
    }
    public void Select(Transform obj){
        SetMaterial(obj, 2);
        var type = Hierarchy.Objs[obj.name].type;
        if (!Selected[type].Contains(obj)) Selected[type].Add(obj);
        OnSelectionsChange();
    }
    public void Unselect(Transform obj){
        SetMaterial(obj, 0);
        Selected[Hierarchy.Objs[obj.name].type].Remove(obj);
        OnSelectionsChange();
    }
    public void UnselectAll(){
        foreach (var selection in Selected){
            foreach (var obj in selection.Value){
                SetMaterial(obj, 0);
            }
            selection.Value.Clear();
        }
        foreach (var i in Hierarchy.Items) draw.hier.ToggleItem(i.Key, 0);
        OnSelectionsChange();
    }
    public void Highlight(Transform obj){
        Highlighted = obj;
        draw.hier.ToggleItem(obj.name, 1);
        SetMaterial(Highlighted, 1);
    }
    public void Unhighlight(){
        var type = Hierarchy.Objs[Highlighted.name].type;
        if (!Selected[type].Contains(Highlighted)){
            SetMaterial(Highlighted, 0);
            draw.hier.ToggleItem(Highlighted.name, 0);
        }
        Highlighted = null;        
    }
    public void OnSelectionsChange(){
        bool empty = true;
        foreach (var s in Selected){
            if (s.Value.Count != 0){
                empty = false;
                break;
            }
        }
        if (!draw.allowDrawing) return;
        draw.menu.Enable("Chỉnh sửa/Xoá", !empty);
    }
    public void SetMaterial(Transform obj, int state){
        if (Hierarchy.Objs[obj.name].type == "point"){
            var mesh = obj.Find("dot").GetComponent<MeshRenderer>();
            if (Hierarchy.Objs[obj.name].movable){
                mesh.material = (Hierarchy.Objs[obj.name].vertexof.Count == 0 ? movable_point[state] : vertex[state]);
            }
            else if (!Hierarchy.Objs[obj.name].movable) mesh.material = default_[state];
        }
        else{
            if (obj.GetComponent<LineRenderer>() != null){
                obj.GetComponent<LineRenderer>().material = line[state];
            }
            else{
                obj.GetComponent<MeshRenderer>().material = trans[state];
            }
        }
    }
}