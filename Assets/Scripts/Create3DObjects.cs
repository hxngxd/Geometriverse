using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create3DObjects : MonoBehaviour
{
    IDHandler idhandler;
    public GameObject pointPref;
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
}
