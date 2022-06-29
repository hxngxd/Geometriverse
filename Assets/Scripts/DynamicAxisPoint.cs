using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicAxisPoint : MonoBehaviour
{
    PointOnAxisLine poal;
    TextMeshPro value;
    string axis;
    Vector3 previousPosition;
    void Start()
    {
        poal = FindObjectOfType<PointOnAxisLine>();
        value = this.transform.Find("value").GetComponent<TextMeshPro>();
        axis = this.name[0].ToString();
        this.GetComponent<DynamicSize>().ratio /= 1.25f;
        this.transform.Find("dot").localScale /= 1.75f;
    }

    void Update()
    {
        if (this.transform.position != previousPosition){
            previousPosition = this.transform.position;
            switch (axis){
                case "X":
                    value.text = this.transform.position.x.ToString();
                    break;
                case "Y":
                    value.text = this.transform.position.z.ToString();
                    break;
                case "Z":
                    value.text = this.transform.position.y.ToString();
                    break;
            }
        }
        if (this.transform.position == Vector3.zero) this.transform.localScale = Vector3.zero;
        if (this.transform.localScale.x > poal.maxSize) this.transform.localScale = new Vector3(poal.maxSize,poal.maxSize,poal.maxSize);
        if (this.name.Contains('.')){
            this.transform.Find("dot").gameObject.SetActive(this.transform.localScale.x <= poal.requireSize[NumAfterDot(this.name)]);
            this.transform.Find("value").gameObject.SetActive(this.transform.localScale.x <= poal.requireSize[NumAfterDot(this.name)]);
        }
    }
    public int NumAfterDot(string name){
        return name.Length - name.IndexOf('.') - 1;
    }
}
