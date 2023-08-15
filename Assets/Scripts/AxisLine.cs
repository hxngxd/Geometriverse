using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLine : MonoBehaviour
{
    Transform cam;
    string axis;
    float distance;
    ScaleRatio refScaleRatio;
    void Start(){
        refScaleRatio = FindObjectOfType<ScaleRatio>();
        cam = Camera.main.transform;
        axis = this.name;
    }
    void Update()
    {
        Vector3 newPos = transform.position;
        switch (axis){
            case "x":
                newPos.x = cam.position.x;
                break;
            case "y":
                newPos.y = cam.position.y;
                break;
            case "z":
                newPos.z = cam.position.z;
                break;
        }
        transform.position = newPos;
        distance = Vector3.Distance(transform.position, cam.position);
        GetComponent<TubeRenderer>()._radiusOne = distance * refScaleRatio.axis_line;
        GetComponent<TubeRenderer>()._radiusTwo = -distance * refScaleRatio.axis_line;
    }
}
