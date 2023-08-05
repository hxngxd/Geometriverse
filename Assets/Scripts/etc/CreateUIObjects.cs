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
    public GameObject DockButtonPref, DividerPref, Vector3Pref, ValuePref, SliderPref, TogglePref, HierItemPref, InspectorPref, MenuCmdPref, MenuContainerPref;
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
    public Button HierarchyItem(string ID, Transform parent){
        var item = Instantiate(HierItemPref, pos0, rot0, parent).transform;
        item.name = ID;
        SetText(item.Find("id"), $"ID: {ID}");
        return item.GetComponent<Button>();
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
    public void MenuCommand(string name, Transform container){
        var ui = Instantiate(MenuCmdPref, pos0, rot0, container).GetComponent<Button>();
        SetText(ui.transform.Find("Text"), draw.menu.getName(name));
        ui.name = name;
        var cmd = MenuManager.Cmds[name];
        ui.interactable = cmd.enable;
        if (!cmd.enable){
            ui.transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color32(128,128,128,255);
        }
        if (cmd.type == 3){
            ui.gameObject.AddComponent<Expand>();
            if (!name.Contains("/")){
                ui.GetComponent<LayoutElement>().enabled = false;
                ui.transform.Find("Text").GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                ui.GetComponent<LayoutGroup>().padding.left = 15;
                ui.onClick.AddListener(delegate {
                    var container = ui.GetComponent<Expand>().container;
                    if (container == null){
                        container = draw.menu.CreateChildCommand(ui.GetComponent<RectTransform>());
                    }
                });
            }
            else ui.transform.Find("Expand").gameObject.SetActive(true);
        }
        else{
            if (cmd.type == 1){
                var toggle = ui.transform.Find("Toggle").gameObject;
                toggle.SetActive(cmd.state);
                ui.onClick.AddListener(delegate {
                    draw.menu.ChangeState(ui.name, !toggle.activeSelf);
                });
            }
            else if (cmd.type == 2){
                ui.transform.Find("Select").gameObject.SetActive(cmd.state);
                ui.onClick.AddListener(delegate {
                    var dir = draw.menu.getDir(ui.name);
                    foreach (string child in MenuManager.Cmds[dir].children){
                        var name = $"{dir}/{child}";
                        draw.menu.ChangeState(name, name == ui.name);
                    }
                });
            }
            ui.onClick.AddListener(delegate {
                cmd.onClick();
                draw.menu.HideAll();
            });
        }
    }
    public void RebuildLayout(RectTransform layout){
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
    }
}
