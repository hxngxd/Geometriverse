using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    public RectTransform rect;
    public TextMeshProUGUI text;
    public RectTransform LoadingImg;
    public float LoadingSpeed;
    void Start(){
        StartCoroutine(DelayShow("Chào mừng đến với Geometriverse!"));
    }
    public void Show(string msg){
        LoadingImg.gameObject.SetActive(false);
        rect.anchoredPosition = new Vector2(0, 0);
        LeanTween.cancel(rect);
        LeanTween.moveY(rect, -90f, 0.5f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.moveY(rect, 0, 0.25f).setEase(LeanTweenType.linear).setDelay(2.5f);
        text.text = msg;
    }
    public void Loading(string msg){
        LoadingImg.gameObject.SetActive(true);
        rect.anchoredPosition = new Vector2(0, -0);
        LeanTween.cancel(rect);
        LeanTween.moveY(rect, -90f, 0.5f).setEase(LeanTweenType.easeOutExpo);
        text.text = msg;
        StopAllCoroutines();
        StartCoroutine(Rotate());
        IEnumerator Rotate(){
            while (true){
                LoadingImg.Rotate(new Vector3(0, 0, Time.deltaTime * LoadingSpeed));
                yield return null;
            }
        }
    }
    public void Hide(){
        LeanTween.moveY(rect, 0, 0.25f).setEase(LeanTweenType.linear).setDelay(2.5f);
    }
    IEnumerator DelayShow(string msg){
        yield return new WaitForSeconds(.5f);
        Show(msg);
    }
}
