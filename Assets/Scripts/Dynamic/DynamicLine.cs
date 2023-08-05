using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicLine : MonoBehaviour
{
    LineRenderer line;
    LineCollider linecollider;
    Draw draw;
    Vector3[] prev = new Vector3[2], v = new Vector3[2];
    string preName;
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        draw = FindObjectOfType<Draw>();
        linecollider = FindObjectOfType<LineCollider>();
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));
        line.positionCount = 2;
        linecollider.AddCollider(line);
        
        while (true){
            var obj = Hierarchy.Objs[this.name];
            bool diff = false;
            for (int i=0;i<2;i++){
                v[i] = Hierarchy.Objs[obj.vertices[i]].go.transform.position;
                diff = diff || (prev[i] != v[i]);
            }
            if (preName != obj.name){
                preName = obj.name;
                draw.hier.ItemLabeling(this.name);
            }
            if (diff){
                foreach (string child in obj.children){
                    var pos = Hierarchy.Objs[child].go.transform.position;
                    float dir = Vector3.Dot(Vector3.Normalize(prev[1] - prev[0]), Vector3.Normalize(pos - prev[0]));
                    Vector3 result = new Vector3();
                    if (0.9f <= dir && dir <= 1.1f){
                        var ratio = Vector3.Distance(prev[0], pos) / Vector3.Distance(prev[0], prev[1]);
                        result = draw.calc.kc_sang_toa_do(v[0], v[1], Vector3.Distance(v[0], v[1]) * ratio);
                    }
                    else if (-1.1f <= dir && dir <= -0.9f){
                        var ratio = Vector3.Distance(prev[1], pos) / Vector3.Distance(prev[0], prev[1]);
                        result = draw.calc.kc_sang_toa_do(v[1], v[0], Vector3.Distance(v[0], v[1]) * ratio);
                    }
                    Hierarchy.Objs[child].go.transform.position = result;
                }
                for (int i=0;i<2;i++){
                    prev[i] = v[i];
                    line.SetPosition(i, v[i]);
                }
            }
            linecollider.RebuildCollider(line);
            line.startWidth = Vector3.Distance(Camera.main.transform.position, v[0])*draw.line.ratio;
            line.endWidth = Vector3.Distance(Camera.main.transform.position, v[1])*draw.line.ratio;
            yield return null;
        }
    }
}
