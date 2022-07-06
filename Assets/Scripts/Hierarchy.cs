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
        public string name, parent, type, vertexof;
        public GameObject go;
        public List<string> vertices, children;
        public Dictionary<string, float> equation;
        public Matrix<double> rotation;
    }
    public static Dictionary<string, Obj> Objs = new Dictionary<string, Obj>();
    public Transform created, current, selected, content;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void Add(string name, string parent, string vertexof, GameObject go){
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        obj.vertexof = vertexof;
        obj.go = go;
        obj.type = "point";
        Objs.Add(go.name, obj);
    }
    public void Add(string name, string parent, List<string> vertices, GameObject go){
        var obj = new Obj();
        obj.name = name;
        obj.vertices = vertices;
        obj.go = go;
        obj.children = new List<string>();
        obj.type = "line";
        Objs.Add(go.name, obj);
    }
    public void Add(string name, List<string> vertices, GameObject go, Matrix<double> rotation, Dictionary<string, float> equation){
        var obj = new Obj();
        obj.name = name;
        obj.vertices = vertices;
        obj.go = go;
        obj.rotation = rotation;
        obj.equation = equation;
        obj.type = "plane";
        Objs.Add(go.name, obj);
    }
    public void Add(string name, string parent, List<string> vertices, GameObject go, Matrix<double> rotation, Dictionary<string, float> equation){
        var obj = new Obj();
        obj.name = name;
        obj.parent = parent;
        obj.go = go;
        obj.vertices = vertices;
        obj.rotation = rotation;
        obj.equation = equation;
        obj.type = "circle";
        Objs.Add(go.name, obj);
    }
    public void Remove(string ID){
        Destroy(Objs[ID].go);
        Objs.Remove(ID);
        Destroy(content.Find(ID).gameObject);
    }
    public void RemoveCurrentObjects(){
        foreach (Transform child in current){
            Destroy(child.gameObject);
        }
    }
    public void ResetAll(){
        // RemoveCurrentObjs();
        // List<string> IDs = new List<string>();
        // foreach (var obj in Hierarchy.Types) IDs.Add(obj.Key);
        // foreach (string ID in IDs){
        //     RemoveObjsWithID(ID);
        // }
        // Types.Clear();
        // Points.Clear();
        // Lines.Clear();
        // draw.cam.ResetCamera();
        // draw.Refresh();
        // draw.mouse.Selected.Clear();
        // draw.mouse.SelectionCount.Clear();
        // foreach (string type in MouseHandler.Types) draw.mouse.SelectionCount.Add(type, 0);
        // draw.mouse.OnSelectionsChange();
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
