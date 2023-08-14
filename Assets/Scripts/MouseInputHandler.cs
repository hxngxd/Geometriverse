using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour
{
    public float doubleClickTimeThreshold; // Maximum time between clicks to count as a double click
    public float holdTimeThreshold; // Minimum time to hold for a long press
    public float singleClickDelay; // Delay after click before confirming it as a single click

    private float lastClickTime;
    private bool isPointerDown;
    private bool hasDetectedDoubleClick;
    private bool hasDetectedHold;

    public delegate void MouseFunc();
    public Dictionary<int, MouseFunc[]> MouseFunctions = new Dictionary<int, MouseFunc[]>(){
        {0, new MouseFunc[3]},
        {1, new MouseFunc[3]},
        {2, new MouseFunc[3]}
    };
    void Update()
    {
        PressType(0);
    }
    void PressType(int id){
        if (Input.GetMouseButtonDown(id))
        {
            if (Time.time - lastClickTime < doubleClickTimeThreshold)
            {
                hasDetectedDoubleClick = true;
                hasDetectedHold = false;
                //DOUBLE CLICK
                ExecuteFunc(id, 1);
            }
            else
            {
                hasDetectedDoubleClick = false;
                hasDetectedHold = false;
                StartCoroutine(SingleClickDelay(id));
            }

            lastClickTime = Time.time;
            isPointerDown = true;
        }

        if (Input.GetMouseButtonUp(id))
        {
            isPointerDown = false;
            StopCoroutine(SingleClickDelay(id));

            if (Time.time - lastClickTime > holdTimeThreshold && !hasDetectedDoubleClick)
            {
                hasDetectedHold = true;
                //HOLD
                ExecuteFunc(id, 2);
            }
        }
    }

    IEnumerator SingleClickDelay(int id)
    {
        yield return new WaitForSeconds(singleClickDelay);

        if (!isPointerDown)
        {
            if (!hasDetectedDoubleClick && !hasDetectedHold)
            {
                //SINGLE CLICK
                ExecuteFunc(id, 0);
            }
        }
    }
    void ExecuteFunc(int id, int press_type){
        if (MouseFunctions[id][press_type] != null){
            MouseFunctions[id][press_type]();
        }
    }
}
