using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class InputFieldHandler : MonoBehaviour
{
    Draw draw;
    void Start(){
        draw = FindObjectOfType<Draw>();
    }
    public bool isValid(string value){
        bool valid = !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != "";
        if (value.Length==1){
            if (value[0]=='.' || value[0]=='-') valid = false;
        }
        return valid;
    }
    public float toFloat(INPUT Input){
        try{
            return isValid(Input.text) ? float.Parse(Input.text) : 0f;
        }
        catch{
            return 0;
        }
    }
    public int toInt(INPUT Input){
        try{
            return isValid(Input.text) ? int.Parse(Input.text) : 0;
        }
        catch{
            return 0;
        }
    }
    public void Vec2Input(List<INPUT> Inputs, Vector3 vector){
        Inputs[0].text = vector.x.ToString();
        Inputs[1].text = vector.y.ToString();
        Inputs[2].text = vector.z.ToString();
    }
    public void Float2Input(INPUT Input, float value){
        Input.text = value.ToString();
    }
    public void Int2Input(INPUT Input, int value){
        Input.text = value.ToString();
    }
    public Vector3 Input2Vec(List<INPUT> Inputs){
        return new Vector3(toFloat(Inputs[0]), toFloat(Inputs[1]), toFloat(Inputs[2]));
    }
    public void ResetInput(INPUT Input){
        draw.listener.Remove(Input);
        Input.text = "";
    }
    public void ResetInputs(List<INPUT> Inputs){
        foreach (INPUT i in Inputs) ResetInput(i);
    }

    public void Update_Position(GameObject go, Vector3 position){
        go.transform.position = draw.calc.ztoy(position);
    }
    public void Update_GeoObj_Name(string ID, string name){
        var obj = Hierarchy.Objs[ID];
        // Hierarchy.Points[ID] = obj;
    }
}
