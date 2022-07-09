using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class CreateUIObjects : MonoBehaviour
{
    Draw draw;
    Quaternion rot0 = Quaternion.identity;
    Vector3 pos0 = Vector3.zero;
    public GameObject DockButtonPref, DividerPref, Vector3Pref, ValuePref, SliderPref, TogglePref, MenuItemPref, MenuCommandPref, CommandContainerPref, HierItemPref, InspectorPref;
    public Transform InspectorViewport;
    List<string> Vec3Path = new List<string>(){
        "Input_X", "Input_Y", "Input_Z"
    };
    void Start()
    {
        draw = FindObjectOfType<Draw>();
    }
    public Transform InspectorContent(string name){
        var content = Instantiate(InspectorPref, pos0, rot0, InspectorViewport).GetComponent<RectTransform>();
        content.name = name;
        content.anchoredPosition = pos0;
        return content;
    }
    public void SetText(Transform text, string content){
        text.GetComponent<TextMeshProUGUI>().text = content;
    }
    public void SetPlaceHolder(Transform text, string content){
        text.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text = content;
    }
    public void SetInputText(Transform input, string content){
        input.GetComponent<INPUT>().text = content;
    }
    public void SetInputType(Transform input, INPUT.ContentType type){
        input.GetComponent<INPUT>().contentType = type;
    }
    public void DockButton(string name, Action onClick, Sprite icon, Transform parent){
        var button = Instantiate(DockButtonPref, pos0, rot0, parent).transform;
        button.localScale = Vector3.one;
        SetText(button.Find("TextBG/Text"), name);
        button.name = name;
        button.GetComponent<Button>().onClick.AddListener(delegate {onClick();});
        button.GetComponent<Image>().sprite = icon;
    }
    public void DockDivider(Transform parent){
        var divider = Instantiate(DividerPref, pos0, rot0, parent).transform;
        divider.localScale = Vector3.one;
    }
    public void HierarchyItem(string ID, string label, Action onclick, Transform parent){
        var item = Instantiate(HierItemPref, pos0, rot0, parent).GetComponent<Toggle>();
        SetText(item.transform.Find("Text"), label);
        item.name = ID;
        draw.listener.Add(item, onclick);
    }
    public List<INPUT> Vec3(string label, Transform parent){
        var ui = Instantiate(Vector3Pref, pos0, rot0, parent).transform;
        SetText(ui.Find("Label"), label);
        List<INPUT> Inputs = new List<INPUT>();
        foreach (string name in Vec3Path){
            var path = $"Components/{name}";
            var Input = ui.Find(path);
            Inputs.Add(Input.GetComponent<INPUT>());
            SetPlaceHolder(Input, "0");
            SetInputType(Input, INPUT.ContentType.DecimalNumber);
        }
        return Inputs;
    }
    public INPUT Value(string label, string Placeholder, string text, INPUT.ContentType type, Transform parent){
        var ui = Instantiate(ValuePref, pos0, rot0, parent).transform;
        SetText(ui.Find("Label"), label);
        var path = "Components/Input";
        var Input = ui.Find(path);
        SetPlaceHolder(Input, Placeholder);
        SetInputText(Input, text);
        SetInputType(Input, type);
        return Input.GetComponent<INPUT>();
    }
    public Slider Slidr(string label, float min, float max, float value, Transform parent){
        var ui = Instantiate(SliderPref, pos0, rot0, parent).transform;
        SetText(ui.Find("Label"), label);
        var slider = ui.Find("Components/Slider").GetComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        SetInputText(ui.Find("Components/Input"), value.ToString());
        return slider;
    }
    public Toggle Togle(string label, bool isOn, Transform parent){
        var ui = Instantiate(TogglePref, pos0, rot0, parent).transform;
        SetText(ui.Find("Label"), label);
        var toggle = ui.Find("Components/Toggle").GetComponent<Toggle>();
        toggle.isOn = isOn;
        return toggle;
    }
    public RectTransform MenuItem(string label, Action onClick, Transform parent){
        var ui = Instantiate(MenuItemPref, pos0, rot0, parent).GetComponent<Button>();
        ui.onClick.AddListener(delegate {onClick();});
        SetText(ui.transform.Find("Text"), label);
        ui.name = label;
        MenuManager.Transforms.Add(label, ui.transform);
        return ui.GetComponent<RectTransform>();
    }
    public Button MenuCommand(string label, Action onClick, Transform parent){
        var ui = Instantiate(MenuCommandPref, pos0, rot0, parent).GetComponent<Button>();
        var toggle = label[label.Length-2] == '1';
        if (label.Contains("/")){
            string name = label.Substring(0, label.Length - 4);
            var tick = ui.transform.Find("Tick").gameObject;
            tick.SetActive(toggle);
            SetText(ui.transform.Find("Text"), name);
            ui.onClick.AddListener(delegate {
                onClick();
                tick.SetActive(!tick.activeSelf);
                draw.menu.HideAll();
            });
            MenuManager.Transforms.Add(name, ui.transform);
            ui.name = name;
            MenuManager.MenuObjects.Add(name, new Dictionary<string, Action>());
        }
        else if (label.Contains(">")){
            string name = label.Substring(0, label.Length - 3);
            ui.transform.Find("Expand").gameObject.SetActive(true);
            SetText(ui.transform.Find("Text"), name);
            MenuManager.Transforms.Add(name, ui.transform);
            ui.name = name;
            MenuManager.MenuObjects.Add(name, new Dictionary<string, Action>());
            onClick();
        }
        else{
            string name = label.Substring(0, label.Length - 3);
            SetText(ui.transform.Find("Text"), name);
            ui.onClick.AddListener(delegate {
                onClick();
                draw.menu.HideAll();
            });
            MenuManager.Transforms.Add(name, ui.transform);
            draw.menu.Buttoggle(name, toggle);
            ui.name = name;
            MenuManager.MenuObjects.Add(name, new Dictionary<string, Action>());
        }
        return ui;
    }
    public RectTransform CommandContainer(string label, Transform parent){
        var ui = Instantiate(CommandContainerPref, pos0, rot0, parent).GetComponent<RectTransform>();
        ui.gameObject.SetActive(false);
        ui.name = label;
        return ui;
    }
    public void RebuildLayout(RectTransform layout){
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
    }
}
