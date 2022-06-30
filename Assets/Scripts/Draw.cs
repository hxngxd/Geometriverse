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
    public Listener listener;
    public Calculate calc;
    public drawPoint point;
    public drawLine line;
    public CameraController cam;
    public bool drawing = false;
    public Coroutine current = null;
    public void Point(){
        Refresh();
        current = StartCoroutine(point.Okay());
    }
    public void Line(){
        Refresh();
        current = StartCoroutine(line.Okay());
    }
    public void CameraControl(){
        Refresh();
        current = StartCoroutine(cam.Okay());
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
