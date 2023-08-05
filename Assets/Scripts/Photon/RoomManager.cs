using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using INPUT=TMPro.TMP_InputField;
using Photon.Realtime;
using Photon.Voice.PUN;
public class RoomManager : MonoBehaviourPunCallbacks
{
    Draw draw;
    ConnectToServer connect;
    ChatManager chat;
    RoomInfoManager info;
    PhotonView photon;
    public GameObject RoomBG, CreateUI, JoinUI, CopyBtn;
    public TextMeshProUGUI Header;
    public INPUT Name, ID, Password;
    public TMP_Dropdown PlayerCount;
    public Button CreateBtn, JoinBtn;
    public Dictionary<string, RoomInfo> RoomList = new Dictionary<string, RoomInfo>();
    public string currentMasterClient = "", joinTime = "";
    void Start()
    {
        photon = this.GetComponent<PhotonView>();
        draw = FindObjectOfType<Draw>();
        connect = FindObjectOfType<ConnectToServer>();
        chat = FindObjectOfType<ChatManager>();
        info = FindObjectOfType<RoomInfoManager>();
        CreateBtn.onClick.AddListener(() => {CreateRoom();});
        JoinBtn.onClick.AddListener(() => {JoinRoom();});
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {   
        foreach (var info in roomList){
            if (info.RemovedFromList){
                RoomList.Remove(info.Name);
            }
            else{
                RoomList[info.Name] = info;
            }
        }
    }
    public void CreateRoom(){
        if (String.IsNullOrEmpty(Name.text) || String.IsNullOrWhiteSpace(Name.text)){
            draw.noti.Show("Vui lòng nhập tên hiển thị!");
            return;
        }
        CreateBtn.interactable = false;
        draw.noti.Loading("Đang tạo phòng...");
        RoomOptions option = new RoomOptions();
        PhotonNetwork.NickName = Name.text;
        option.MaxPlayers = System.Convert.ToByte(PlayerCount.value + 2);
        option.PublishUserId = true;
        option.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        option.CustomRoomProperties.Add("pw", Password.text);
        option.CustomRoomProperties.Add("master", PhotonNetwork.NickName);
        option.CustomRoomPropertiesForLobby = new string[]{"pw", "master"};
        PhotonNetwork.CreateRoom(ID.text, option);

        currentMasterClient = "";
        chat.input.text = chat.content.text = "";
    }
    public void JoinRoom(){
        if (String.IsNullOrEmpty(Name.text) || String.IsNullOrWhiteSpace(Name.text)){
            draw.noti.Show("Vui lòng nhập tên hiển thị!");
            return;
        }
        if (String.IsNullOrEmpty(ID.text) || String.IsNullOrWhiteSpace(ID.text)){
            draw.noti.Show("Vui lòng nhập mã phòng!");
            return;
        }
        if (!RoomList.ContainsKey(ID.text)){
            draw.noti.Show("Phòng không tồn tại!");
            return;
        }
        else{
            if (Password.text != RoomList[ID.text].CustomProperties["pw"].ToString()){
                draw.noti.Show("Sai mật khẩu, vui lòng thử lại!");
                return;
            }
            currentMasterClient = RoomList[ID.text].CustomProperties["master"].ToString();
        }
        JoinBtn.interactable = false;
        draw.noti.Loading("Đang tham gia phòng...");
        PhotonNetwork.NickName = Name.text;
        PhotonNetwork.JoinRoom(ID.text);

        chat.input.text = chat.content.text = "";
    }
    public override void OnJoinedRoom()
    {
        draw.noti.Show(PhotonNetwork.IsMasterClient ? "Đã tạo phòng!" : "Đã tham gia phòng!");
        if (PhotonNetwork.IsMasterClient){
            chat.SendNotification($"<b>{PhotonNetwork.NickName}</b> đã tạo phòng!");
        }
        else{
            chat.SendNotification($"<b>{PhotonNetwork.NickName}</b> đã tham gia phòng!");
            draw.AllowDraw(false);
        }
        RoomBG.SetActive(false);
        draw.menu.Enable(connect.createroomDir, false);
        draw.menu.Enable(connect.joinroomDir, false);
        draw.menu.Enable(connect.exitroomDir, true);
        draw.menu.Enable(connect.chatdir, true);
        draw.menu.ChangeState(connect.chatdir, true);
        draw.panel.CreateTab("Trò chuyện", draw.panel.Chat);
        info.UpdatePlayerList();
        joinTime = DateTime.Now.ToString("HH:mm:ss");
        draw.hier.ResetAll();
    }
    public void LeaveRoom(){
        draw.noti.Loading("Đang rời khỏi phòng...");
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        if (!draw.allowDrawing) draw.AllowDraw(true);
        chat.input.text = chat.content.text = "";
        draw.noti.Show("Đã rời khỏi phòng!");
        draw.menu.Enable(connect.createroomDir, true);
        draw.menu.Enable(connect.joinroomDir, true);
        draw.menu.Enable(connect.exitroomDir, false);
        draw.menu.Enable(connect.chatdir, false);
        draw.menu.ChangeState(connect.chatdir, false);
        if (draw.panel.TabsList.ContainsKey("Trò chuyện")) draw.panel.CloseTab("Trò chuyện");
        draw.hier.ResetAll();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        draw.noti.Show("Lỗi không xác định, không thể tạo phòng!");
        CreateBtn.interactable = true;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
        switch (message){
            case "Game does not exist":
                draw.noti.Show("Phòng không tồn tại!");
                break;
            case "Game full":
                draw.noti.Show("Phòng đã đủ người!");
                break;
        }
        JoinBtn.interactable = true;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        var msg = $"<b>{otherPlayer.NickName}</b> đã thoát phòng!";
        if (otherPlayer.NickName != currentMasterClient){
            draw.noti.Show(msg);
        }
        else{
            draw.noti.Loading("Chủ phòng đã thoát hoặc mất kết nối, chuẩn bị rời khỏi phòng!");
            draw.menu.Enable(connect.exitroomDir, false);
            Invoke("LeaveRoom", 2f);
        }
        chat.SendNotification(msg);
        info.UpdatePlayerList();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var msg = $"<b>{newPlayer.NickName}</b> đã tham gia phòng!";
        draw.noti.Show(msg);
        chat.SendNotification(msg);
        info.UpdatePlayerList();
    }
    public void CopyID(INPUT id){
        GUIUtility.systemCopyBuffer = id.text;
        draw.noti.Show($"Mã phòng đã được copy vào Clipboard!");
    }
    public void TogglePassword(TextMeshProUGUI text){
        if (text.text == "HIỆN"){
            text.text = "ẨN";
            Password.contentType = INPUT.ContentType.Standard;
        }
        else{
            text.text = "HIỆN";
            Password.contentType = INPUT.ContentType.Password;
        }
        Password.enabled = false;
        Password.enabled = true;
    }
}
