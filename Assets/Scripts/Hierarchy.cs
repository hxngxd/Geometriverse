using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
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
    public struct Plane{
        public string name;
        public bool expand;
        public GameObject go;
        public List<string> vertices;
        public List<string> children;
        public Matrix<double> rotation;
        public Dictionary<string, float> equation;
    }
    public static Dictionary<string, List<GameObject>> currentObjects = new Dictionary<string, List<GameObject>>(){
        {"point", new List<GameObject>()},
        {"line", new List<GameObject>()},
        {"plane", new List<GameObject>()},
    };
    public static Dictionary<string, string> Types = new Dictionary<string, string>(); 
    public static Dictionary<string, Point> Points = new Dictionary<string, Point>();
    public static Dictionary<string, Line> Lines = new Dictionary<string, Line>();
    public static Dictionary<string, Plane> Planes = new Dictionary<string, Plane>();
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
            else{
                draw.mouse.Unselect(go.transform);
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

        var roundedPos = draw.calc.roundVec3(go.transform.position, 2);
        string s = $"Điểm: {name}({roundedPos.x},{roundedPos.z},{roundedPos.y})";
        if (parent != ""){
            switch (Hierarchy.Types[parent]){
                case "line":
                    s += $"\n(Thuộc đoạn thẳng: {Hierarchy.Lines[parent].name})";
                    break;
                case "plane":
                    s += $"\n(Thuộc mặt phẳng: {Hierarchy.Planes[parent].name})";
                    break;
            }
        }
        AddItem(go.gameObject, s);
    }
    public void RemovePoint(string ID){
        Destroy(Points[ID].go);
        Points.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void AddLine(string name, string start, string end, string plane, LineRenderer go, List<string> children){
        var obj = new Line();
        obj.name = name;
        obj.start = start;
        obj.end = end;
        obj.plane = plane;
        obj.go = go;
        obj.children = children;
        Lines.Add(go.name, obj);
        Types.Add(go.name, "line");

        string s = $"Đoạn thẳng: {name}";
        if (plane != ""){
            s += $"\n(Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);
    }
    public void RemoveLine(string ID){
        Destroy(Lines[ID].go.gameObject);
        Lines.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void AddPlane(string name, bool expand, List<string> vertices, List<string> children, Matrix<double> rotation, GameObject go, Dictionary<string, float> equation){
        var obj = new Plane();
        obj.name = name;
        obj.expand = expand;
        obj.vertices = vertices;
        obj.children = children;
        obj.rotation = rotation;
        obj.go = go;
        obj.equation = equation;
        Planes.Add(go.name, obj);
        Types.Add(go.name, "plane");

        AddItem(go, $"Mặt phẳng: {name}");
    }
    public void RemovePlane(string ID){
        Destroy(Planes[ID].go);
        Planes.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void RemoveCurrentObjects(){
        foreach (var obj in currentObjects){
            foreach (var go in obj.Value){
                try{
                    RemoveObjectsWithID(go.name);
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
            RemoveObjectsWithID(ID);
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
    public void RemoveObjectsWithID(string ID){
        switch (Hierarchy.Types[ID]){
            case "point":
                RemovePoint(ID);
                break;
            case "line":
                RemoveLine(ID);
                break;
            case "plane":
                RemovePlane(ID);
                break;
        }
    }
}
