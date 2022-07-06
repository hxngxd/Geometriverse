using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLine : MonoBehaviour
{
    LineRenderer line;
    string axis;
    public float ratio, collider_ratio;
    BoxCollider axislinecollider;
    void Start()
    {
        axis = this.name;
        line = this.GetComponent<LineRenderer>();
        axislinecollider = this.GetComponent<BoxCollider>();
    }

    void Update()
    {
        var CamPosition = Camera.main.transform.position;
        var currentPosition = this.transform.position;
        float dist = 0;
        switch (axis){
            case "Xline":
                currentPosition.x = CamPosition.x;
                dist = Vector3.Distance(CamPosition, new Vector3(CamPosition.x, 0, 0));
                axislinecollider.size = new Vector3(axislinecollider.size.x, dist * collider_ratio, dist * collider_ratio);
                break;
            case "Yline":
                currentPosition.z = CamPosition.z;
                dist = Vector3.Distance(CamPosition, new Vector3(0, 0, CamPosition.z));
                axislinecollider.size = new Vector3(dist * collider_ratio, dist * collider_ratio, axislinecollider.size.z);
                break;
            case "Zline":
                currentPosition.y = CamPosition.y;
                dist = Vector3.Distance(CamPosition, new Vector3(0, CamPosition.y, 0));
                axislinecollider.size = new Vector3(dist * collider_ratio, axislinecollider.size.y, dist * collider_ratio);
                break;
        }
        this.transform.position = currentPosition;
        line.startWidth = line.endWidth = dist * ratio;
        axislinecollider.enabled = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }
}
