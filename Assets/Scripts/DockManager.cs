using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pair=System.Collections.Generic.KeyValuePair<System.Action, UnityEngine.Sprite>;
using ListPair=System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.KeyValuePair<System.Action, UnityEngine.Sprite>>;
public class DockManager : MonoBehaviour
{
    const string Path = "Sprites/Icons/";
    Sprite emptySprite;
    Draw draw;
    CreateUIObjects uiobj;
    List<ListPair> Functions;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        uiobj = FindObjectOfType<CreateUIObjects>();
        Functions = new List<ListPair>(){
            new ListPair("Điểm", new Pair(() => {}, Load(""))),
            new ListPair("Đường thẳng", new Pair(() => {}, Load(""))),
            new ListPair("/", new Pair(() => {}, Load(""))),
            new ListPair("Mặt phẳng", new Pair(() => {}, Load(""))),
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
        // return Resources.Load<Sprite>(Path + name);
        return emptySprite;
    }
}
