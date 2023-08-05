using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    MouseHandler mouse;
    bool hover = false;
    void Start(){
        mouse = FindObjectOfType<MouseHandler>();
    }
    public void OnPointerEnter(PointerEventData eventData){
        hover = true;
    }
 
    public void OnPointerExit(PointerEventData eventData){ 
        hover = false;
    }
    void Update(){
        if (!hover) return;
        var obj = Hierarchy.Objs[this.name];
        var transform = obj.go.transform;
        if (Hierarchy.Objs.ContainsKey(this.name) && !mouse.Selected[Hierarchy.Objs[this.name].type].Contains(transform)) mouse.Highlight(transform);
    }
}
