using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSphere : MonoBehaviour
{
    Draw draw;
    Vector3[] prev = new Vector3[2], v = new Vector3[2];
    string preName;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));

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
                for (int i=0;i<2;i++) prev[i] = v[i];
                this.transform.position = v[0];
                var r = Vector3.Distance(v[0], v[1])*2f;
                this.transform.localScale = new Vector3(r,r,r);
                foreach (var child in obj.children){
                    if (Hierarchy.Objs[child].type == "point"){
                        var point = Hierarchy.Objs[child].go;
                        point.transform.position = draw.calc.kc_sang_toa_do(v[0], point.transform.position, r/2f);
                    }
                }
            }
            yield return null;
        }
    }
}
