using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicPoint : MonoBehaviour
{
    Draw draw;
    TextMeshPro value;
    Transform hierItem = null;
    string preName;
    Vector3 prePosition;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        value = this.transform.Find("value").GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (hierItem == null) hierItem = draw.hier.content.Find(this.name);
        else{
            if (Hierarchy.Points.ContainsKey(this.name)){
                var p = Hierarchy.Points[this.name];
                var pos = this.transform.position;
                if (prePosition != pos || preName != p.name){
                    prePosition = pos;
                    preName = p.name;
                    value.text = p.name;

                    var roundedPos = draw.calc.roundVec3(prePosition, 2);
                    string s = $"Điểm: {p.name}({roundedPos.x},{roundedPos.z},{roundedPos.y})";
                    if (p.parent != ""){
                        switch (Hierarchy.Types[p.parent]){
                            case "line":
                                s += $"\n(Thuộc đoạn thẳng: {Hierarchy.Lines[p.parent].name})";
                                break;
                            case "plane":
                                s += $"\n(Thuộc mặt phẳng: {Hierarchy.Planes[p.parent].name})";
                                break;
                            case "3pointcircle":
                                s += $"\n(Thuộc đường tròn: {Hierarchy.Circles[p.parent].name})";
                                break;
                        }
                    }
                    hierItem.Find("Text").GetComponent<TextMeshProUGUI>().text = s;
                }
            }
        }
    }
}
