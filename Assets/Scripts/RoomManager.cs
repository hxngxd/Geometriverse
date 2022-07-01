using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using Photon.Realtime;
public class RoomManager : MonoBehaviourPunCallbacks
{
    MenuManager menu;
    ConnectToServer connect;
    IDHandler idhandler;
    public INPUT JoinInput, CreateInput, JoinName, RoomID, CreateID;
    public string ID;
    void Start()
    {
        menu = FindObjectOfType<MenuManager>();
        idhandler = FindObjectOfType<IDHandler>();
        connect = FindObjectOfType<ConnectToServer>();
    }

    public void Create(){
        connect.Canvas.SetActive(true);
        connect.Loading.SetActive(true);
        connect.Create.SetActive(false);
        PhotonNetwork.CreateRoom(ID);
        PhotonNetwork.NickName = CreateInput.text;
    }
    public void CopyID(){
        GUIUtility.systemCopyBuffer = ID;
    }
    public void Join(){
        if (JoinInput.text.Length==23){
            connect.Canvas.SetActive(true);
            connect.Loading.SetActive(true);
            connect.Join.SetActive(false);
            PhotonNetwork.JoinRoom(JoinInput.text);
            PhotonNetwork.NickName = JoinName.text;
            ID = JoinInput.text;
        }
        else{

        }
    }
    public void Leave(){
        connect.Canvas.SetActive(true);
        connect.Loading.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinedRoom()
    {
        print("JOINED");
        connect.Canvas.SetActive(false);
        connect.Loading.SetActive(false);
        menu.Buttoggle("Kết nối đến Server", false);
        menu.Buttoggle("Tạo phòng", false);
        menu.Buttoggle("Tham gia phòng", false);
        menu.Buttoggle("Thoát phòng", true);
        menu.Buttoggle("Thoát khỏi Server", false);
        RoomID.gameObject.SetActive(true);
        RoomID.text = ID;
    }
    public override void OnLeftRoom()
    {
        connect.Canvas.SetActive(false);
        connect.Loading.SetActive(false);
        menu.Buttoggle("Kết nối đến Server", false);
        menu.Buttoggle("Tạo phòng", true);
        menu.Buttoggle("Tham gia phòng", true);
        menu.Buttoggle("Thoát phòng", false);
        menu.Buttoggle("Thoát khỏi Server", true);
        RoomID.gameObject.SetActive(false);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("FAILED");
        connect.Loading.SetActive(false);
        menu.Buttoggle("Kết nối đến Server", false);
        menu.Buttoggle("Tạo phòng", true);
        menu.Buttoggle("Tham gia phòng", true);
        menu.Buttoggle("Thoát phòng", false);
        menu.Buttoggle("Thoát khỏi Server", true);
        RoomID.gameObject.SetActive(false);
        connect.Canvas.SetActive(true);
        connect.Join.SetActive(true);
    }
}
