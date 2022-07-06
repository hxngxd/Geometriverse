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
    // public drawLine line;
    // public drawPlane plane;
    // public draw3PointCircle circle3;
    // public drawPolygon polygon;
    public CameraController cam;
    public Coroutine current = null;
    public ScrollRect scroll;
    public void Point(){
        Refresh();
        StartDrawing(point.content, point.Okay());
    }
    // public void Line(){
    //     Refresh();
    //     SetScrollContent(line.content);
    //     current = StartCoroutine(line.Okay());
    // }
    // public void Plane(){
    //     Refresh();
    //     SetScrollContent(plane.content);
    //     current = StartCoroutine(plane.Okay());
    // }
    // public void CameraControl(){
    //     Refresh();
    //     SetScrollContent(cam.content);
    //     current = StartCoroutine(cam.Okay());
    // }
    // public void Circle3Point(){
    //     Refresh();
    //     SetScrollContent(circle3.content);
    //     current = StartCoroutine(circle3.Okay());
    // }
    // public void Polygon(bool type){
    //     Refresh();
    //     SetScrollContent(polygon.content);
    //     current = StartCoroutine(polygon.Okay(type));
    // }
    public void StartDrawing(Transform content, IEnumerator Okay){
        scroll.content = content.GetComponent<RectTransform>();
        current = StartCoroutine(Okay);
    }
    public void Refresh(){
        Cancel();
        point.Cancel();
        // line.Cancel();
        // plane.Cancel();
        // circle3.Cancel();
        // polygon.Cancel();
    }
    public void Cancel(){
        if (current != null){
            StopAllCoroutines();
            hier.RemoveCurrentObjects();
            current = null;
        }
    }
}
