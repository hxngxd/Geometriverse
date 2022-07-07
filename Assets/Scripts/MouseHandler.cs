using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHandler : MonoBehaviour
{
    Draw draw;
    public Material[] trans, solid;
    public Transform Highlighted = null;
    public Dictionary<string, List<Transform>> Selected = new Dictionary<string, List<Transform>>(){
        {"point", new List<Transform>()},
    };
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void Follow(GameObject gameObject){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    void Update(){
        string a = "";
        foreach (var t in Selected){
            foreach (var s in t.Value) a += s.name + " ";
        }
        print(a);

        if (Highlighted != null) Unhighlight();

        if (Selected.Count != 0){
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

        if (draw.current != null) return;

        if (Input.GetMouseButtonDown(0)){
            if (Hierarchy.Objs.ContainsKey(obj.name)){
                var type = Hierarchy.Objs[obj.name].type;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
                    if (!Selected[type].Contains(obj)){
                        if (Selected[type].Count==0) OnSelect(obj);
                        else{
                            Select(obj);
                            draw.Refresh();
                        }
                    }
                    else{
                        Unselect(obj);
                        if (Selected[type].Count==1){
                            draw.Refresh();
                            OnSelect(Selected[type][0]);
                        }
                    }
                }
                else{
                    if (!Selected[type].Contains(obj)){
                        UnselectAll();
                        draw.Refresh();
                        OnSelect(obj);
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
        switch (Hierarchy.Objs[obj.name].type){
            case "point":
                StartCoroutine(draw.point.OnSelect(obj.gameObject));
                break;
            // case "line":
            //     draw.current = StartCoroutine(draw.line.OnSelect(obj.GetComponent<LineRenderer>()));
            //     break;
            // case "plane":
            //     draw.current = StartCoroutine(draw.plane.OnSelect(obj.gameObject));
            //     break;
            // case "circle":
            //     draw.current = StartCoroutine(draw.circle3.OnSelect(obj.GetComponent<LineRenderer>()));
            //     break;
        }
    }
    public void DeleteSelected(){
        foreach (var selection in Selected){
            foreach (var obj in selection.Value){
                draw.hier.Remove(obj.name);
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
        OnSelectionsChange();
    }
    public void Highlight(Transform obj){
        Highlighted = obj;
        SetMaterial(Highlighted, 1);
    }
    public void Unhighlight(){
        var type = Hierarchy.Objs[Highlighted.name].type;
        if (!Selected[type].Contains(Highlighted)) SetMaterial(Highlighted, 0);
        Highlighted = null;        
    }
    public void OnSelectionsChange(){
        bool noselected = Selected.Count == 0;
        draw.menu.Buttoggle("Xoá", !noselected);
    }
    public void SetMaterial(Transform obj, int state){
        switch (Hierarchy.Objs[obj.name].type){
            case "point":
                obj.Find("dot").GetComponent<MeshRenderer>().material = solid[state];
                break;
            case "line":
            case "circle":
                obj.GetComponent<LineRenderer>().material = solid[state];
                break;
            case "plane":
                obj.GetComponent<MeshRenderer>().material = trans[state];
                break;
        }
    }
}