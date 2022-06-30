using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hierarchy : MonoBehaviour
{
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
    void Update(){
    }
    public void AddPoint(string name, string parent, GameObject go){
        var obj = new Point();
        obj.name = name;
        obj.parent = parent;
        obj.go = go;
        Points.Add(go.name, obj);
        Types.Add(go.name, "point");
    }
    public void RemovePoint(string ID){
        Destroy(Points[ID].go);
        Points.Remove(ID);
        Types.Remove(ID);
    }
    public void RemoveLine(string ID){
        Destroy(Lines[ID].go.gameObject);
        Lines.Remove(ID);
        Types.Remove(ID);
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
    
}
