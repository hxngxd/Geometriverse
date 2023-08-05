using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionManager : MonoBehaviour
{
    Draw draw;
    public static List<KeyValuePair<string, object[]>> UndoActions = new List<KeyValuePair<string, object[]>>();
    public static List<KeyValuePair<string, object[]>> RedoActions = new List<KeyValuePair<string, object[]>>();
    void Start()
    {
        draw = FindObjectOfType<Draw>();
    }
    void Update(){
        if (!draw.allowDrawing) return;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
            if (Input.GetKeyDown(KeyCode.Z)){
                Undo();
            }
            if (Input.GetKeyDown(KeyCode.Y)){
                Redo();
            }
        }
        if (UndoActions.Count > 25){
            UndoActions.RemoveAt(0);
        }
        if (MenuManager.Cmds.ContainsKey("Chỉnh sửa/Undo") && MenuManager.Cmds.ContainsKey("Chỉnh sửa/Redo")){
            if (UndoActions.Count == 0 && MenuManager.Cmds["Chỉnh sửa/Undo"].enable){
                draw.menu.Enable("Chỉnh sửa/Undo", false);
            }
            if (UndoActions.Count != 0 && !MenuManager.Cmds["Chỉnh sửa/Undo"].enable){
                draw.menu.Enable("Chỉnh sửa/Undo", true);
            }

            if (RedoActions.Count == 0 && MenuManager.Cmds["Chỉnh sửa/Redo"].enable){
                draw.menu.Enable("Chỉnh sửa/Redo", false);
            }
            if (RedoActions.Count != 0 && !MenuManager.Cmds["Chỉnh sửa/Redo"].enable){
                draw.menu.Enable("Chỉnh sửa/Redo", true);
            }
        }
    }
    public void Undo(){
        if (UndoActions.Count == 0) return;
        var type = UndoActions[UndoActions.Count-1].Key;
        var param = UndoActions[UndoActions.Count-1].Value;
        switch (type){
            case "setpos":
                var go = Hierarchy.Objs[(string)param[0]].go;
                var current_pos = draw.calc.ztoy(go.transform.position);
                RedoActions.Add(new KeyValuePair<string, object[]>("setpos", new object[]{param[0], current_pos}));
                draw.input.Update_Position(Hierarchy.Objs[(string)param[0]].go, (Vector3)param[1]);
                break;
        }
        UndoActions.RemoveAt(UndoActions.Count-1);
    }
    public void Redo(){
        if (RedoActions.Count == 0) return;
        var type = RedoActions[RedoActions.Count-1].Key;
        var param = RedoActions[RedoActions.Count-1].Value;
        switch (type){
            case "setpos":
                var go = Hierarchy.Objs[(string)param[0]].go;
                var current_pos = draw.calc.ztoy(go.transform.position);
                UndoActions.Add(new KeyValuePair<string, object[]>("setpos", new object[]{param[0], current_pos}));
                draw.input.Update_Position(Hierarchy.Objs[(string)param[0]].go, (Vector3)param[1]);
                break;
        }
        RedoActions.RemoveAt(RedoActions.Count-1);
    }
}
