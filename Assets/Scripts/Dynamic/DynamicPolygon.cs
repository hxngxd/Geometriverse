using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class DynamicPolygon : MonoBehaviour
{
    LineRenderer line;
    LineCollider linecollider;
    Draw draw;
    Vector3[] prev = new Vector3[2], v = new Vector3[2];
    public Dictionary<string, KeyValuePair<int, int>> ChildrenOnSide = new Dictionary<string, KeyValuePair<int, int>>();
    string preName;
    int preNOC, NOC;
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    public void COS(Hierarchy.Obj obj){
        ChildrenOnSide.Clear();
        foreach (var child in obj.children){
            if (Hierarchy.Objs[child].type == "point"){
                var point = Hierarchy.Objs[child].go;
                var pos = point.transform.position;
                for (int i=0;i<line.positionCount;i++){
                    int startid = (i==0 ? line.positionCount-1 : i-1), endid = i;
                    Vector3 start = line.GetPosition(startid), end = line.GetPosition(endid);
                    if (Mathf.Abs(Vector3.Distance(start, pos) + Vector3.Distance(end, pos) - Vector3.Distance(start, end)) <= Vector3.Distance(start, end) / 500f){
                        ChildrenOnSide.Add(child, new KeyValuePair<int, int>(startid, endid));
                        break;
                    }
                }
            }
        }
    }
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));
        var obj = Hierarchy.Objs[this.name];
        preNOC = NOC = obj.children.Count;
        linecollider.AddCollider(line);
        
        while (true){
            obj = Hierarchy.Objs[this.name];
            NOC = obj.children.Count;
            bool diff = false;
            for (int i=0;i<2;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
                draw.hier.ItemLabeling(this.name);
            }
            if (preNOC != NOC){
                COS(obj);
                preNOC  = NOC;
            }
            if (diff){
                var center = draw.calc.ztoy(v[0]);
                var vertex = draw.calc.ztoy(v[1]);
                var plane_rot = Hierarchy.Objs[obj.parents[0]].rotation;
                var positions = draw.calc.dinh_da_giac(center, vertex, 1, plane_rot);
                var rotation = draw.calc.rm_vectors(center, draw.calc.ztoy(positions[0]), vertex);
                positions = draw.calc.dinh_da_giac(center, vertex, line.positionCount, rotation.Multiply(plane_rot));
                foreach (var child in obj.children){
                    if (Hierarchy.Objs[child].type == "point"){
                        var point = Hierarchy.Objs[child].go;
                        var pos = point.transform.position;
                        var startid = ChildrenOnSide[child].Key;
                        var endid = ChildrenOnSide[child].Value;
                        var ratio = Vector3.Distance(line.GetPosition(startid), pos) / Vector3.Distance(line.GetPosition(startid), line.GetPosition(endid));
                        var cur_dist = Vector3.Distance(positions[startid], positions[endid]);
                        point.transform.position = draw.calc.kc_sang_toa_do(positions[startid], positions[endid], cur_dist * ratio);
                    }
                }
                line.SetPosition(0, v[1]);
                for (int i=2;i<obj.vertices.Count;i++){
                    var point = Hierarchy.Objs[obj.vertices[i]].go;
                    point.transform.position = positions[i-1];
                    line.SetPosition(i-1, positions[i-1]);
                }
                prev[1] = v[1];
                prev[0] = v[0];
            }
            Hierarchy.Objs[this.name] = obj;
            linecollider.RebuildCollider(line);   
            yield return null;
        }
    }
}
