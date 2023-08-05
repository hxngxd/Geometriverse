using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomInfoManager : MonoBehaviour
{
    public GameObject info, textPref, togglePref;
    public Transform num, names, time, allow;
    public TMP_InputField id;
    public TextMeshProUGUI maxplayer;
    public Dictionary<string, Player> Players = new Dictionary<string, Player>(); 
    PhotonView photon;
    RoomManager room;
    Draw draw;
    ChatManager chat;
    void Start(){
        photon = this.GetComponent<PhotonView>();
        room = FindObjectOfType<RoomManager>();
        draw = FindObjectOfType<Draw>();
        chat = FindObjectOfType<ChatManager>();
    }
    void Update()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom){
            if (Input.GetKeyDown(KeyCode.Tab)){
                Show();
            }
            if (Input.GetKeyUp(KeyCode.Tab)){
                Hide();
            }
        }
    }
    public void PutText(string label, Transform parent){
        var text = Instantiate(textPref, Vector3.zero, Quaternion.identity, parent).GetComponent<TextMeshProUGUI>();
        text.text = label;
        draw.uiobj.RebuildLayout(info.GetComponent<RectTransform>());
    }
    public void PutToggle(bool isOn, string receiver){
        var toggle = Instantiate(togglePref, Vector3.zero, Quaternion.identity, allow).transform.Find("Toggle").GetComponent<Toggle>();
        toggle.isOn = isOn;
        toggle.interactable = PhotonNetwork.IsMasterClient;

        if (receiver == PhotonNetwork.LocalPlayer.UserId){
            toggle.onValueChanged.AddListener(delegate {
                draw.AllowDraw(toggle.isOn);
                photon.RPC("UpdatePlayerList", RpcTarget.Others);
            });
        }
        else{
            toggle.onValueChanged.AddListener(delegate {
                photon.RPC("RPC_Allow", RpcTarget.Others, receiver, toggle.isOn);
                Players[receiver].CustomProperties["allow"] = toggle.isOn;
            });
        }
    }
    [PunRPC]
    public void RPC_Allow(string receiver, bool allow){
        chat.SendNotification(allow ? $"<b>{Players[receiver].NickName}</b> đã được cấp quyền vẽ!" : $"<b>{Players[receiver].NickName}</b> đã bị thu hồi quyền vẽ!");
        UpdatePlayerList();
        if (PhotonNetwork.LocalPlayer.UserId == receiver){
            draw.noti.Show(allow ? "Bạn đã được cấp quyền vẽ!" : "Bạn đã bị thu hồi quyền vẽ!");
            draw.AllowDraw(allow);
        }
    }
    public void Show(){
        info.SetActive(true);
        id.text = PhotonNetwork.CurrentRoom.Name;
        maxplayer.text = $"<b>{PhotonNetwork.CurrentRoom.MaxPlayers.ToString()}</b> người";
        int index = 1;
        PutText("STT", num);
        PutText("Tên", names);
        PutText("Thời gian tham gia", time);
        PutText("Được vẽ", allow);
        foreach (var player in Players){
            PutText(index.ToString(), num);
            if (player.Key == PhotonNetwork.LocalPlayer.UserId){
                PutText(player.Value.NickName + " (bạn)" + (player.Value.IsMasterClient ? " (chủ phòng)" : ""), names);
                PutText(room.joinTime, time);
                PutToggle(draw.allowDrawing, player.Key);
            }
            else{
                PutText(player.Value.NickName + (player.Value.IsMasterClient ? " (chủ phòng)" : ""), names);
                PutText(player.Value.CustomProperties["time"].ToString(), time);
                PutToggle((bool)player.Value.CustomProperties["allow"], player.Key);
            }
            index++;
        }
    }
    public void Hide(){
        info.SetActive(false);
        foreach (var p in new List<Transform>(){num, names, time, allow}){
            foreach (Transform child in p){
                Destroy(child.gameObject);
            }
        }
    }
    [PunRPC]
    public void UpdatePlayerList(){
        Players.Clear();
        foreach (var player in PhotonNetwork.CurrentRoom.Players){
            player.Value.CustomProperties = new ExitGames.Client.Photon.Hashtable();
            Players.Add(player.Value.UserId, player.Value);
            photon.RPC("RPC_RequestInfo", RpcTarget.Others, PhotonNetwork.LocalPlayer.UserId, player.Value.UserId);
        }
    }
    [PunRPC]
    void RPC_RequestInfo(string sender, string receiver){
        if (PhotonNetwork.LocalPlayer.UserId == receiver){
            photon.RPC("SendInfo", RpcTarget.Others, PhotonNetwork.LocalPlayer.UserId, sender, room.joinTime, draw.allowDrawing);
        }
    }
    [PunRPC]
    void SendInfo(string sender, string receiver, string time, bool allowdrawing){
        if (PhotonNetwork.LocalPlayer.UserId == receiver){
            Players[sender].CustomProperties["time"] = time;
            Players[sender].CustomProperties["allow"] = allowdrawing;
        }
    }
}
