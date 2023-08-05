using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class RaycastHandler : MonoBehaviour
{
    public struct MouseHit{
        public string ID;
        public Transform transform;
        public Vector3 point;
    }
    [SerializeField] EventSystem m_EventSystem;
    Calculate calc;
    void Start(){
        calc = FindObjectOfType<Calculate>();
    }
    public Transform GetUI(RectTransform canvasRect)
    {
        var m_Raycaster = canvasRect.GetComponent<GraphicRaycaster>();
        PointerEventData m_PointerEventData;
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        return (results.Count==0 ? null : results[0].gameObject.transform);
    }
    public bool isMouseOverUI(){
        return EventSystem.current.IsPointerOverGameObject();
    }
    public KeyValuePair<Vector3, Vector3> MouseToRay(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return new KeyValuePair<Vector3, Vector3>(calc.ztoy(ray.origin), calc.ztoy(ray.GetPoint(1)));
    }
    public MouseHit Hit(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var result = new MouseHit();
        if (Physics.Raycast(ray, out hit)){
            result.ID = hit.collider.name;
            result.transform = hit.collider.transform;
            result.point = hit.point;
        }
        if (result.ID == null){
            result.transform = GameObject.Find("Background").transform;
            result.ID = "Background";
        }
        return result;
    }
}
