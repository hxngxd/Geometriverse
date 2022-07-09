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
        this.transform.Find("dot").localScale /= 1.5f;
        this.GetComponent<SphereCollider>().radius = 0.08f;
    }

    void Update()
    {
        if (this.transform.position != previousPosition){
            previousPosition = this.transform.position;
            switch (this.name[0]){
                case 'X':
                    value.text = this.transform.position.x.ToString();
                    break;
                case 'Y':
                    value.text = this.transform.position.z.ToString();
                    break;
                case 'Z':
                    value.text = this.transform.position.y.ToString();
                    break;
            }
        }
        if (this.transform.position == Vector3.zero){
            this.transform.localScale = Vector3.zero;
        }
        if (this.transform.localScale.x > poal.requireSize[0]){
            this.transform.localScale = new Vector3(poal.requireSize[0], poal.requireSize[0], poal.requireSize[0]);
        }
        if (this.name.Contains('.')){
            bool active = this.transform.localScale.x <= poal.requireSize[NumAfterDot(this.name)];
            this.transform.Find("dot").gameObject.SetActive(active);
            value.gameObject.SetActive(active);
        }
        this.GetComponent<SphereCollider>().enabled = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && value.gameObject.activeSelf);
    }
    public int NumAfterDot(string name){
        return name.Length - name.IndexOf('.') - 1;
    }
}
