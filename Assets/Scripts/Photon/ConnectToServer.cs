using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    Draw draw;
    RoomManager room;
    public string dir = "Mạng/", connectDir, disconnectDir, createroomDir, joinroomDir, exitroomDir, chatdir = "Cửa sổ/Trò chuyện";
    bool connectOnClick = false;
    void Start(){
        draw = FindObjectOfType<Draw>();
        room = FindObjectOfType<RoomManager>();
        connectDir = dir + "Kết nối đến server";
        disconnectDir = dir + "Thoát khỏi server";
        createroomDir = dir + "Tạo phòng";
        joinroomDir = dir + "Tham gia phòng";
        exitroomDir = dir + "Thoát phòng";
    }
    public void Connect(){
        draw.noti.Loading("Chuẩn bị kết nối đến server...");
        draw.menu.Enable(connectDir, false);
        PhotonNetwork.ConnectUsingSettings();
        connectOnClick = true;
    }
    public override void OnConnectedToMaster()
    {
        if (connectOnClick) draw.noti.Show("Đã kết nối thành công!");
        connectOnClick = false;
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        draw.menu.Enable(disconnectDir, true);
        draw.menu.Enable(createroomDir, true);
        draw.menu.Enable(joinroomDir, true);
        room.RoomList.Clear();
    }

    public override void OnLeftLobby()
    {
        room.RoomList.Clear();
    }
    public void Disconnect(){
        draw.noti.Loading("Đang thoát server...");
        draw.menu.Enable(disconnectDir, false);
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        switch (cause.ToString()){
            case "DnsExceptionOnConnect":
                draw.noti.Show("Vui lòng kiểm tra lại kết nối Internet!");
                break;
            case "DisconnectByClientLogic":
                draw.noti.Show("Đã thoát khỏi server!");
                break;      
            case "ClientTimeout":
            case "Exception":
                draw.noti.Show("Mất kết nối đến server, vui lòng kết nối lại!");
                break;         
        }
        room.RoomBG.SetActive(false);
        draw.menu.Enable(connectDir, true);
        draw.menu.Enable(disconnectDir, false);
        draw.menu.Enable(createroomDir, false);
        draw.menu.Enable(joinroomDir, false);
        room.RoomList.Clear();
        print(cause);
    }
}
