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
        draw.listener.Remove_Input(Input);
        Input.text = "";
    }
    public void ResetInputs(List<INPUT> Inputs){
        foreach (INPUT i in Inputs) ResetInput(i);
    }

    public void Update_Position(GameObject go, Vector3 position){
        go.transform.position = draw.calc.swapYZ(position);
    }
    public void Update_Point_Name(string ID, string name){
        var obj = Hierarchy.Points[ID];
        obj.name = name;
        Hierarchy.Points[ID] = obj;
    }
    public void Update_Line_Name(string ID, string name){
        var obj = Hierarchy.Lines[ID];
        obj.name = name;
        Hierarchy.Lines[ID] = obj;
    }
    public void Update_Plane_Name(string ID, string name){
        var obj = Hierarchy.Planes[ID];
        obj.name = name;
        Hierarchy.Planes[ID] = obj;
    }
    public void Update_Circle_Name(string ID, string name){
        var obj = Hierarchy.Circles[ID];
        obj.name = name;
        Hierarchy.Circles[ID] = obj;
    }
    public void Update_Polygon_Name(string ID, string name){
        var obj = Hierarchy.Polygons[ID];
        obj.name = name;
        Hierarchy.Polygons[ID] = obj;
    }
}
