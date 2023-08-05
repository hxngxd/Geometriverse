using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOnAxisLine : MonoBehaviour
{
    Draw draw;
    public Transform Xs, Ys, Zs;
    public float step, ratio, max;
    public List<float> sizes, ranges;
    public Material Xm, Ym, Zm;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        for (float i=0;i<=step;i++){
            sizes.Add(1f / draw.calc.pow(2, i));
            ranges.Add(8f / draw.calc.pow(2, i));
        }
        Create();
    }

    void Create(){
        for (int i=0;i<=step;i++){
            for (float j=-ranges[i];j<=ranges[i];j+=sizes[i]){
                if (Xs.Find($"Xpoint_{j}") == null){
                    var x = CreatePoint(new Vector3(j,0,0), Xs, $"Xpoint_{j}", Xm);
                }
                if (Ys.Find($"Ypoint_{j}") == null){
                    var y = CreatePoint(new Vector3(0,0,j), Ys, $"Ypoint_{j}", Ym);
                }
                if (Zs.Find($"Zpoint_{j}") == null){
                    var z = CreatePoint(new Vector3(0,j,0), Zs, $"Zpoint_{j}", Zm);
                    z.transform.Find("value").transform.localPosition = new Vector3(-0.12f, 0, 0);
                }
            }
        }
    }

    public GameObject CreatePoint(Vector3 position, Transform parent, string name, Material color){
        var p = draw.obj.Point(position, parent);
        p.name = name;
        Destroy(p.GetComponent<DynamicSize>());
        p.AddComponent<DynamicAxisPoint>();
        p.transform.Find("dot").GetComponent<MeshRenderer>().material = color;
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
