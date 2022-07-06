using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHandler : MonoBehaviour
{
    Draw draw;
    public Material[] plane, defaultMat;
    public Transform Highlighted = null;
    public List<Transform> Selected = new List<Transform>();
    public Dictionary<string, int> SelectionCount = new Dictionary<string, int>();
    public static List<string> Types = new List<string>(){"point", "line", "plane", "3pointcircle", "polygon"};
    void Start(){
        draw = FindObjectOfType<Draw>();
        foreach (string type in Types) SelectionCount.Add(type, 0);
    }
    public void Follow(GameObject gameObject){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    void Update(){
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
        if (!Selected.Contains(obj) && hit.ID != "Background") Highlight(obj);

        // if (draw.drawing) return;

        // if (Input.GetMouseButtonDown(0)){
        //     if (Hierarchy.Types.ContainsKey(hit.ID)){
        //         if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
        //             if (!Selected.Contains(obj)){
        //                 if (Selected.Count==0) OnSelect(obj);
        //                 else{
        //                     Select(obj);
        //                     draw.Refresh();
        //                 }
        //             }
        //             else{
        //                 Unselect(obj);
        //             }
        //         }
        //         else{
        //             if (!Selected.Contains(obj)){
        //                 UnselectAll();
        //                 draw.Refresh();
        //                 OnSelect(obj);
        //             }
        //         }
        //     }
        //     else{
        //         UnselectAll();
        //         draw.Refresh();
        //     }
        // }
    }
    public void OnSelect(Transform obj){
        // var hierItem = draw.hier.content.Find(obj.name).GetComponent<Toggle>();
        // if (!hierItem.isOn) hierItem.isOn = true;
        // if (!Selected.Contains(obj)){
        //     switch (Hierarchy.Types[obj.name]){
        //         case "point":
        //             draw.current = StartCoroutine(draw.point.OnSelect(obj.gameObject));
        //             break;
        //         case "line":
        //             draw.current = StartCoroutine(draw.line.OnSelect(obj.GetComponent<LineRenderer>()));
        //             break;
        //         case "plane":
        //             draw.current = StartCoroutine(draw.plane.OnSelect(obj.gameObject));
        //             break;
        //         case "3pointcircle":
        //             draw.current = StartCoroutine(draw.circle3.OnSelect(obj.GetComponent<LineRenderer>()));
        //             break;
        //         case "polygon":
        //             draw.current = StartCoroutine(draw.polygon.OnSelect(obj.GetComponent<LineRenderer>()));
        //             break;
        //     }
        // }
    }
    public void DeleteSelected(){
        foreach (var selection in Selected){
            // draw.hier.RemoveObjectsWithID(selection.name);
        }
        Selected.Clear();
        SelectionCount.Clear();
        foreach (string type in Types) SelectionCount.Add(type, 0);
        OnSelectionsChange();
        UnselectAll();
        draw.Refresh();
    }
    public void Select(Transform obj){
        SetMaterial(obj, 2);
        if (!Selected.Contains(obj)){
            Selected.Add(obj);
            // SelectionCount[Hierarchy.Types[obj.name]]++;
        }
        OnSelectionsChange();
    }
    public void Unselect(Transform obj){
        SetMaterial(obj, 0);
        Selected.Remove(obj);
        // SelectionCount[Hierarchy.Types[obj.name]]--;
        OnSelectionsChange();
    }
    public void UnselectAll(){
        foreach (Transform obj in Selected){
            SetMaterial(obj, 0);
        }
        Selected.Clear();
        foreach (string type in Types){
            SelectionCount[type] = 0;
        }
        OnSelectionsChange();
    }
    public void Highlight(Transform obj){
        Highlighted = obj;
        SetMaterial(Highlighted, 1);
    }
    public void Unhighlight(){
        if (!Selected.Contains(Highlighted)) SetMaterial(Highlighted, 0);
        Highlighted = null;        
    }
    public void OnSelectionsChange(){
        bool noselected = Selected.Count == 0;
        draw.menu.Buttoggle("Xoá", !noselected);
    }
    public void SetMaterial(Transform obj, int state){
        // if (obj == null || !Hierarchy.Types.ContainsKey(obj.name)) return;
        // switch (Hierarchy.Types[obj.name]){
        //     case "point":
        //         obj.Find("dot").GetComponent<MeshRenderer>().material = defaultMat[state];
        //         break;
        //     case "line":
        //     case "3pointcircle":
        //     case "polygon":
        //         obj.GetComponent<LineRenderer>().material = defaultMat[state];
        //         break;
        //     case "plane":
        //         obj.GetComponent<MeshRenderer>().material = plane[state];
        //         break;
        // }
    }
}