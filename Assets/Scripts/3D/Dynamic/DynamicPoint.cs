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

    void Update()
    {
        if (Hierarchy.Objs.ContainsKey(this.name)){
            var obj = Hierarchy.Objs[this.name];
            var pos = this.transform.position;
            if (prePosition != pos || preName != obj.name){
                prePosition = pos;
                preName = obj.name;
                value.text = preName;
            }
        }
    }
}
