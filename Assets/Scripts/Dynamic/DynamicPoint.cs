using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicPoint : MonoBehaviour
{
    Draw draw;
    TextMeshPro value;
    string preName;
    Vector3 prePosition;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        value = this.transform.Find("value").GetComponent<TextMeshPro>();
    }
    IEnumerator Dynamic(){
        yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));

        while (true){
            var obj = Hierarchy.Objs[this.name];
            var pos = this.transform.position;
            if (prePosition != pos || preName != obj.name){
                prePosition = pos;
                preName = obj.name;
                value.text = preName;
                draw.hier.ItemLabeling(this.name);
            }
            yield return null;
        }
    }
    void OnEnable(){StartCoroutine(Dynamic());}
    void OnDisable(){StopAllCoroutines();}
    // IEnumerator Dynamic(){
    //     yield return new WaitUntil(() => Hierarchy.Objs.ContainsKey(this.name));

    //     while (true){
            
    //         yield return null;
    //     }
    // }
}
