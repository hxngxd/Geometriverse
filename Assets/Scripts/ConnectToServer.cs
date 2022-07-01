using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    MenuManager menu;
    public GameObject Loading, Create, Join, Canvas;
    void Start(){
        menu = FindObjectOfType<MenuManager>();
    }
    public void Connect(){
        Canvas.SetActive(true);
        Loading.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Disconnect(){
        Canvas.SetActive(true);
        Loading.SetActive(true);
        PhotonNetwork.LeaveLobby();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        print("CONNECTED!");
        Canvas.SetActive(false);
        Loading.SetActive(false);
        menu.Buttoggle("Kết nối đến Server", false);
        menu.Buttoggle("Tạo phòng", true);
        menu.Buttoggle("Tham gia phòng", true);
        menu.Buttoggle("Thoát phòng", false);
        menu.Buttoggle("Thoát khỏi Server", true);
    }
    public override void OnLeftLobby()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("DISCONNECTED!");
        PhotonNetwork.LeaveLobby();
        Canvas.SetActive(false);
        Loading.SetActive(false);
        menu.Buttoggle("Kết nối đến Server", true);
        menu.Buttoggle("Tạo phòng", false);
        menu.Buttoggle("Tham gia phòng", false);
        menu.Buttoggle("Thoát phòng", false);
        menu.Buttoggle("Thoát khỏi Server", false);
    }
}
