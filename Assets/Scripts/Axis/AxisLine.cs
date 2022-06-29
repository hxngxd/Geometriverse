using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLine : MonoBehaviour
{
    LineRenderer line;
    Calculate calc;
    string axis;
    public float ratio;
    void Start()
    {
        axis = this.name;
        line = this.GetComponent<LineRenderer>();
        calc = this.GetComponent<Calculate>();
    }

    void Update()
    {
        var CamPosition = Camera.main.transform.position;
        var currentPosition = this.transform.position;
        float dist = 0;
        switch (axis){
            case "X":
                currentPosition.x = CamPosition.x;
                dist = Vector3.Distance(CamPosition, new Vector3(CamPosition.x, 0, 0));
                break;
            case "Y":
                currentPosition.z = CamPosition.z;
                dist = Vector3.Distance(CamPosition, new Vector3(0, 0, CamPosition.z));
                break;
            case "Z":
                currentPosition.y = CamPosition.y;
                dist = Vector3.Distance(CamPosition, new Vector3(0, CamPosition.y, 0));
                break;
        }
        this.transform.position = currentPosition;
        line.startWidth = line.endWidth = dist * ratio;
    }
}
