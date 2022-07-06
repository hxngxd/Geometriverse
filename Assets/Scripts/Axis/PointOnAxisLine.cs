using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOnAxisLine : MonoBehaviour
{
    Create3DObjects obj;
    public Transform Xs, Ys, Zs;
    public float Step, range;
    public List<float> requireSize;
    void Start()
    {
        obj = FindObjectOfType<Create3DObjects>();
        while (requireSize[requireSize.Count-1] != Step){
            requireSize.Add(requireSize[requireSize.Count-1]/2f);
        }
        Create();
    }
    void Create(){
        float current_step = 1f, current_range = range;
        while (current_step != Step/2f){
            for (float i=-current_range;i<=current_range;i+=current_step){
                if (Xs.Find($"pointX_{i}") == null){
                    var x = CreatePoint(new Vector3(i,0,0), Xs, $"pointX_{i}");
                }
                if (Ys.Find($"pointY_{i}") == null){
                    var y = CreatePoint(new Vector3(0,0,i), Ys, $"pointY_{i}");
                }
                if (Zs.Find($"pointZ_{i}") == null){
                    var z = CreatePoint(new Vector3(0,i,0), Zs, $"pointZ_{i}");
                    z.transform.Find("value").transform.localPosition = new Vector3(-0.12f, 0, 0);
                }
            }
            current_step /= 2;
            current_range /= 2;
        }
    }
    public GameObject CreatePoint(Vector3 position, Transform parent, string name){
        var p = obj.Point(position, parent);
        p.name = name;
        p.AddComponent<DynamicAxisPoint>();
        return p;
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
