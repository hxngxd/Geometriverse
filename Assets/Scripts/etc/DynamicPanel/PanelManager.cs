using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicPanels;

public class PanelManager : MonoBehaviour
{
    Draw draw;
    public DynamicPanelsCanvas canvas;
    public RectTransform Inspector, Hierarchy, About, Settings, Chat;
    public Dictionary<string, PanelTab> TabsList = new Dictionary<string, PanelTab>();
    public Transform Tabs;
    Vector2 minSize = new Vector2(250f, 200f);
    float initSize = 500f;
    void Awake(){
        draw = FindObjectOfType<Draw>();
        PanelNotificationCenter.OnTabClosed += OnTabClosed;
    }
    void Start(){
        PanelGroup group = new PanelGroup(canvas, Direction.Top);
        group.AddElement(CreateTab("Inspector", Inspector));
        group.AddElement(CreateTab("Hierarchy", Hierarchy));
        group.DockToRoot(Direction.Left);
        canvas.ForceRebuildLayoutImmediate();
        group.ResizeTo(new Vector2(initSize, group.Size.y));
    }
    public Panel CreateTab(string label, RectTransform content){
        Panel newPanel = PanelUtils.CreatePanelFor(content, canvas);
        newPanel[0].Label = label;
        newPanel[0].MinSize = minSize;
        newPanel.ResizeTo(new Vector2(initSize, initSize));
        TabsList.Add(label, newPanel[0]);
        return newPanel;
    }
    void OnTabClosed(PanelTab tab){
        draw.menu.ChangeState(tab.Content.name, false);
        TabsList.Remove(draw.menu.getName(tab.Content.name));
        tab.Content.gameObject.SetActive(false);
        tab.Content.transform.SetParent(Tabs);
        tab.Destroy();
    }
    public void CloseTab(string name){
        TabsList[name].Content.gameObject.SetActive(false);
        TabsList[name].Content.transform.SetParent(Tabs);
        TabsList[name].Destroy();
        TabsList.Remove(name);
    }
    
}
