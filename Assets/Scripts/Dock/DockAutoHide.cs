using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockAutoHide : MonoBehaviour
{
    RaycastHandler raycast;
    public bool notHiden, autohide;
    public RectTransform dockCanvas;
    void Start()
    {
        raycast = FindObjectOfType<RaycastHandler>();
        if (autohide) ToggleDock(false);
    }

    void Update()
    {
        if (autohide) AutoHide();
        else{
            if (!notHiden) ToggleDock(true);
        }
    }
    public void AutoHide(){
        var hit = raycast.GetUI(dockCanvas);
        if (hit!=null && hit.IsChildOf(this.transform) && !notHiden) ToggleDock(true);
        else if (notHiden){
            if (hit==null || !hit.IsChildOf(this.transform)) ToggleDock(false);
        }
    }
    public void ToggleDock(bool toggle){
        var y = (toggle ? 0 : -85f);
        LeanTween.moveY(this.GetComponent<RectTransform>(), y, toggle ? 0.5f : 0.2f).setEase(toggle ? LeanTweenType.easeOutExpo : LeanTweenType.linear);
        notHiden = toggle;
    }
}
