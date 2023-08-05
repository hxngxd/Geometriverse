using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class CameraController : MonoBehaviour
{
    public Transform Centre;
    public Vector3 Cam_Rotation;
    public float Cam_Distance, RotateSpeed, ZoomSpeed, OrbitDampening, ZoomDampening, DragSpeed, MoveSpeed;
    public List<Transform> GridCameras;
    Draw draw;
    bool startingAnimation = false;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        foreach (Transform cam in GridCameras){
            LeanTween.moveLocalZ(cam.gameObject, -Cam_Distance, 2f).setEase(LeanTweenType.easeOutQuint);
        }
        LeanTween.moveLocalZ(Camera.main.gameObject, -Cam_Distance, 2f).setEase(LeanTweenType.easeOutQuint);
        IEnumerator wait(){
            yield return new WaitForSeconds(2f);
            startingAnimation = true;
        }
        StartCoroutine(wait());
    }
    void Update()
    {
        if (startingAnimation){
            Move();
            Drag();
            Rotate();
            Zoom();
        }
    }
    void Rotate(){
        if (Input.GetMouseButton(1))
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                Cam_Rotation.x += Input.GetAxis("Mouse X") * RotateSpeed;
                Cam_Rotation.y -= Input.GetAxis("Mouse Y") * RotateSpeed;
            }
        }
        Quaternion QT = Quaternion.Euler(Cam_Rotation.y, Cam_Rotation.x, 0);
        Centre.rotation = Quaternion.Lerp(Centre.rotation, QT, Time.deltaTime * OrbitDampening);
    }

    void Move(){
        if (Input.GetMouseButton(1) && !draw.raycast.isMouseOverUI())
        {
            float speed = MoveSpeed/200f;
            Vector3 move = Vector3.zero;
            if(Input.GetKey(KeyCode.W)) move += Vector3.forward * speed;
            if (Input.GetKey(KeyCode.S)) move -= Vector3.forward * speed;
            if (Input.GetKey(KeyCode.D)) move += Vector3.right * speed;
            if (Input.GetKey(KeyCode.A)) move -= Vector3.right * speed;
            if (Input.GetKey(KeyCode.E)) move += Vector3.up * speed;
            if (Input.GetKey(KeyCode.Q)) move -= Vector3.up * speed;
            Centre.Translate(move * Time.deltaTime * MoveSpeed);
        }
    }

    void Zoom(){
        if (Camera.main.transform.localPosition.z != this.Cam_Distance * -1f)
        {
            Camera.main.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Camera.main.transform.localPosition.z, Cam_Distance * -1f, Time.deltaTime * ZoomDampening));
            foreach (Transform cam in GridCameras){
                cam.transform.localPosition = Camera.main.transform.localPosition;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f && !draw.raycast.isMouseOverUI())
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ZoomDampening;
            ScrollAmount *= (Cam_Distance * 0.3f);
            Cam_Distance = Cam_Distance - Mathf.Sign(ScrollAmount)*Cam_Distance*15f/100f;
            Cam_Distance = Mathf.Clamp(Cam_Distance, .1f, 20f);
        }
    }
    void Drag(){
        if (Input.GetMouseButton(2) && !draw.raycast.isMouseOverUI())
        {
            this.transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * DragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * DragSpeed, 0);
        }
    }
    public void ResetCamera(){
        Cam_Rotation = new Vector3(-45,30,0);
        this.transform.position = Vector3.zero;
        Cam_Distance = 11f;
    }
}
