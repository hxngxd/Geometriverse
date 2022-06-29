using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using INPUT=TMPro.TMP_InputField;
using TMPro;
public class Listener : MonoBehaviour
{
    public void Add_Slider(Slider slider, Action action){
        slider.onValueChanged.AddListener(delegate {action();});
    }
    public void Add_Input(INPUT Input, Action action){
        Input.onDeselect.AddListener(delegate {action();});
    }
    public void Add_Inputs(List<INPUT> Inputs, int startIndex, Action action){
        for (int i=startIndex;i<Inputs.Count;i++) Add_Input(Inputs[i], action);
    }
    public void Add_Button(Button button, Action action){
        button.onClick.AddListener(delegate {action();});
    }
    public void Add_Toggle(Toggle toggle, Action action){
        toggle.onValueChanged.AddListener(delegate {action();});
    }
    // public void Remove_Slider(Slider slider){
    //     slider.onValueChanged.RemoveAllListeners();
    //     Add_Slider(slider, () => {
    //         slider.transform.Find("Label/Text").GetComponent<TextMeshProUGUI>().text = draw.calc.fRound(slider.value, 1).ToString();
    //     });
    // }
    public void Remove_Input(INPUT Input){
        Input.onDeselect.RemoveAllListeners();
    }
    public void Remove_Inputs(List<INPUT> Inputs, int startIndex){
        for (int i=startIndex;i<Inputs.Count;i++) Remove_Input(Inputs[i]);
    }
    public void Remove_Button(Button button){
        button.onClick.RemoveAllListeners();
    }
    public void Remove_Toggle(Toggle toggle){
        toggle.onValueChanged.RemoveAllListeners();
    }
}
