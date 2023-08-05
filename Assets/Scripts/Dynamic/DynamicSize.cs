using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSize : MonoBehaviour
{
    public float ratio;
    void Update()
    {
        var d = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        var s = d*ratio;
        this.transform.localScale = new Vector3(s,s,s);
    }
}