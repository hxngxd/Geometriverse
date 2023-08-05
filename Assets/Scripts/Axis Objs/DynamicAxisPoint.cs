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
        
        if (this.transform.position == Vector3.zero && this.name != "Centre"){
            this.transform.localScale = Vector3.zero;
        }
        else{
            this.transform.localScale = Vector3.one * poal.ratio * Vector3.Distance(this.transform.position, Camera.main.transform.position);
        }

        bool active = this.transform.localScale.x <= (this.name.Contains('.') ? poal.sizes[NumAfterDot(this.name)] : poal.max);
        this.transform.Find("dot").gameObject.SetActive(active);
        value.gameObject.SetActive(active);

        this.GetComponent<SphereCollider>().enabled = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && value.gameObject.activeSelf);
    }

    public int NumAfterDot(string name){
        return name.Length - name.IndexOf('.') - 1;
    }
}
