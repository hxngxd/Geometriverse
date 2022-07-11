using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class Hierarchy : MonoBehaviour
{
    Draw draw;
    public struct Obj{
        public string name, parent, type;
        public GameObject go;
        public List<string> vertices, children, vertexof;
        public Dictionary<string, float> equation;
        public Matrix<double> rotation;
        public bool expand;
    }
    public static Dictionary<string, Obj> Objs = new Dictionary<string, Obj>();
    public Transform created, current, content;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void AddPoint(string name, string parent, GameObject go){
        go.GetComponent<SphereCollider>().enabled = true;
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        if (parent != "") Objs[parent].children.Add(go.name);
        obj.vertexof = new List<string>();
        obj.go = go;
        obj.type = "point";
        Objs.Add(go.name, obj);
    }
    public void AddLine(string name, string parent, List<string> vertices, GameObject go, Dictionary<string, float> equation){
        go.transform.SetParent(created);
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        if (parent != "") Objs[parent].children.Add(go.name);
        obj.vertices = vertices;
        foreach (var vertex in vertices) Objs[vertex].vertexof.Add(go.name);
        obj.go = go;
        obj.equation = equation;
        obj.children = new List<string>();
        obj.type = "line";
        Objs.Add(go.name, obj);
    }
    public void AddPlane(string name, List<string> vertices, GameObject go, Matrix<double> rotation, Dictionary<string, float> equation, bool expand){
        go.transform.SetParent(created);
        var obj = new Obj();
        obj.name = name;
        obj.vertices = vertices;
        foreach (var vertex in vertices) Objs[vertex].vertexof.Add(go.name);
        obj.go = go;
        obj.rotation = rotation;
        obj.equation = equation;
        obj.children = new List<string>();
        obj.expand = expand;
        obj.type = "plane";
        Objs.Add(go.name, obj);
    }
    public void AddCircle(string name, string parent, List<string> vertices, GameObject go, Matrix<double> rotation, Dictionary<string, float> equation){
        go.transform.SetParent(created);
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        if (parent != "") Objs[parent].children.Add(go.name);
        obj.go = go;
        foreach (var vertex in vertices) Objs[vertex].vertexof.Add(go.name);
        obj.vertices = vertices;
        obj.rotation = rotation;
        obj.equation = equation;
        obj.children = new List<string>();
        obj.type = "circle";
        Objs.Add(go.name, obj);
    }
    public void AddCircle(string name, string parent, List<string> vertices, GameObject go, Dictionary<string, float> equation){
        go.transform.SetParent(created);
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        if (parent != "") Objs[parent].children.Add(go.name);
        obj.go = go;
        foreach (var vertex in vertices) Objs[vertex].vertexof.Add(go.name);
        obj.vertices = vertices;
        obj.equation = equation;
        obj.children = new List<string>();
        obj.type = "circle";
        Objs.Add(go.name, obj);
    }
    public void AddSphere(string name, List<string> vertices, GameObject go, Dictionary<string, float> equation){
        go.transform.SetParent(created);
        var obj = new Obj();
        obj.name = name;
        obj.go = go;
        foreach (var vertex in vertices) Objs[vertex].vertexof.Add(go.name);
        obj.vertices = vertices;
        obj.equation = equation;
        obj.children = new List<string>();
        obj.type = "sphere";
        Objs.Add(go.name, obj);
    }
    public void Remove(string ID){
        Destroy(Objs[ID].go);
        Objs.Remove(ID);
    }
    public void RemoveCurrentObjects(){
        foreach (Transform child in current){
            if (Objs.ContainsKey(child.name)){
                if (Objs[child.name].parent != "") Objs[Objs[child.name].parent].children.Remove(child.name);
                Objs.Remove(child.name);
            }
            Destroy(child.gameObject);
        }
    }
    public void FinishedCurrentObjects(){
        var list = new List<Transform>();
        foreach (Transform child in current){
            list.Add(child);
        }
        foreach (Transform child in list){
            child.SetParent(created);
        }
    }
    public void ResetAll(){
        RemoveCurrentObjects();
        foreach (var obj in Objs){
            Destroy(obj.Value.go);
        }
        Objs.Clear();
        draw.cam.ResetCamera();
        draw.Refresh();
        draw.mouse.Selected.Clear();
        draw.mouse.OnSelectionsChange();
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
