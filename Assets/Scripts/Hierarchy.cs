using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Photon.Pun;
using Photon.Realtime;
public class Hierarchy : MonoBehaviour
{
    Draw draw;
    public PhotonView photon;
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
    public struct Circle{
        public string name, center, vertex, plane;
        public LineRenderer go;
        public List<string> vertices;
        public List<string> children;
        public Matrix<double> rotation;
        public Dictionary<string, float> equation;
    }
    public struct Polygon{
        public string name, center, vertex, plane;
        public int step;
        public LineRenderer go;
        public List<string> children;
        public Dictionary<string, float> equation;
        public bool type;
    }
    public static Dictionary<string, List<GameObject>> currentObjects = new Dictionary<string, List<GameObject>>(){
        {"point", new List<GameObject>()},
        {"line", new List<GameObject>()},
        {"plane", new List<GameObject>()},
        {"3pointcircle", new List<GameObject>()},
        {"polygon", new List<GameObject>()}
    };
    public static Dictionary<string, string> Types = new Dictionary<string, string>(); 
    public static Dictionary<string, Point> Points = new Dictionary<string, Point>();
    public static Dictionary<string, Line> Lines = new Dictionary<string, Line>();
    public static Dictionary<string, Plane> Planes = new Dictionary<string, Plane>();
    public static Dictionary<string, Circle> Circles = new Dictionary<string, Circle>();
    public static Dictionary<string, Polygon> Polygons = new Dictionary<string, Polygon>();
    public Transform content;
    void Start(){
        draw = FindObjectOfType<Draw>();
        photon = GetComponent<PhotonView>();
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
                    s += $" (Thuộc đoạn thẳng: {Hierarchy.Lines[parent].name})";
                    break;
                case "plane":
                    s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[parent].name})";
                    break;
                case "3pointcircle":
                    s += $" (Thuộc đường tròn: {Hierarchy.Circles[parent].name})";
                    break;
            }
        }
        AddItem(go.gameObject, s);

        if (RoomManager.inRoom){
            photon.RPC("RPC_AddPoint", RpcTarget.OthersBuffered, name, go.name, parent, go.transform.position);
        }
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
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);

        if (RoomManager.inRoom){
            photon.RPC("RPC_AddLine", RpcTarget.OthersBuffered, name, go.name, start, end, plane, children.ToArray());
        }
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

        if (RoomManager.inRoom){
            photon.RPC("RPC_AddPlane", RpcTarget.OthersBuffered, name, go.name, expand, vertices.ToArray(), children.ToArray(), MatrixToArray(rotation), equation);
        }
    }
    public void RemovePlane(string ID){
        Destroy(Planes[ID].go);
        Planes.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void AddCircle(string name, string center, string vertex, string plane, LineRenderer go, List<string> vertices, List<string> children, Matrix<double> rotation, Dictionary<string, float> equation){
        var obj = new Circle();
        obj.name = name;
        obj.center = center;
        obj.vertex = vertex;
        obj.plane = plane;
        obj.go = go;
        obj.vertices = vertices;
        obj.children = children;
        obj.rotation = rotation;
        obj.equation = equation;
        Circles.Add(go.name, obj);
        Types.Add(go.name, "3pointcircle");

        string s = $"Đường tròn: {name}";
        if (plane != ""){
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);

        if (RoomManager.inRoom){
            photon.RPC("RPC_AddCircle", RpcTarget.OthersBuffered, name, go.name, center, vertex, plane, vertices.ToArray(), children.ToArray(), MatrixToArray(rotation), equation);
        }
    }
    public void RemoveCircle(string ID){
        Destroy(Circles[ID].go.gameObject);
        Circles.Remove(ID);
        Types.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void AddPolygon(string name, string center, string vertex, string plane, int step, LineRenderer go, List<string> children, Dictionary<string, float> equation, bool type){
        var obj = new Polygon();
        obj.name = name;
        obj.center = center;
        obj.vertex = vertex;
        obj.plane = plane;
        obj.step = step;
        obj.go = go;
        obj.children = children;
        obj.equation = equation;
        obj.type = type;
        Polygons.Add(go.name, obj);
        Types.Add(go.name, "polygon");
        
        string s = (type ? $"Đường tròn: {name}" : $"Đa giác: {name}");
        if (plane != ""){
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);

        if (RoomManager.inRoom) {
            photon.RPC("RPC_AddPolygon", RpcTarget.OthersBuffered, name, go.name, center, vertex, plane, step, children.ToArray(), equation, type);
        }
    }
    public void RemovePolygon(string ID){
        Destroy(Polygons[ID].go.gameObject);
        Polygons.Remove(ID);
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
        foreach (string type in MouseHandler.Types) draw.mouse.SelectionCount.Add(type, 0);
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
            case "3pointcircle":
                RemoveCircle(ID);
                break;
            case "polygon":
                RemovePolygon(ID);
                break;
        }
    }
    [PunRPC]
    public void RPC_AddPoint(string name, string ID, string parent, Vector3 position){
        var go = draw.obj.Point(position, draw.hierContent);
        go.name = ID;
        go.GetComponent<SphereCollider>().enabled = true;
        var obj = new Point();
        obj.name = name;
        obj.parent = parent;
        obj.go = go;
        Points.Add(ID, obj);
        Types.Add(ID, "point");

        var roundedPos = draw.calc.roundVec3(position, 2);
        string s = $"Điểm: {name}({roundedPos.x},{roundedPos.z},{roundedPos.y})";
        if (parent != ""){
            switch (Hierarchy.Types[parent]){
                case "line":
                    s += $" (Thuộc đoạn thẳng: {Hierarchy.Lines[parent].name})";
                    break;
                case "plane":
                    s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[parent].name})";
                    break;
                case "3pointcircle":
                    s += $" (Thuộc đường tròn: {Hierarchy.Circles[parent].name})";
                    break;
            }
        }
        AddItem(go, s);
    }
    [PunRPC]
    public void RPC_AddLine(string name, string ID, string start, string end, string plane, string[] children){
        var go = draw.obj.Line(draw.hierContent);
        go.name = ID;
        go.positionCount = 2;
        var obj = new Line();
        obj.name = name;
        obj.start = start;
        obj.end = end;
        obj.plane = plane;
        obj.go = go;
        obj.children = new List<string>(children);
        Lines.Add(ID, obj);
        Types.Add(ID, "line");

        string s = $"Đoạn thẳng: {name}";
        if (plane != ""){
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);
    }
    [PunRPC]
    public void RPC_AddPlane(string name, string ID, bool expand, string[] vertices, string[] children, float[] rotation, Dictionary<string, float> equation){
        var go = draw.obj.Plane(draw.hierContent);
        go.name = ID;
        var obj = new Plane();
        obj.name = name;
        obj.expand = expand;
        obj.vertices = new List<string>(vertices);
        obj.children = new List<string>(children);
        obj.rotation = ArrayToMatrix(rotation);
        obj.go = go;
        obj.equation = equation;
        Planes.Add(ID, obj);
        Types.Add(ID, "plane");

        AddItem(go, $"Mặt phẳng: {name}");
    }
    [PunRPC]
    public void RPC_AddCircle(string name, string ID, string center, string vertex, string plane, string[] vertices, string[] children, float[] rotation, Dictionary<string, float> equation){
        var go = draw.obj.Polygon(draw.hierContent);
        go.gameObject.AddComponent<DynamicCircle>();
        go.name = ID;
        var obj = new Circle();
        obj.name = name;
        obj.center = center;
        obj.vertex = vertex;
        obj.plane = plane;
        obj.go = go;
        obj.vertices = new List<string>(vertices);
        obj.children = new List<string>(children);
        obj.rotation = ArrayToMatrix(rotation);
        obj.equation = equation;
        Circles.Add(ID, obj);
        Types.Add(ID, "3pointcircle");

        string s = $"Đường tròn: {name}";
        if (plane != ""){
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);
    }
    [PunRPC]
    public void RPC_AddPolygon(string name, string ID, string center, string vertex, string plane, int step, string[] children, Dictionary<string, float> equation, bool type){
        var go = draw.obj.Polygon(draw.hierContent);
        go.gameObject.AddComponent<DynamicPolygon>();
        go.name = ID;
        var obj = new Polygon();
        obj.name = name;
        obj.center = center;
        obj.vertex = vertex;
        obj.plane = plane;
        obj.step = step;
        obj.go = go;
        obj.type = type;
        obj.children = new List<string>(children);
        obj.equation = equation;
        Polygons.Add(ID, obj);
        Types.Add(ID, "polygon");

        string s = (type ? $"Đường tròn: {name}" : $"Đa giác: {name}");
        if (plane != ""){
            s += $" (Thuộc mặt phẳng: {Hierarchy.Planes[plane].name})";
        }
        AddItem(go.gameObject, s);
    }
    public Matrix<double> ArrayToMatrix(float[] M){
        return DenseMatrix.OfArray(new double[,]{
            {M[0], M[1], M[2]},
            {M[3], M[4], M[5]},
            {M[6], M[7], M[8]}
        });
    }
    public Array MatrixToArray(Matrix<double> M){
        List<float> res = new List<float>(){(float)M.Row(0)[0], (float)M.Row(0)[1], (float)M.Row(0)[2], (float)M.Row(1)[0], (float)M.Row(1)[1], (float)M.Row(1)[2], (float)M.Row(2)[0], (float)M.Row(2)[1], (float)M.Row(2)[2]};
        return res.ToArray();
    }
}
