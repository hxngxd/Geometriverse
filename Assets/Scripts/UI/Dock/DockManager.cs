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
            new ListPair("Điểm", new Pair(() => {draw.letDraw(draw.point);}, Load("point"))),
            new ListPair("Đoạn thẳng", new Pair(() => {draw.letDraw(draw.line, false);}, Load("line"))),
            new ListPair("Đoạn thẳng liên tục", new Pair(() => {draw.letDraw(draw.line, true);}, Load("multiline"))),
            new ListPair("Mặt phẳng", new Pair(() => {draw.letDraw(draw.plane);}, Load("plane"))),
            new ListPair("Đường tròn qua 3 điểm", new Pair(() => {draw.letDraw(draw.circle, false);}, Load("circle3point"))),
            new ListPair("Đường tròn trên mặt phẳng", new Pair(() => {draw.letDraw(draw.circle, true);}, Load("circleonplane"))),
            new ListPair("Đa giác", new Pair(() => {draw.letDraw(draw.polygon, false);}, Load(""))),
            new ListPair("Đa giác đều", new Pair(() => {draw.letDraw(draw.polygon, true);}, Load(""))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Hình cầu", new Pair(() => {draw.letDraw(draw.sphere);}, Load(""))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Camera", new Pair(() => {}, Load("camera"))),
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
