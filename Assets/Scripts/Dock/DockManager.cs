using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pair=System.Collections.Generic.KeyValuePair<System.Action, UnityEngine.Sprite>;
using ListPair=System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.KeyValuePair<System.Action, UnityEngine.Sprite>>;
public class DockManager : MonoBehaviour
{
    const string Path = "Sprites/";
    Sprite emptySprite;
    Draw draw;
    CreateUIObjects uiobj;
    List<ListPair> Functions;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        uiobj = FindObjectOfType<CreateUIObjects>();
        Functions = new List<ListPair>(){
            new ListPair("Chuột", new Pair(() => {draw.Refresh();}, Load("cursor"))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Điểm", new Pair(() => {draw.Point();}, Load("point"))),
            new ListPair("Đoạn thẳng", new Pair(() => {draw.Line();}, Load("line"))),
            new ListPair("Đoạn thẳng liên tục", new Pair(() => {draw.Line();}, Load("multiline"))),
            new ListPair("Mặt phẳng", new Pair(() => {}, Load("plane"))),
            new ListPair("Đường tròn qua 3 điểm", new Pair(() => {}, Load("circle3point"))),
            new ListPair("Đường tròn trên mặt phẳng", new Pair(() => {}, Load("circleonplane"))),
            new ListPair("Đa giác đều trên mặt phẳng", new Pair(() => {}, Load("polygon"))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Hình chóp", new Pair(() => {}, Load(""))),
            new ListPair("Hình chóp đều", new Pair(() => {}, Load(""))),
            new ListPair("Hình lập phương", new Pair(() => {}, Load(""))),
            new ListPair("Hình cầu", new Pair(() => {}, Load(""))),
            new ListPair("Hình trụ", new Pair(() => {}, Load(""))),
            new ListPair("Hình nón", new Pair(() => {}, Load(""))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Camera", new Pair(() => {draw.CameraControl();}, Load("camera"))),
        };
        Create();
    }
    public void Create(){
        foreach (ListPair func in Functions){
            if (func.Key == "/"){
                uiobj.DockDivider(this.transform);
            }
            else{
                uiobj.DockButton(func.Key, func.Value.Key, func.Value.Value, this.transform);
            }
        }
    }
    public Sprite Load(string name){
        return (name == "" ? emptySprite : Resources.Load<Sprite>(Path + name));
    }
}
