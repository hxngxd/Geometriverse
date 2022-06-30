using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Draw : MonoBehaviour
{
    public IDHandler idhandler;
    public Transform hierContent;
    public Hierarchy hier;
    public Create3DObjects obj;
    public CreateUIObjects uiobj;
    public RaycastHandler raycast;
    public MouseHandler mouse;
    public InputFieldHandler inputhandler;
    public MenuManager menu;
    public PanelManager panel;
    public Listener listener;
    public Calculate calc;
    public drawPoint point;
    public drawLine line;
    public CameraController cam;
    public bool drawing = false;
    public Coroutine current = null;
    public ScrollRect InspectorSR;
    public void Point(){
        Refresh();
        SetScrollContent(point.content);
        current = StartCoroutine(point.Okay());
    }
    public void Line(){
        Refresh();
        SetScrollContent(line.content);
        current = StartCoroutine(line.Okay());
    }
    public void CameraControl(){
        Refresh();
        SetScrollContent(cam.content);
        current = StartCoroutine(cam.Okay());
    }
    public void SetScrollContent(Transform content){
        InspectorSR.content = content.GetComponent<RectTransform>();
    }
    public void Refresh(){
        drawing = false;
        point.Cancel();
        line.Cancel();
        cam.Cancel();
        current = null;
    }
    public void Cancel(){
        if (current != null){
            StopCoroutine(current);
            hier.RemoveCurrentObjects();
        }
        drawing = false;
    }
}
