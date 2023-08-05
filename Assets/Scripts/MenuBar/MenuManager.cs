using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class MenuManager : MonoBehaviour
{
    public struct Cmd{
        public int type;
        //    0       1        2         3
        // button, toggle, selection, expand
        public bool enable, state;
        public Action onClick;
        public List<string> children;
    }
    public static Dictionary<string, Cmd> Cmds = new Dictionary<string, Cmd>();
    public Transform itemsContainer, Containers;
    public RectTransform MenuCanvas;
    Draw draw; 
    RoomManager room; ConnectToServer connect; 
    public List<string> firstCmds = new List<string>();
    void Start()
    {
        connect = FindObjectOfType<ConnectToServer>();
        room = FindObjectOfType<RoomManager>();
        draw = FindObjectOfType<Draw>();
        AddFirstCmds();
    }
    public void AddFirstCmds(){
        Cmds.Clear();
        firstCmds.Clear();
        File();
        Edit();
        Tool();
        Window();
        Help();
        Network();
        foreach (var cmd in firstCmds){
            draw.uiobj.MenuCommand(cmd, itemsContainer);
            draw.uiobj.RebuildLayout(MenuCanvas);
        }
        draw.mouse.OnSelectionsChange();
    }
    public void AddCmd(string label, bool enable, Action onClick, string dir){
        if (!MenuManager.Cmds.ContainsKey($"{dir}/{label}")){
            var cmd = new MenuManager.Cmd();
            cmd.type = 0;
            cmd.enable = enable;
            cmd.onClick = onClick;
            MenuManager.Cmds.Add($"{dir}/{label}", cmd);
        }
        if (dir != "" && !MenuManager.Cmds[dir].children.Contains(label)){
            MenuManager.Cmds[dir].children.Add(label);
        }
    }
    public void AddCmd(string label, int type, bool enable, bool state, Action onClick, string dir){
        if (!MenuManager.Cmds.ContainsKey($"{dir}/{label}")){
            var cmd = new MenuManager.Cmd();
            cmd.type = type;
            cmd.state = state;
            cmd.enable = enable;
            cmd.onClick = onClick;
            MenuManager.Cmds.Add($"{dir}/{label}", cmd);
        }
        if (dir != "" && !MenuManager.Cmds[dir].children.Contains(label)){
            MenuManager.Cmds[dir].children.Add(label);
        }
    }
    public void AddCmd(string label, bool enable, string dir){
        if (!MenuManager.Cmds.ContainsKey(dir == "" ? label : $"{dir}/{label}")){
            var cmd = new MenuManager.Cmd();
            cmd.type = 3;
            cmd.enable = enable;
            cmd.children = new List<string>();
            if (dir=="") MenuManager.Cmds.Add(label, cmd);
            else MenuManager.Cmds.Add($"{dir}/{label}", cmd);
        }
        if (dir != "" && !MenuManager.Cmds[dir].children.Contains(label)){
            MenuManager.Cmds[dir].children.Add(label);
        }
    }
    public RectTransform CreateChildCommand(RectTransform parent){
        var cmd = MenuManager.Cmds[parent.name];
        var container = Instantiate(draw.uiobj.MenuContainerPref, Vector3.zero, Quaternion.identity, Containers).GetComponent<RectTransform>();
        container.name = parent.name;
        foreach (string child in cmd.children){
            draw.uiobj.MenuCommand($"{parent.name}/{child}", container.Find("Container"));
        }
        if (!parent.name.Contains('/')){
            container.anchoredPosition = new Vector2(parent.anchoredPosition.x, 0);
        }
        else{
            var parentContainer = parent.parent.parent.GetComponent<RectTransform>();
            container.anchoredPosition = new Vector2(parentContainer.anchoredPosition.x + 407, parentContainer.anchoredPosition.y + parent.anchoredPosition.y + 5);
        }
        return container;
    }
    public List<string> PathSplit(string path){
        return new List<string>(path.Split('/'));
    }
    public string getName(string path){
        var split = PathSplit(path);
        return split[split.Count - 1];
    }
    public string getDir(string path){
        return path.Substring(0, path.LastIndexOf('/'));
    }
    void Update(){
        if (Containers.childCount != 0){
            if (Input.GetMouseButtonDown(0)){
                var ui = draw.raycast.GetUI(MenuCanvas);
                if (ui == null || ui == itemsContainer) HideAll();
            }
        }
    }
    public void HideAll(){
        foreach (Transform child in Containers) Destroy(child.gameObject);
    }
    public void ChangeState(string name, bool state){
        if (MenuManager.Cmds.ContainsKey(name)){
            var cmd = MenuManager.Cmds[name];
            cmd.state = state;
            MenuManager.Cmds[name] = cmd;
        }
    }
    public void Enable(string name, bool isEnable){
        var cmd = MenuManager.Cmds[name];
        cmd.enable = isEnable;
        MenuManager.Cmds[name] = cmd;
    }
    public void File(){
        firstCmds.Add("Tập tin");
        AddCmd("Tập tin", true, "");
            if (draw.allowDrawing){
                AddCmd("Không gian mới", true, () => {
                    if (draw.inRoom()){
                        draw.hier.photon.RPC("ResetAll", RpcTarget.AllBuffered);
                    }
                    else{
                        draw.hier.ResetAll();
                    }
                }, "Tập tin");
                AddCmd("Mở tập tin", false, () => {

                }, "Tập tin");
            }
            AddCmd("Lưu", false, () => {

            }, "Tập tin");
            AddCmd("Lưu thành", false, () => {

            }, "Tập tin");
            AddCmd("Thoát", true, () => {
                Application.Quit();
            }, "Tập tin");
    }
    public void Edit(){
        if (!draw.allowDrawing) return;
        firstCmds.Add("Chỉnh sửa");
        AddCmd("Chỉnh sửa", true, "");
            AddCmd("Undo", false, () => {
                draw.action.Undo();
            }, "Chỉnh sửa");
            AddCmd("Redo", false, () => {
                draw.action.Redo();
            }, "Chỉnh sửa");
            AddCmd("Xoá", false, () => {
                draw.mouse.DeleteSelected();
            }, "Chỉnh sửa");
    }
    public void Tool(){
        if (!draw.allowDrawing) return;
        firstCmds.Add("Công cụ");
        AddCmd("Công cụ", true, "");
            AddCmd("Vẽ thủ công", true, "Công cụ");
                AddCmd("Điểm", true, () => {
                    draw.letDraw(draw.point);
                }, "Công cụ/Vẽ thủ công");
                AddCmd("Đường thẳng", true, "Công cụ/Vẽ thủ công");
                    AddCmd("Đoạn thẳng nối 2 điểm", true, () => {
                        draw.letDraw(draw.line, new bool[]{false, true});
                    }, "Công cụ/Vẽ thủ công/Đường thẳng");
                    AddCmd("Đoạn thẳng liên tục", true, () => {
                        draw.letDraw(draw.line, new bool[]{true, true});
                    }, "Công cụ/Vẽ thủ công/Đường thẳng");
                    AddCmd("Đường thằng đi qua 2 điểm", true, () => {
                        draw.letDraw(draw.line, new bool[]{false, false});
                    }, "Công cụ/Vẽ thủ công/Đường thẳng");
                    AddCmd("Đường thẳng liên tục", true, () => {
                        draw.letDraw(draw.line, new bool[]{true, false});
                    }, "Công cụ/Vẽ thủ công/Đường thẳng");
                AddCmd("Mặt phẳng", true, () => {
                    draw.letDraw(draw.plane);
                }, "Công cụ/Vẽ thủ công");
                AddCmd("Đường tròn", true, "Công cụ/Vẽ thủ công");
                    AddCmd("Đi qua 3 điểm", true, () => {
                        draw.letDraw(draw.circle, new bool[]{false});
                    }, "Công cụ/Vẽ thủ công/Đường tròn");
                    AddCmd("Trên mặt phẳng", true, () => {
                        draw.letDraw(draw.circle, new bool[]{true});
                    }, "Công cụ/Vẽ thủ công/Đường tròn");
                AddCmd("Đa giác", true, "Công cụ/Vẽ thủ công");
                    AddCmd("Đa giác đều trên mặt phẳng", true, () => {
                        draw.letDraw(draw.polygon);
                    }, "Công cụ/Vẽ thủ công/Đa giác");
                    AddCmd("Hình tam giác", false, "Công cụ/Vẽ thủ công/Đa giác");
                        AddCmd("Cân", false, () => {}, "Công cụ/Vẽ thủ công/Đa giác/Hình tam giác");
                        AddCmd("Vuông", false, () => {}, "Công cụ/Vẽ thủ công/Đa giác/Hình tam giác");
                        AddCmd("Vuông cân", false, () => {}, "Công cụ/Vẽ thủ công/Đa giác/Hình tam giác");
                    AddCmd("Hình chữ nhật", false, () => {}, "Công cụ/Vẽ thủ công/Đa giác");
                AddCmd("Hình cầu", true, () => {
                    draw.letDraw(draw.sphere);
                }, "Công cụ/Vẽ thủ công");
            AddCmd("Vẽ tự động", true, "Công cụ");
                AddCmd("Qua điểm vẽ", true, "Công cụ/Vẽ tự động");
                    AddCmd("Đường thẳng", true, "Công cụ/Vẽ tự động/Qua điểm vẽ");
                        AddCmd("Song song với", true, "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng");
                            AddCmd("Đường thẳng", true, () => {}, "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng/Song song với");
                            AddCmd("Mặt phẳng", true, () => {},  "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng/Song song với");
                        AddCmd("Vuông góc với", true,  "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng");
                            AddCmd("Đường thẳng", true, () => {},  "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng/Vuông góc với");
                            AddCmd("Mặt phẳng", true, () => {},  "Công cụ/Vẽ tự động/Qua điểm vẽ/Đường thẳng/Vuông góc với");
            AddCmd("Hình chiếu", true, "Công cụ");
                AddCmd("Của điểm", true, "Công cụ/Hình chiếu");
                    AddCmd("Lên đường thẳng", false, () => {
                        var point_go = draw.obj.Point(draw.calc.hc_diem_dt(getPointPosIndex(0), getLinePos(0)), draw.hier.created);
                        var line_go = draw.obj.Line(draw.hier.created, false);
                        draw.hier.AddPoint("", point_go, new List<string>(){getID("line", 0)}, true, draw.inRoom());
                        draw.hier.AddLine("", line_go, new List<string>(){getID("point", 0), point_go.name}, draw.inRoom());
                    }, "Công cụ/Hình chiếu/Của điểm");
                    AddCmd("Lên đường tròn", false, () => {}, "Công cụ/Hình chiếu/Của điểm");
                    AddCmd("Lên mặt phẳng", false, () => {
                        var point_go = draw.obj.Point(draw.calc.ztoy(draw.calc.hc_diem_mp(draw.calc.ztoy(getPointPosIndex(0)), getPlaneEq(0))), draw.hier.created);
                        var line_go = draw.obj.Line(draw.hier.created, false);
                        draw.hier.AddPoint("", point_go, new List<string>(){getID("plane", 0)}, true, draw.inRoom());
                        draw.hier.AddLine("", line_go, new List<string>(){getID("point", 0), point_go.name}, draw.inRoom());
                    }, "Công cụ/Hình chiếu/Của điểm");
                    AddCmd("Lên hình cầu", false, () => {}, "Công cụ/Hình chiếu/Của điểm");
                    AddCmd("Lên đa giác đều", false, () => {}, "Công cụ/Hình chiếu/Của điểm");
                AddCmd("Của đường thẳng", true, "Công cụ/Hình chiếu");
                    AddCmd("Lên mặt phẳng", false, () => {}, "Công cụ/Hình chiếu/Của đường thẳng");
                AddCmd("Của đường tròn", true, "Công cụ/Hình chiếu");
                    AddCmd("Lên mặt phẳng", false, () => {}, "Công cụ/Hình chiếu/Của đường tròn");
            AddCmd("Giao điểm", true, "Công cụ");
                AddCmd("Đường thẳng và đường thẳng", false, () => {
                    var intersect = draw.calc.duong_vgc(getLinePos(0), getLinePos(1));
                    var point_go1 = draw.obj.Point(intersect.Key, draw.hier.created);
                    draw.hier.AddPoint("", point_go1, (intersect.Key == intersect.Value ? new List<string>(){getID("line", 0), getID("line", 1)} : new List<string>(){getID("line", 0)}), true, draw.inRoom());
                    if (intersect.Key != intersect.Value){
                        var point_go2 = draw.obj.Point(intersect.Value, draw.hier.created);
                        draw.hier.AddPoint("", point_go2, new List<string>(){getID("line", 1)}, true, draw.inRoom());
                        var line_go = draw.obj.Line(draw.hier.created, false);
                        draw.hier.AddLine("", line_go, new List<string>(){point_go1.name, point_go2.name}, draw.inRoom());
                    }
                }, "Công cụ/Giao điểm");
                AddCmd("Đường thẳng và mặt phẳng", false, () => {}, "Công cụ/Giao điểm");
            AddCmd("Giao tuyến", true, "Công cụ");
                AddCmd("Mặt phẳng và mặt phẳng", false, () => {}, "Công cụ/Giao tuyến");
                AddCmd("Mặt phẳng và hình cầu", false, () => {}, "Công cụ/Giao tuyến");
        
        string getID(string type, int index){
            return draw.mouse.Selected[type][index].name;
        }
        Vector3 getPointPosIndex(int index){
            return draw.mouse.Selected["point"][index].position;
        }
        Vector3 getPointPosID(string id){
            return Hierarchy.Objs[id].go.transform.position;
        }
        KeyValuePair<Vector3, Vector3> getLinePos(int index){
            var vertices = Hierarchy.Objs[getID("line", index)].vertices;
            return new KeyValuePair<Vector3, Vector3>(getPointPosID(vertices[0]), getPointPosID(vertices[1]));
        }
        Dictionary<string, float> getPlaneEq(int index){
            return Hierarchy.Objs[getID("plane", index)].equation;
        }
    }           
    public void Window(){
        firstCmds.Add("Cửa sổ");
        AddCmd("Cửa sổ", true, "");
            AddCmd("Toàn màn hình", 1, true, Screen.fullScreen, () => {
                Screen.fullScreen = !Screen.fullScreen;
            }, "Cửa sổ");
            AddCmd("Độ phân giải", true, "Cửa sổ");
                var resolutions = Screen.resolutions;
                foreach (var res in resolutions){
                    AddCmd($"{res.width} x {res.height}", 2, true, (res.width == Screen.width && res.height == Screen.height), () => {
                        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                    }, "Cửa sổ/Độ phân giải");
                }
            if (draw.allowDrawing) AddCmd("Tự động ẩn thanh công cụ", 1, true, draw.dockah.autohide, () => {
                draw.dockah.autohide = !draw.dockah.autohide;
                var wdcv = draw.panel.canvas.GetComponent<RectTransform>();
                wdcv.offsetMin = new Vector2(wdcv.offsetMin.x, draw.dockah.autohide ? 0 : 90);
                draw.dockah.GetComponent<LayoutGroup>().padding.top = (draw.dockah.autohide ? 50 : 5);
                draw.dockah.GetComponent<LayoutGroup>().enabled = false;
                draw.dockah.GetComponent<LayoutGroup>().enabled = true;
            }, "Cửa sổ");
            AddCmd("Hierarchy", 1, true, draw.panel.Hierarchy.gameObject.activeSelf, () => {
                if (draw.panel.Hierarchy.gameObject.activeSelf) draw.panel.CloseTab("Hierarchy");
                else draw.panel.CreateTab("Hierarchy", draw.panel.Hierarchy);
            }, "Cửa sổ");
            AddCmd("Inspector", 1, true, draw.panel.Inspector.gameObject.activeSelf, () => {
                if (draw.panel.Inspector.gameObject.activeSelf) draw.panel.CloseTab("Inspector");
                else draw.panel.CreateTab("Inspector", draw.panel.Inspector);
            }, "Cửa sổ");
            AddCmd("Trò chuyện", 1, PhotonNetwork.InRoom, draw.panel.Chat.gameObject.activeSelf, () => {
                if (draw.panel.Chat.gameObject.activeSelf) draw.panel.CloseTab("Trò chuyện");
                else draw.panel.CreateTab("Trò chuyện", draw.panel.Chat);
            }, "Cửa sổ");
    }
    public void Help(){
        firstCmds.Add("Trợ giúp");
        AddCmd("Trợ giúp", true, "");
            AddCmd("Hướng dẫn", true, () => {
                Application.OpenURL("https://github.com/hxngxd/Geometriverse");
            }, "Trợ giúp");
    }
    public void Network(){
        firstCmds.Add("Mạng");
        AddCmd("Mạng", true, "");
            AddCmd("Kết nối đến server", !PhotonNetwork.IsConnected, () => {
                connect.Connect();
            }, "Mạng");
            AddCmd("Thoát khỏi server", PhotonNetwork.IsConnected, () => {
                connect.Disconnect();
            }, "Mạng");
            AddCmd("Tạo phòng", PhotonNetwork.IsConnected && !PhotonNetwork.InRoom, () => {
                room.RoomBG.SetActive(true);
                room.CreateUI.SetActive(true);
                room.JoinUI.SetActive(false);
                room.Header.text = "TẠO PHÒNG";
                room.CopyBtn.SetActive(true);
                room.ID.readOnly = true;
                room.ID.text = draw.id.RoomID();
                room.Password.text = "";
                room.CreateBtn.interactable = true;
            }, "Mạng");
            AddCmd("Tham gia phòng", PhotonNetwork.IsConnected && !PhotonNetwork.InRoom, () => {
                room.RoomBG.SetActive(true);
                room.CreateUI.SetActive(false);
                room.JoinUI.SetActive(true);
                room.Header.text = "THAM GIA PHÒNG";
                room.CopyBtn.SetActive(false);
                room.ID.readOnly = false;
                room.ID.text = "";
                room.Password.text = "";
                room.JoinBtn.interactable = true;
            }, "Mạng");
            AddCmd("Thoát phòng", PhotonNetwork.IsConnected && PhotonNetwork.InRoom, () => {
                room.LeaveRoom();
            }, "Mạng");
    }
}
