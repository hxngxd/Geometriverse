using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using Photon.Pun;
public class InputFieldHandler : MonoBehaviour
{
    Draw draw;
    PhotonView photon;
    void Start(){
        draw = FindObjectOfType<Draw>();
        photon = this.GetComponent<PhotonView>();
    }
    public bool isValid(string value){
        bool notEmpty = !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        bool notOnlySign = !(value.Length==1 && (value[0]=='.' || value[0]=='-'));
        return notEmpty && notOnlySign;
    }
    public float toFloat(INPUT Input){
        try{
            return isValid(Input.text) ? float.Parse(Input.text) : 0f;
        }
        catch{
            return 0f;
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
    [PunRPC]
    void RPC_UP(string ID, Vector3 position){
        Hierarchy.Objs[ID].go.transform.position = draw.calc.ztoy(position);
    }
    [PunRPC]
    void RPC_UN(string ID, string name){
        var obj = Hierarchy.Objs[ID];
        obj.name = name;
        Hierarchy.Objs[ID] = obj;
    }
    public void Update_Position(GameObject go, Vector3 position){
        if (draw.inRoom()){
            photon.RPC("RPC_UP", RpcTarget.All, go.name, position);
        }
        else{
            RPC_UP(go.name, position);
        }
    }
    public void Update_Name(string ID, string name){
        if (draw.inRoom()){
            photon.RPC("RPC_UN", RpcTarget.All, ID, name);
        }
        else{
            RPC_UN(ID, name);
        }
    }
}
