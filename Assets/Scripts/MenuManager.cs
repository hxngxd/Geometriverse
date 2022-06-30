using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{
    public static Dictionary<string, List<string>> MenuObjects = new Dictionary<string, List<string>>();
    public static Dictionary<string, Transform> Transforms = new Dictionary<string, Transform>();
    public Transform itemsContainer, Containers;
    public RectTransform MenuCanvas;
    Draw draw; DockAutoHide dockAH;
    bool someoneIsNotHidden;
    void Start()
    {
        dockAH = FindObjectOfType<DockAutoHide>();
        draw = FindObjectOfType<Draw>();
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
            {"Làm mới", () => {
                draw.hier.ResetAll();
            }},
            {"Thoát", () => {
                Application.Quit();
            }},
        };
        CreateCommands(cmd, container);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Edit(){
        var container = draw.uiobj.CommandContainer("Chỉnh sửa", Containers);
        var item = draw.uiobj.MenuItem("Chỉnh sửa", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"edit1", () => {
                
            }},
            {"Xoá", () => {
                
            }},
        };
        CreateCommands(cmd, container);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Tool(){
        var container = draw.uiobj.CommandContainer("Công cụ", Containers);
        var item = draw.uiobj.MenuItem("Công cụ", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"tool1", () => {
                
            }},
            {"tool2", () => {
                
            }},
        };
        CreateCommands(cmd, container);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Window(){
        var container = draw.uiobj.CommandContainer("Cửa sổ", Containers);
        var item = draw.uiobj.MenuItem("Cửa sổ", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Toàn màn hình/"+b2i(Screen.fullScreen), () => {
                Screen.fullScreen = !Screen.fullScreen;
            }},
            {$"Độ phân giải ({Screen.currentResolution})", () => {
                ////
            }},
            {"Tự động ẩn thanh công cụ/"+b2i(dockAH.autohide), () => {
                dockAH.autohide = !dockAH.autohide;
            }},
            {"Hierarchy/"+b2i(draw.panel.Hierarchy.gameObject.activeSelf), () => {
                if (draw.panel.Hierarchy.gameObject.activeSelf){
                    draw.panel.CloseTab("Hierarchy");
                }
                else{
                    draw.panel.CreateTab("Hierarchy", draw.panel.Hierarchy);
                }
            }},
            {"Inspector/"+b2i(draw.panel.Inspector.gameObject.activeSelf), () => {
                if (draw.panel.Inspector.gameObject.activeSelf){
                    draw.panel.CloseTab("Inspector");
                }
                else{
                    draw.panel.CreateTab("Inspector", draw.panel.Inspector);
                }
            }},
            {"Cài đặt/"+b2i(draw.panel.Settings.gameObject.activeSelf), () => {
                if (draw.panel.Settings.gameObject.activeSelf){
                    draw.panel.CloseTab("Cài đặt");
                }
                else{
                    draw.panel.CreateTab("Cài đặt", draw.panel.Settings);
                }
            }}
        };
        CreateCommands(cmd, container);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Help(){
        var container = draw.uiobj.CommandContainer("Trợ giúp", Containers);
        var item = draw.uiobj.MenuItem("Trợ giúp", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Về Geometriverse/"+b2i(draw.panel.About.gameObject.activeSelf), () => {
                if (draw.panel.About.gameObject.activeSelf){
                    draw.panel.CloseTab("Về Geometriverse");
                }
                else{
                    draw.panel.CreateTab("Về Geometriverse", draw.panel.About);
                }
            }},
            {"Hướng dẫn/"+b2i(draw.panel.Manual.gameObject.activeSelf), () => {
                if (draw.panel.Manual.gameObject.activeSelf){
                    draw.panel.CloseTab("Hướng dẫn");
                }
                else{
                    draw.panel.CreateTab("Hướng dẫn", draw.panel.Manual);
                }
            }},
        };
        CreateCommands(cmd, container);
        draw.uiobj.RebuildLayout(MenuCanvas);
    }
    public void Network(){
        var container = draw.uiobj.CommandContainer("Mạng", Containers);
        var item = draw.uiobj.MenuItem("Mạng", () => ToggleContainer(container), itemsContainer);
        var cmd = new Dictionary<string, Action>(){
            {"Kết nối đến Server", () => {}},
            {"Thoát khỏi Server0", () => {}},
            {"Tạo phòng0", () => {}},
            {"Tham gia phòng0", () => {}},
            {"Thoát phòng0", () => {}},
        };
        CreateCommands(cmd, container);
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
    public void CreateCommands(Dictionary<string, Action> cmds, RectTransform container){
        List<string> cmd = new List<string>();
        foreach (var child in cmds){
            draw.uiobj.MenuCommand(child.Key, child.Value, container);
            cmd.Add(child.Key);
        }
        MenuObjects.Add(container.name, cmd);
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
                }
            }
        }
    }
}
