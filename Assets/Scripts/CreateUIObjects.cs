using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class CreateUIObjects : MonoBehaviour
{
    Listener listener;
    Quaternion rot0 = Quaternion.identity;
    Vector3 pos0 = Vector3.zero;
    public GameObject DockButtonPref, DividerPref;
    void Start()
    {
        listener = FindObjectOfType<Listener>();
    }
    public void SetText(Transform text, string content){
        text.GetComponent<TextMeshProUGUI>().text = content;
    }
    public void DockButton(string name, Action onClick, Sprite icon, Transform parent){
        var button = Instantiate(DockButtonPref, pos0, rot0, parent).transform;
        button.localScale = Vector3.one;
        SetText(button.Find("TextBG/Text"), name);
        button.name = name;
        listener.Add_Button(button.GetComponent<Button>(), onClick);
        button.GetComponent<Image>().sprite = icon;
    }
    public void DockDivider(Transform parent){
        var divider = Instantiate(DividerPref, pos0, rot0, parent).transform;
        divider.localScale = Vector3.one;
    }
}
