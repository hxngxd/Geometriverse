using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOnAxisLine : MonoBehaviour
{
    Create3DObjects obj;
    public Transform Xs, Ys, Zs;
    public float Step, range, maxSize;
    public List<float> requireSize;
    void Start()
    {
        obj = FindObjectOfType<Create3DObjects>();
        Create();
    }
    void Create(){
        for (float i=-range;i<=range;i+=Step){
            var x = obj.Point(new Vector3(i,0,0), Xs);
            x.AddComponent<DynamicAxisPoint>();
            x.name = $"pointX_{i}";
            x.GetComponent<SphereCollider>().enabled = true;
            var y = obj.Point(new Vector3(0,0,i), Ys);
            y.AddComponent<DynamicAxisPoint>();
            y.name = $"pointY_{i}";
            y.GetComponent<SphereCollider>().enabled = true;
            var z = obj.Point(new Vector3(0,i,0), Zs);
            z.AddComponent<DynamicAxisPoint>();
            z.name = $"pointZ_{i}";
            z.GetComponent<SphereCollider>().enabled = true;
            z.transform.Find("value").transform.localPosition = new Vector3(-0.12f, 0, 0);
        }
    }
    void Update()
    {
        var CamPosition = Camera.main.transform.position;
        var x = Xs.position;
        x.x = (int)CamPosition.x;
        Xs.position = x;

        var y = Ys.position;
        y.z = (int)CamPosition.z;
        Ys.position = y;

        var z = Zs.position;
        z.y = (int)CamPosition.y;
        Zs.position = z;
    }
}
