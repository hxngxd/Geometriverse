using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDHandler : MonoBehaviour
{
    List<char> CharList = new List<char>(){
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','M','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        '0','1','2','3','4','5','6','7','8','9'
    };
    public string GenID(int length){
        string ID = "";
        for (int i=0;i<length;i++){
            int randomID = Random.Range(0, CharList.Count);
            ID += CharList[randomID];
        }
        return ID;
    }
    public string RoomID(){
        return $"{GenID(4)}-{GenID(4)}-{GenID(4)}";
    }
}
