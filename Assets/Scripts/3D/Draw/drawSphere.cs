using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class drawSphere : MonoBehaviour
{
    public Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Draw draw;
    public Transform content;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        content = draw.uiobj.InspectorContent(this.GetType().Name);
        Inspector();
    }
    public void Inspector(){
        Inputs.Add("name", new List<INPUT>(){draw.uiobj.Value("Tên đoạn", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("center_name", new List<INPUT>(){draw.uiobj.Value("Tên tâm", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("center_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("vertex_name", new List<INPUT>(){draw.uiobj.Value("Tên đỉnh", "Tên...", "", INPUT.ContentType.Alphanumeric, content)});
        Inputs.Add("vertex_pos", draw.uiobj.Vec3("Toạ độ", content));
        Inputs.Add("radius", new List<INPUT>(){draw.uiobj.Value("Bán kính", "0", "", INPUT.ContentType.DecimalNumber, content)});
        content.gameObject.SetActive(false);
    }
}
