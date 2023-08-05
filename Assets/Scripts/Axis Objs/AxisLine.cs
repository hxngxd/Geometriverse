using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLine : MonoBehaviour
{
    public LineRenderer line;
    public BoxCollider collider_;
    public float ratio, collider_ratio;

    void Update()
    {
        var CamPosition = Camera.main.transform.position;
        var currentPosition = this.transform.position;
        var cldsize = collider_.size;
        float dist = 0;
        switch (this.name[0]){
            case 'X':
                currentPosition.x = CamPosition.x;
                dist = Vector3.Distance(CamPosition, new Vector3(CamPosition.x, 0, 0));
                collider_.size = new Vector3(collider_.size.x, dist * collider_ratio, dist * collider_ratio);
                break;
            case 'Y':
                currentPosition.z = CamPosition.z;
                dist = Vector3.Distance(CamPosition, new Vector3(0, 0, CamPosition.z));
                collider_.size = new Vector3(dist * collider_ratio, dist * collider_ratio, collider_.size.z);
                break;
            case 'Z':
                currentPosition.y = CamPosition.y;
                dist = Vector3.Distance(CamPosition, new Vector3(0, CamPosition.y, 0));
                collider_.size = new Vector3(dist * collider_ratio, collider_.size.y, dist * collider_ratio);
                break;
        }
        this.transform.position = currentPosition;
        line.startWidth = line.endWidth = dist * ratio;
        collider_.enabled = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }
}
