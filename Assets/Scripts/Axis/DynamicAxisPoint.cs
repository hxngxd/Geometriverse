using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DynamicAxisPoint : MonoBehaviour
{
    PointOnAxisLine poal;
    TextMeshPro value;
    Vector3 previousPosition;
    void Start()
    {
        poal = FindObjectOfType<PointOnAxisLine>();
        value = this.transform.Find("value").GetComponent<TextMeshPro>();
        this.GetComponent<DynamicSize>().ratio /= 1.25f;
        this.transform.Find("dot").localScale /= 1.75f;
    }

    void Update()
    {
        if (this.transform.position != previousPosition){
            previousPosition = this.transform.position;
            if (this.name.Contains("X")){
                value.text = this.transform.position.x.ToString();
            }
            else if (this.name.Contains("Y")){
                value.text = this.transform.position.z.ToString();
            }
            else{
                value.text = this.transform.position.y.ToString();
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
