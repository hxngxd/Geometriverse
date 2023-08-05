using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create3DObjects : MonoBehaviour
{
    IDHandler idhandler;
    public GameObject pointPref, linePref, planePref, spherePref;
    Quaternion rot0 = Quaternion.identity;
    Vector3 pos0 = Vector3.zero;
    public int ID_len;
    void Start(){
        idhandler = FindObjectOfType<IDHandler>();
    }
    public GameObject Point(Vector3 position, Transform parent){
        var point = Instantiate(pointPref, position, rot0, parent);
        point.name = idhandler.GenID(ID_len);
        return point;
    }
    public GameObject Line(Transform parent, bool loop){
        var line = Instantiate(linePref, pos0, rot0, parent).GetComponent<LineRenderer>();
        line.name = idhandler.GenID(ID_len);
        line.loop = loop;
        return line.gameObject;
    }
    public GameObject Plane(Transform parent){
        var plane = Instantiate(planePref, pos0, rot0, parent);
        plane.name = idhandler.GenID(ID_len);
        return plane;
    }
    public GameObject Sphere(Vector3 position, Transform parent){
        var sphere = Instantiate(spherePref, position, rot0, parent);
        sphere.name = idhandler.GenID(ID_len);
        return sphere;
    }
}
