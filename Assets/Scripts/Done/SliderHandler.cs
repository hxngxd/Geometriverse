using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;

public class SliderHandler : MonoBehaviour
{
    public INPUT valueInput;
    public Slider slider;
    Draw draw;
    float previousValue;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        draw.listener.Add(valueInput, () => {
            float min = slider.minValue, max = slider.maxValue;
            float currentval = draw.input.toFloat(valueInput);
            if (currentval < min) currentval = min;
            if (currentval > max) currentval = max;
            slider.value = currentval;
        });
    }

    void Update()
    {
        if (previousValue != slider.value){
            previousValue = slider.value;
            valueInput.text = draw.calc.fRound(slider.value, 2).ToString();
        }
    }
}
