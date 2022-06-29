using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLine : MonoBehaviour
{
    LineRenderer line;
    string axis;
    void Start()
    {
        axis = this.name;
        line = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        var CamPosition = Camera.main.transform.position;
        var currentPosition = this.transform.position;
        switch (axis){
            case "X":
                currentPosition.x = CamPosition.x;
                break;
            case "Y":
                currentPosition.z = CamPosition.z;
                break;
            case "Z":
                currentPosition.y = CamPosition.y;
                break;
        }
        this.transform.position = currentPosition;
    }
}
