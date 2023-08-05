using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class Hierarchy : MonoBehaviour
{
    Draw draw;
    public struct Obj{
        public string name, type;
        public GameObject go;
        public List<string> parents, children, vertices, vertexof;
        public Dictionary<string, float> equation;
        public Matrix<double> rotation;
        public bool expand, movable;
    }
    public struct Item{
        public Button button;
        public TextMeshProUGUI name;
    }
    public static Dictionary<string, Obj> Objs = new Dictionary<string, Obj>();
    public static Dictionary<string, Item> Items = new Dictionary<string, Item>();
    public static Dictionary<string, string> TypeTranslate = new Dictionary<string, string>(){
        {"point", "Điểm"},
        {"line", "Đoạn thẳng"},
        {"plane", "Mặt phẳng"},
        {"circle", "Đường tròn"},
        {"polygon", "Đa giác đều"},
        {"sphere", "Hình cầu"},
    };
    public Transform created, current, content;
    public PhotonView photon;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void AddChildren(string ID, List<string> parents){
        foreach (var parent in parents){
            Objs[parent].children.Add(ID);
        }
    }
    public void AddVertices(string ID, List<string> vertices){
        foreach (var vertex in vertices){
            Hierarchy.Objs[vertex].go.transform.Find("dot").GetComponent<MeshRenderer>().material = (Hierarchy.Objs[vertex].movable ? draw.mouse.vertex[0] : draw.mouse.default_[0]);
            Objs[vertex].vertexof.Add(ID);
        }
    }
    public void AddItem(string ID){
        var item = new Item();
        item.button = draw.uiobj.HierarchyItem(ID, content);
        item.button.name = ID;
        item.name = item.button.transform.Find("name").GetComponent<TextMeshProUGUI>();
        Items.Add(ID, item);
        item.button.onClick.AddListener(delegate {
            var type = Objs[item.button.name].type;
            var obj = Objs[item.button.name].go.transform;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
                if (!draw.mouse.Selected[type].Contains(obj)){
                    if (draw.mouse.Selected[type].Count==0){
                        draw.mouse.OnSelect(obj);
                        foreach (var i in Items) ToggleItem(i.Key, 0);
                    }
                    else{
                        draw.mouse.Select(obj);
                        draw.Refresh();
                    }
                    draw.hier.ToggleItem(item.button.name, 2);
                }
                else{
                    draw.mouse.Unselect(obj);
                    ToggleItem(item.button.name, 0);
                    if (draw.mouse.Selected[type].Count==1){
                        draw.Refresh();
                        draw.mouse.OnSelect(draw.mouse.Selected[type][0]);
                        ToggleItem(draw.mouse.Selected[type][0].name, 2);
                    }
                }
            }
            else{
                if (!draw.mouse.Selected[type].Contains(obj)){
                    draw.mouse.UnselectAll();
                    draw.Refresh();
                    draw.mouse.OnSelect(obj);
                    foreach (var i in Items) ToggleItem(i.Key, 0);
                    ToggleItem(item.button.name, 2);
                }
            }
        });
        ItemLabeling(ID);
    }
    public void ToggleItem(string ID, int state){
        var colors = Items[ID].button.colors;
        switch (state){
            case 0:
                colors.normalColor = new Color32(50, 50, 50, 255);
                colors.highlightedColor = new Color32(100, 100, 100, 255);
                colors.pressedColor = new Color32(0, 64, 128, 255);
                break;
            case 1:
                colors.normalColor = colors.highlightedColor = colors.pressedColor = new Color32(100, 100, 100, 255);
                break;
            case 2:
                colors.normalColor = colors.highlightedColor = colors.pressedColor = new Color32(0, 128, 255, 255);
                break;
        }
        Items[ID].button.colors = colors;
    }
    public void ItemLabeling(string ID){
        var obj = Objs[ID];
        Items[ID].name.text = $"• {TypeTranslate[obj.type]}: {obj.name}";
        string parent = "";
        if (obj.parents != null && obj.parents.Count != 0){
            parent = $"| <size=20>thuộc {TypeTranslate[Objs[obj.parents[0]].type]}: {Objs[obj.parents[0]].name}</size>";
        }
        if (obj.type == "point"){
            Items[ID].name.text += $" <size=20>{draw.calc.roundVec3(draw.calc.ztoy(obj.go.transform.position), 2)}</size>" + (parent != "" ? $" {parent}" : "");
        }
        else{
            Items[ID].name.text += (parent != "" ? $" {parent}" : "");
        }

        if (obj.children != null && obj.children.Count != 0){
            foreach (var child in obj.children){
                ItemLabeling(child);
            }
        }
    }
    [PunRPC]
    public void RPC_AddPoint(string name, string id, string[] parents, bool movable, Vector3 position){
        var go = draw.obj.Point(position, created);
        go.name = id;
        AddPoint(name, go, new List<string>(parents), movable, false);
    }
    [PunRPC]
    public void RPC_AddLine(string name, string id, string[] vertices){
        var go = draw.obj.Line(created, false);
        go.GetComponent<LineRenderer>().positionCount = 2;
        go.name = id;
        AddLine(name, go, new List<string>(vertices), false);
    }
    [PunRPC]
    public void RPC_AddPlane(string name, string id, string[] vertices, bool expand){
        var go = draw.obj.Plane(created);
        go.name = id;
        AddPlane(name, go, new List<string>(vertices), expand, false);
    }
    [PunRPC]
    public void RPC_AddCircle(string name, string id, string[] parents, string[] vertices){
        var go = draw.obj.Line(created, true);
        go.name = id;
        go.GetComponent<LineRenderer>().positionCount = 180;
        if (parents == null){
            AddCircle(name, go, new List<string>(vertices), false);
        }
        else{
            AddCircle(name, go, new List<string>(parents), new List<string>(vertices), false);
        }
    }
    [PunRPC]
    public void RPC_AddPolygon(string name, string id, string[] parents, string[] vertices){
        var go = draw.obj.Line(created, true);
        go.GetComponent<LineRenderer>().positionCount = vertices.Length-1;
        go.name = id;
        AddPolygon(name, go, new List<string>(parents), new List<string>(vertices), false);
    }
    [PunRPC]
    public void RPC_AddSphere(string name, string id, string[] vertices, Vector3 position){
        var go = draw.obj.Sphere(position, created);
        go.name = id;
        AddSphere(name, go, new List<string>(vertices), false);
    }

    public void AddPoint(string name, GameObject go, List<string> parents, bool movable, bool rpc){
        go.GetComponent<SphereCollider>().enabled = true;
        go.AddComponent<DynamicPoint>();
        var obj = new Obj();
        obj.name = name;
        obj.type = "point";
        obj.go = go;

        obj.parents = parents;
        AddChildren(go.name, parents);
        obj.vertexof = new List<string>();
        obj.movable = movable;
        go.transform.Find("dot").GetComponent<MeshRenderer>().material = (movable ? draw.mouse.movable_point[0] : draw.mouse.default_[0]);
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddPoint", RpcTarget.OthersBuffered, name, go.name, parents.ToArray(), movable, go.transform.position);
        }
    }
    public void AddLine(string name, GameObject go, List<string> vertices, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicLine>();
        var obj = new Obj();
        obj.name = name;
        obj.type = "line";
        obj.go = go;

        obj.parents = draw.getPlaneFromVertices(vertices);
        AddChildren(go.name, obj.parents);
        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        obj.children = new List<string>();
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddLine", RpcTarget.OthersBuffered, name, go.name, vertices.ToArray());
        }
    }
    public void AddPlane(string name, GameObject go, List<string> vertices, bool expand, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicPlane>();
        go.GetComponent<MeshCollider>().enabled = true;
        var obj = new Obj();
        obj.name = name;
        obj.type = "plane";
        obj.go = go;

        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        var v = new Vector3[3];
        for (int i=0;i<3;i++) v[i] = draw.calc.ztoy(Hierarchy.Objs[vertices[i]].go.transform.position);
        obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
        obj.equation = draw.calc.pt_mp(v[0], v[1], v[2]);
        obj.parents = new List<string>();
        obj.children = new List<string>();
        foreach (var o in Objs){
            if (o.Value.type == "line"){
                if (vertices.Contains(o.Value.vertices[0]) && vertices.Contains(o.Value.vertices[1])){
                    obj.children.Add(o.Key);
                    obj.children.AddRange(o.Value.children);
                    o.Value.parents.Add(go.name);
                    foreach (var child in o.Value.children){
                        Objs[child].parents.Add(go.name);
                    }
                }
            }
        }
        obj.expand = expand;
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddPlane", RpcTarget.OthersBuffered, name, go.name, vertices.ToArray(), expand);
        }
    }
    public void AddCircle(string name, GameObject go, List<string> vertices, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicCircle>();
        var obj = new Obj();
        obj.name = name;
        obj.type = "circle";
        obj.go = go;

        obj.parents = draw.getPlaneFromVertices(vertices);
        AddChildren(go.name, obj.parents);
        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        var v = new Vector3[3];
        for (int i=0;i<3;i++) v[i] = draw.calc.ztoy(Hierarchy.Objs[vertices[i]].go.transform.position);
        obj.rotation = draw.calc.rm_plane_xy(v[0], v[1], v[2]);
        obj.children = new List<string>();
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddCircle", RpcTarget.OthersBuffered, name, go.name, null, vertices.ToArray());
        }
    }
    public void AddCircle(string name, GameObject go, List<string> parents, List<string> vertices, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicCircle>();
        var obj = new Obj();
        obj.name = name;
        obj.type = "circle";
        obj.go = go;

        obj.parents = parents;
        AddChildren(go.name, parents);
        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        obj.children = new List<string>();
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddCircle", RpcTarget.OthersBuffered, name, go.name, parents.ToArray(), vertices.ToArray());
        }
    }
    public void AddPolygon(string name, GameObject go, List<string> parents, List<string> vertices, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicPolygon>();
        var obj = new Obj();
        obj.name = name;
        obj.type = "polygon";
        obj.go = go;

        obj.parents = parents;
        AddChildren(go.name, parents);
        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        obj.children = new List<string>();
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddPolygon", RpcTarget.OthersBuffered, name, go.name, parents.ToArray(), vertices.ToArray());
        }
    }
    public void AddSphere(string name, GameObject go, List<string> vertices, bool rpc){
        go.transform.SetParent(created);
        go.AddComponent<DynamicSphere>();
        go.GetComponent<SphereCollider>().enabled = true;
        var obj = new Obj();
        obj.name = name;
        obj.type = "sphere";
        obj.go = go;

        obj.vertices = vertices;
        AddVertices(go.name, vertices);
        obj.parents = new List<string>();
        obj.children = new List<string>();
        Objs.Add(go.name, obj);
        AddItem(go.name);

        if (rpc){
            photon.RPC("RPC_AddSphere", RpcTarget.OthersBuffered, name, go.name, vertices.ToArray(), go.transform.position);
        }
    }
    [PunRPC]
    public void Remove(string ID){
        var obj = Hierarchy.Objs[ID];
        if (obj.parents != null){
            foreach (var parent in obj.parents){
                Objs[parent].children.Remove(ID);
            }
        }
        if (obj.children != null){
            foreach (var child in obj.children){
                Objs[child].parents.Remove(ID);
            }
        }
        if (obj.vertices != null){
            foreach (var vertex in obj.vertices){
                Objs[vertex].vertexof.Remove(ID);
            }
        }
        if (obj.vertexof != null){
            foreach (var v in obj.vertexof){
                Objs[v].vertices.Remove(ID);
            }
        }
        Destroy(Objs[ID].go);
        Destroy(Items[ID].button.gameObject);
        Items.Remove(ID);
        Objs.Remove(ID);
    }
    public void RemoveCurrentObjects(){
        var list = new List<Transform>();
        foreach (Transform child in current){
            list.Add(child);
        }
        foreach (Transform child in list){
            if (Objs.ContainsKey(child.name)){
                if (draw.inRoom()){
                    photon.RPC("Remove", RpcTarget.AllBuffered, child.name);
                }
                else{
                    Remove(child.name);
                }
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
    [PunRPC]
    public void ResetAll(){
        RemoveCurrentObjects();
        foreach (var obj in Objs){
            Destroy(obj.Value.go);
            Destroy(Items[obj.Key].button.gameObject);
        }
        Items.Clear();
        Objs.Clear();
        draw.cam.ResetCamera();
        draw.Refresh();
        foreach (var s in draw.mouse.Selected) s.Value.Clear();
        draw.mouse.OnSelectionsChange();
        ActionManager.UndoActions.Clear();
        ActionManager.RedoActions.Clear();
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
