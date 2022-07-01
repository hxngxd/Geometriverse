using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hierarchy : MonoBehaviour
{
    Draw draw;
    public struct Point{
        public string name, parent;
        public GameObject go;
    }
    public struct Line{
        public string name, start, end, plane;
        public LineRenderer go;
        public List<string> children;
    }
    public static Dictionary<string, List<GameObject>> currentObjects = new Dictionary<string, List<GameObject>>(){
        {"point", new List<GameObject>()},
        {"line", new List<GameObject>()},
    };
    public static Dictionary<string, string> Types = new Dictionary<string, string>(); 
    public static Dictionary<string, Point> Points = new Dictionary<string, Point>();
    public static Dictionary<string, Line> Lines = new Dictionary<string, Line>();
    public Transform content;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void AddItem(GameObject go, string name){
        //MULTI SELECT PLEASE
        string ID = go.name;
        draw.uiobj.HierarchyItem(ID, name, () => {
            var item = content.Find(ID).GetComponent<Toggle>();
            if (item.isOn){
                foreach (Transform child in content){
                    if (child.name != ID){
                        child.GetComponent<Toggle>().isOn = false;
                    }
                }
                draw.mouse.UnselectAll();
                draw.Refresh();
                draw.mouse.OnSelect(go.transform);
            }
        }, content);
    }
    public void UnselectAllItem(){
        if (content.childCount==0) return;
        foreach (Transform child in content){
            child.GetComponent<Toggle>().isOn = false;
        }
    }
    public void AddPoint(string name, string parent, GameObject go){
        var obj = new Point();
        obj.name = name;
        obj.parent = parent;
        obj.go = go;
        Points.Add(go.name, obj);
        Types.Add(go.name, "point");

        var pos = draw.calc.roundVec3(go.transform.position, 2);
        AddItem(go, $"Điểm {name}({pos.x},{pos.z},{pos.y})");
    }
    public void RemovePoint(string ID){
        Destroy(Points[ID].go);
        Points.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void AddLine(string name, string start, string end, string plane, LineRenderer go, List<string> Children){
        var obj = new Line();
        obj.name = name;
        obj.start = start;
        obj.end = end;
        obj.plane = plane;
        obj.go = go;
        obj.children = Children;
        Lines.Add(go.name, obj);
        Types.Add(go.name, "line");

        AddItem(go.gameObject, $"Đoạn thẳng {name} (Điểm đầu: {Hierarchy.Points[start].name}, điểm cuối: {Hierarchy.Points[end].name})");
    }
    public void RemoveLine(string ID){
        Destroy(Lines[ID].go.gameObject);
        Lines.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void RemoveCurrentObjects(){
        foreach (var obj in currentObjects){
            foreach (var go in obj.Value){
                try{
                    string ID = go.name;
                    switch (obj.Key){
                        case "point":
                            RemovePoint(ID);
                            break;
                        case "line":
                            RemoveLine(ID);
                            break;
                    }
                }
                catch{
                    Destroy(go);
                }
            }
            obj.Value.Clear();
        }
    }
    public void ResetCurrentObjects(){
        foreach (var obj in currentObjects){
            obj.Value.Clear();
        }
    }
    public void ResetAll(){
        RemoveCurrentObjects();
        List<string> IDs = new List<string>();
        foreach (var obj in Hierarchy.Types) IDs.Add(obj.Key);
        foreach (string ID in IDs){
            switch (Hierarchy.Types[ID]){
                case "point":
                    RemovePoint(ID);
                    break;
                case "line":
                    RemoveLine(ID);
                    break;
            }
        }
        Types.Clear();
        Points.Clear();
        Lines.Clear();
        draw.cam.ResetCamera();
        draw.Refresh();
        draw.mouse.Selected.Clear();
        draw.mouse.SelectionCount.Clear();
        foreach (string type in draw.mouse.Types) draw.mouse.SelectionCount.Add(type, 0);
        draw.mouse.OnSelectionsChange();
    }
    
}
