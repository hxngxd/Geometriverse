using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using INPUT=TMPro.TMP_InputField;
using TMPro;
public class Listener : MonoBehaviour
{
    public void Add(Slider slider, Action action){
        slider.onValueChanged.AddListener(delegate {action();});
    }
    public void Add(INPUT Input, Action action){
        Input.onDeselect.AddListener(delegate {action();});
    }
    public void Add(List<INPUT> Inputs, Action action){
        for (int i=0;i<Inputs.Count;i++) Add(Inputs[i], action);
    }
    public void Add(Button button, Action action){
        button.onClick.AddListener(delegate {action();});
    }
    public void Add(Toggle toggle, Action action){
        toggle.onValueChanged.AddListener(delegate {action();});
    }
    public void Remove(Slider slider){
        slider.onValueChanged.RemoveAllListeners();
    }
    public void Remove(INPUT Input){
        Input.onDeselect.RemoveAllListeners();
    }
    public void Remove(List<INPUT> Inputs){
        for (int i=0;i<Inputs.Count;i++) Remove(Inputs[i]);
    }
    public void Remove(Button button){
        button.onClick.RemoveAllListeners();
    }
    public void Remove(Toggle toggle){
        toggle.onValueChanged.RemoveAllListeners();
    }
}
