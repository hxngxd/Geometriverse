using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Draw : MonoBehaviour
{
    public Hierarchy hier;
    public Create3DObjects obj;
    public CreateUIObjects uiobj;
    public RaycastHandler raycast;
    public MouseHandler mouse;
    public InputFieldHandler input;
    public MenuManager menu;
    public PanelManager panel;
    public Listener listener;
    public Calculate calc;
    public drawPoint point;
    public drawLine line;
    public drawPlane plane;
    public drawCircle circle;
    public CameraController cam;
    public bool isDrawing = false;
    public ScrollRect scroll;
    public void Point(){
        Refresh();
        StartDrawing(point.content, point.Okay());
    }
    public void Line(){
        Refresh();
        StartDrawing(line.content, line.Okay());
    }
    public void Plane(){
        Refresh();
        StartDrawing(plane.content, plane.Okay());
    }
    public void Circle(){
        Refresh();
        StartDrawing(circle.content, circle.Okay());
    }
    public void StartDrawing(Transform content, IEnumerator Okay){
        isDrawing = true;
        scroll.content = content.GetComponent<RectTransform>();
        StartCoroutine(Okay);
    }
    public void Refresh(){
        Cancel();
        point.Cancel();
        line.Cancel();
        plane.Cancel();
        circle.Cancel();
    }
    public void Cancel(){
        StopAllCoroutines();
        hier.RemoveCurrentObjects();
        isDrawing = false;
    }
}
