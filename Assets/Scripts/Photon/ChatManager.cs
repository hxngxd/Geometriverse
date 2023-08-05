using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using TMPro;
public class ChatManager : MonoBehaviour
{
    PhotonView photon;
    Recorder photonRecorder;
    public TMP_InputField input;
    public TextMeshProUGUI content;
    string[] msgs = new string[6];
    public bool isVoiceEnabled = false;
    void Start(){
        photon = this.GetComponent<PhotonView>();
        photonRecorder = this.GetComponent<Recorder>();
    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.Return)){
            Send($"<b>{PhotonNetwork.NickName}</b>: {input.text}");
        }
    }
    [PunRPC]
    public void Receive(string msg){
        content.text += $"<size=20>[{DateTime.Now.ToString("HH:mm:ss")}]</size> {msg}\n";
    }
    public void Send(string msg){
        for (int i=0;i<5;i++) msgs[i] = msgs[i+1];
        msgs[5] = input.text;
        bool diff = false;
        for (int i=0;i<6;i++) if (msgs[i] != input.text) diff = true;
        if (diff && !String.IsNullOrEmpty(input.text) && !String.IsNullOrWhiteSpace(input.text)){
            photon.RPC("Receive", RpcTarget.All, msg);
        }
        input.text = "";
        input.ActivateInputField();
        input.Select();
    }
    public void SendNotification(string msg){
        Receive(msg);
    }
    public void ToggleVoice(Image img){
        isVoiceEnabled = !isVoiceEnabled;
        img.sprite = Resources.Load<Sprite>(isVoiceEnabled ? "Sprites/micon" : "Sprites/micoff");
        photonRecorder.TransmitEnabled = isVoiceEnabled;
    }
    
}
