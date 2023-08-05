using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Expand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Draw draw;
    public RectTransform container = null;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public void OnPointerEnter(PointerEventData eventData){
        if (!this.name.Contains('/')){
            if (draw.menu.Containers.childCount >= 1 && container == null){
                foreach (Transform child in draw.menu.Containers) Destroy(child.gameObject);
                container = draw.menu.CreateChildCommand(this.GetComponent<RectTransform>());
            }
        }
        else{
            if (container == null) container = draw.menu.CreateChildCommand(this.GetComponent<RectTransform>());
            else{
                foreach (Transform child in draw.menu.Containers){
                    if (child.name.Contains(container.name) && child.name != container.name){
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData){ 
        if (this.name.Contains('/')) StartCoroutine(OnExit());
    }
    IEnumerator OnExit(){
        while (container != null){
            var ui = draw.raycast.GetUI(draw.menu.MenuCanvas);
            if (ui != null){
                if (MenuManager.Cmds.ContainsKey(ui.name) && ui.IsChildOf(this.transform.parent) && ui != this.transform.parent && ui != this.transform){
                    foreach (Transform child in draw.menu.Containers){
                        if (child.name.Contains(container.name)){
                            Destroy(child.gameObject);
                        }
                    }
                    break;
                }
            }
            yield return null;
        }
    }
}
