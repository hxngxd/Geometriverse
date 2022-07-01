using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class MenuManager : MonoBehaviour
{
    public static Dictionary<string, Dictionary<string, Action>> MenuObjects = new Dictionary<string, Dictionary<string, Action>>();
    public static Dictionary<string, Transform> Transforms = new Dictionary<string, Transform>();
    public Transform itemsContainer, Containers;
    public RectTransform MenuCanvas;
    Draw draw; DockAutoHide dockAH;
    bool someoneIsNotHidden;
    Transform previousHit;
    Transform currentSubContainer;
    ConnectToServer connect;
    RoomManager room;
    LineCollider linecollider;
    PhotonView photon;
    void Start()
    {
        connect = FindObjectOfType<ConnectToServer>();
        dockAH = FindObjectOfType<DockAutoHide>();
        draw = FindObjectOfType<Draw>();
        room = FindObjectOfType<RoomManager>();
        linecollider = FindObjectOfType<LineCollider>();
        photon = GetComponent<PhotonView>();
        File();
        Edit();
        Tool();
        Window();
        Help();
        Network();
    }
    public void File(){
        var container = draw.uiobj.CommandContainer("Tập tin", Containers);
        var item = draw.uiobj.MenuItem("Tập tin", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Làm mới(1)", () => {
                draw.hier.ResetAll();
            }},
            {"Thoát(1)", () => {
                Application.Quit();
            }},
        };
        CreateCommands(cmd, container, false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Edit(){
        var container = draw.uiobj.CommandContainer("Chỉnh sửa", Containers);
        var item = draw.uiobj.MenuItem("Chỉnh sửa", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Xoá(0)", () => {
                draw.mouse.DeleteSelected();
            }},
        };
        CreateCommands(cmd, container, false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    [PunRPC]
    void AddChildren(string parent, string child){
        draw.point.RPC_AddChildren(parent, child);
    }
    public void Tool(){
        var container = draw.uiobj.CommandContainer("Công cụ", Containers);
        var item = draw.uiobj.MenuItem("Công cụ", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Hình chiếu(>)", () => {
                var cmd = new Dictionary<string, Action>(){
                    {"Điểm lên đoạn thẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["point"]==1 && draw.mouse.SelectionCount["line"]==1 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var start = Hierarchy.Lines[d["line"][0]].start;
                        var end = Hierarchy.Lines[d["line"][0]].end;
                        var p = Hierarchy.Points[d["point"][0]];
                        var p1 = draw.obj.Point((Hierarchy.Points[start].go.transform.position+Hierarchy.Points[end].go.transform.position)/2, draw.hierContent);
                        p1.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["line"][0], p1);
                        Hierarchy.Lines[d["line"][0]].children.Add(p1.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["line"][0], p1.name);
                    }},
                    {"Điểm lên mặt phẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["point"]==1 && draw.mouse.SelectionCount["plane"]==1 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var p = Hierarchy.Points[d["point"][0]];
                        var plane = Hierarchy.Planes[d["plane"][0]];
                        var equation = plane.equation;
                        var p1 = draw.obj.Point(draw.calc.swapYZ(draw.calc.HC_diem_len_mp(draw.calc.swapYZ(p.go.transform.position), equation)), draw.hierContent);
                        p1.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["plane"][0], p1);
                        Hierarchy.Planes[d["plane"][0]].children.Add(p1.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["plane"][0], p1.name);
                    }},
                    {"Đoạn thẳng lên măt phẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["line"]==1 && draw.mouse.SelectionCount["plane"]==1 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var start = Hierarchy.Lines[d["line"][0]].start;
                        var end = Hierarchy.Lines[d["line"][0]].end;
                        var plane = Hierarchy.Planes[d["plane"][0]];
                        var equation = plane.equation;
                        var p1 = draw.obj.Point(draw.calc.swapYZ(draw.calc.HC_diem_len_mp(draw.calc.swapYZ(Hierarchy.Points[start].go.transform.position), equation)), draw.hierContent);
                        p1.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["plane"][0], p1);
                        Hierarchy.Planes[d["plane"][0]].children.Add(p1.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["plane"][0], p1.name);
                        var p2 = draw.obj.Point(draw.calc.swapYZ(draw.calc.HC_diem_len_mp(draw.calc.swapYZ(Hierarchy.Points[end].go.transform.position), equation)), draw.hierContent);
                        p2.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["plane"][0], p2);
                        Hierarchy.Planes[d["plane"][0]].children.Add(p2.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["plane"][0], p2.name);
                        var l = draw.obj.Line(draw.hierContent);
                        draw.hier.AddLine("", p1.name, p2.name, d["plane"][0], l, new List<string>());
                        Hierarchy.Planes[d["plane"][0]].children.Add(l.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["plane"][0], l.name);
                    }}
                };
                MenuObjects["Hình chiếu"] = cmd;
            }},
            {"Giao điểm(>)", () => {
                var cmd = new Dictionary<string, Action>(){
                    {"Đoạn thẳng và đoạn thẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["line"]==2 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var l1 = Hierarchy.Lines[d["line"][0]];
                        var l2 = Hierarchy.Lines[d["line"][1]];
                        var p = draw.calc.Duong_vuong_goc_chung(new KeyValuePair<Vector3, Vector3>(Hierarchy.Points[l1.start].go.transform.position, Hierarchy.Points[l1.end].go.transform.position), new KeyValuePair<Vector3, Vector3>(Hierarchy.Points[l2.start].go.transform.position, Hierarchy.Points[l2.end].go.transform.position));
                        var p1 = draw.obj.Point(p.Key, draw.hierContent);
                        p1.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["line"][0], p1);
                        Hierarchy.Lines[d["line"][0]].children.Add(p1.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["line"][0], p1.name);
                        if (p.Key != p.Value){
                            var p2 = draw.obj.Point(p.Value, draw.hierContent);
                            p2.GetComponent<SphereCollider>().enabled = true;
                            draw.hier.AddPoint("", d["line"][1], p2);
                            Hierarchy.Lines[d["line"][1]].children.Add(p2.name);
                            if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["line"][1], p1.name);
                        }
                    }},
                    {"Đường vuông góc chung 2 đoạn thẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["line"]==2 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var l1 = Hierarchy.Lines[d["line"][0]];
                        var l2 = Hierarchy.Lines[d["line"][1]];
                        var p = draw.calc.Duong_vuong_goc_chung(new KeyValuePair<Vector3, Vector3>(Hierarchy.Points[l1.start].go.transform.position, Hierarchy.Points[l1.end].go.transform.position), new KeyValuePair<Vector3, Vector3>(Hierarchy.Points[l2.start].go.transform.position, Hierarchy.Points[l2.end].go.transform.position));
                        var p1 = draw.obj.Point(p.Key, draw.hierContent);
                        p1.GetComponent<SphereCollider>().enabled = true;
                        draw.hier.AddPoint("", d["line"][0], p1);
                        Hierarchy.Lines[d["line"][0]].children.Add(p1.name);
                        if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["line"][0], p1.name);
                        if (p.Key != p.Value){
                            var p2 = draw.obj.Point(p.Value, draw.hierContent);
                            p2.GetComponent<SphereCollider>().enabled = true;
                            draw.hier.AddPoint("", d["line"][1], p2);
                            Hierarchy.Lines[d["line"][1]].children.Add(p2.name);
                            if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["line"][1], p1.name);

                            var l3 = draw.obj.Line(draw.hierContent);
                            l3.positionCount=2;
                            draw.hier.AddLine("", p1.name, p2.name, "", l3, new List<string>());
                        }
                    }},
                    {"Đoạn thẳng và mặt phẳng(1)", () => {
                        if (!(draw.mouse.SelectionCount["line"]==1 && draw.mouse.SelectionCount["plane"]==1 && draw.mouse.Selected.Count==2)) return;
                        var d = draw.mouse.GetSelections();
                        var l = Hierarchy.Lines[d["line"][0]];
                        var plane = Hierarchy.Planes[d["plane"][0]];
                        var equation = plane.equation;
                        var start = Hierarchy.Points[l.start];
                        var end = Hierarchy.Points[l.end];
                        var intersect = draw.calc.intersect_line_plane(new KeyValuePair<Vector3, Vector3>(draw.calc.swapYZ(start.go.transform.position), draw.calc.swapYZ(end.go.transform.position)), equation);
                        if (intersect.Key){
                            var p1 = draw.obj.Point(draw.calc.swapYZ(intersect.Value), draw.hierContent);
                            p1.GetComponent<SphereCollider>().enabled = true;
                            draw.hier.AddPoint("", d["plane"][0], p1);
                            Hierarchy.Planes[d["plane"][0]].children.Add(p1.name);
                            if (RoomManager.inRoom) photon.RPC("AddChildren", RpcTarget.OthersBuffered, d["plane"][0], p1.name);
                        }
                    }},
                };
                MenuObjects["Giao điểm"] = cmd;
            }},
        };
        CreateCommands(cmd, container, false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Window(){
        var container = draw.uiobj.CommandContainer("Cửa sổ", Containers);
        var item = draw.uiobj.MenuItem("Cửa sổ", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {$"Toàn màn hình(/{b2i(Screen.fullScreen)})", () => {
                Screen.fullScreen = !Screen.fullScreen;
            }},
            {$"Độ phân giải(>)", () => {
                Resolution[] resolutions = Screen.resolutions;
                var cmd = new Dictionary<string, Action>();
                foreach (var res in resolutions){
                    if (!cmd.ContainsKey($"{res.width}x{res.height}(1)")) cmd.Add($"{res.width}x{res.height}(1)", () => {
                        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                    });
                }
                MenuObjects["Độ phân giải"] = cmd;
            }},
            {$"Tự động ẩn thanh công cụ(/{b2i(dockAH.autohide)})", () => {
                dockAH.autohide = !dockAH.autohide;
            }},
            {$"Hierarchy(/{b2i(draw.panel.Hierarchy.gameObject.activeSelf)})", () => {
                if (draw.panel.Hierarchy.gameObject.activeSelf){
                    draw.panel.CloseTab("Hierarchy");
                }
                else{
                    draw.panel.CreateTab("Hierarchy", draw.panel.Hierarchy);
                }
            }},
            {$"Inspector(/{b2i(draw.panel.Inspector.gameObject.activeSelf)})", () => {
                if (draw.panel.Inspector.gameObject.activeSelf){
                    draw.panel.CloseTab("Inspector");
                }
                else{
                    draw.panel.CreateTab("Inspector", draw.panel.Inspector);
                }
            }},
            // {$"Cài đặt(/{b2i(draw.panel.Settings.gameObject.activeSelf)})", () => {
            //     if (draw.panel.Settings.gameObject.activeSelf){
            //         draw.panel.CloseTab("Cài đặt");
            //     }
            //     else{
            //         draw.panel.CreateTab("Cài đặt", draw.panel.Settings);
            //     }
            // }}
        };
        CreateCommands(cmd, container, false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Help(){
        var container = draw.uiobj.CommandContainer("Trợ giúp", Containers);
        var item = draw.uiobj.MenuItem("Trợ giúp", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {$"Về Geometriverse(/{b2i(draw.panel.About.gameObject.activeSelf)})", () => {
                if (draw.panel.About.gameObject.activeSelf){
                    draw.panel.CloseTab("Về Geometriverse");
                }
                else{
                    draw.panel.CreateTab("Về Geometriverse", draw.panel.About);
                }
            }},
            {$"Hướng dẫn(/{b2i(draw.panel.Manual.gameObject.activeSelf)})", () => {
                if (draw.panel.Manual.gameObject.activeSelf){
                    draw.panel.CloseTab("Hướng dẫn");
                }
                else{
                    draw.panel.CreateTab("Hướng dẫn", draw.panel.Manual);
                }
            }},
        };
        CreateCommands(cmd, container, false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Network(){
        var container = draw.uiobj.CommandContainer("Mạng", Containers);
        var item = draw.uiobj.MenuItem("Mạng", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Kết nối đến Server(1)", () => {
                connect.Connect();
            }},
            {"Thoát khỏi Server(0)", () => {
                connect.Disconnect();
            }},
            {"Tạo phòng(0)", () => {
                connect.Canvas.SetActive(true);
                connect.Create.SetActive(true);
                room.CreateID.text = room.ID = $"{draw.idhandler.RoomID(7)}-{draw.idhandler.RoomID(7)}-{draw.idhandler.RoomID(7)}";
            }},
            {"Tham gia phòng(0)", () => {
                connect.Canvas.SetActive(true);
                connect.Join.SetActive(true);
            }},
            {"Thoát phòng(0)", () => {
                room.Leave();
            }},
            {$"Trò chuyện(/{b2i(draw.panel.Chat.gameObject.activeSelf)})", () => {
                if (draw.panel.Chat.gameObject.activeSelf){
                    draw.panel.CloseTab("Trò chuyện");
                }
                else{
                    draw.panel.CreateTab("Trò chuyện", draw.panel.Chat);
                }
            }},
        };
        CreateCommands(cmd, container, false);
        Buttoggle("Trò chuyện", false);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void ToggleContainer(RectTransform container){
        HideAll();
        container.gameObject.SetActive(true);
        someoneIsNotHidden = true;
        container.anchoredPosition = new Vector3(itemsContainer.Find(container.name).GetComponent<RectTransform>().anchoredPosition.x, -40f, 0);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public string b2i(bool b){
        return (b ? "1" : "0");
    }
    public void Tickle(string name, bool toggle){
        var tick = Transforms[name].Find("Tick").gameObject;
        tick.gameObject.SetActive(toggle);
    }
    public void Buttoggle(string name, bool toggle){
        Transforms[name].GetComponent<Button>().interactable = toggle;
        Transforms[name].Find("Text").GetComponent<TextMeshProUGUI>().color = (toggle ? Color.white : Color.gray);
    }
    public void HideAll(){
        foreach (Transform child in Containers){
            child.gameObject.SetActive(false);
        }
        someoneIsNotHidden = false;
    }
    public void CreateCommands(Dictionary<string, Action> cmds, RectTransform container, bool isAdded){
        List<string> cmd = new List<string>();
        foreach (var child in cmds){
            draw.uiobj.MenuCommand(child.Key, child.Value, container);
            cmd.Add(child.Key);
        }
        if (!isAdded) MenuObjects.Add(container.name, cmds);
    }
    public void DestroySubContainer(){
        if (currentSubContainer != null){
            foreach (Transform child in currentSubContainer){
                MenuObjects.Remove(child.name);
                Transforms.Remove(child.name);
                Destroy(child.gameObject);
            }
            Destroy(currentSubContainer.gameObject);
            currentSubContainer = null;
        }
    }
    void Update()
    {
        var hit = draw.raycast.GetUI(GameObject.Find("MenuCanvas").GetComponent<RectTransform>());
        if (someoneIsNotHidden){
            if (hit == null || hit.name == "Items Container"){
                if (Input.GetMouseButtonDown(0)) HideAll();
            }
            else{
                if (hit.IsChildOf(itemsContainer)){
                    ToggleContainer(Containers.Find(hit.name).GetComponent<RectTransform>());
                    DestroySubContainer();
                }
                else if (hit.parent.IsChildOf(Containers) && Transforms.ContainsValue(hit)){
                    if (previousHit != hit){
                        previousHit = hit;
                        if (hit.Find("Expand").gameObject.activeSelf){
                            void Create(){
                                var container = draw.uiobj.CommandContainer($"{hit.name}_subContainer", hit.parent);
                                container.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                                container.anchoredPosition = new Vector3(507f, hit.GetComponent<RectTransform>().anchoredPosition.y + 7, 0);
                                CreateCommands(MenuObjects[hit.name], container, true);
                                container.gameObject.SetActive(true);
                                draw.uiobj.RebuildLayout(MenuCanvas);
                                currentSubContainer = container;
                            }
                            if (currentSubContainer == null){
                                Create();
                            }
                            else{
                                if (currentSubContainer.name != $"{hit.name}_subContainer"){
                                    DestroySubContainer();
                                    Create();
                                }
                            }
                        }
                        else{
                            if (currentSubContainer != null && hit.parent.name != currentSubContainer.name) DestroySubContainer();
                        }
                    }
                }
            }
        }
    }
}
