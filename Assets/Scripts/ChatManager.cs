using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class ChatManager : MonoBehaviour
{
    PhotonView photon;
    public TMP_InputField input;
    public TextMeshProUGUI content;
    void Awake()
    {
        photon = GetComponent<PhotonView>();
    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.Return)){
            Send($"<b>{PhotonNetwork.NickName}</b>: {input.text}");
        }
    }
    [PunRPC]
    void Receive(string msg){
        content.text += msg + "\n";
    }
    public void Send(string msg){
        photon.RPC("Receive", RpcTarget.AllBufferedViaServer, msg);
        input.text = "";
        input.Select();
        input.ActivateInputField();
    }

    
}
