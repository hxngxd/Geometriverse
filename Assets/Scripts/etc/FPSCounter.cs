using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI Display;
    float freq = 1f, time;
    int frameCount=0;
    void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time > freq){
            Display.text = $"{Mathf.RoundToInt(frameCount/time)}fps" + (PhotonNetwork.IsConnected ? $" {PhotonNetwork.GetPing()}ms": "");
            time -= freq;
            frameCount = 0;
        }
    }
}
