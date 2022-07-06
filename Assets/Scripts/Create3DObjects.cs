using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create3DObjects : MonoBehaviour
{
    IDHandler idhandler;
    public GameObject pointPref, linePref, planePref;
    Quaternion rot0 = Quaternion.identity;
    Vector3 pos0 = Vector3.zero;
    void Start(){
        idhandler = FindObjectOfType<IDHandler>();
    }
    public GameObject Point(Vector3 position, Transform parent){
        var point = Instantiate(pointPref, position, rot0, parent);
        point.name = idhandler.GenID();
        return point;
    }
    public LineRenderer Line(Transform parent, bool loop){
        var line = Instantiate(linePref, pos0, rot0, parent).GetComponent<LineRenderer>();
        line.name = idhandler.GenID();
        line.loop = loop;
        return line;
    }
    public GameObject Plane(Transform parent){
        var plane = Instantiate(planePref, pos0, rot0, parent);
        plane.name = idhandler.GenID();
        return plane;
    }
}
