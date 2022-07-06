using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using INPUT=TMPro.TMP_InputField;
public class CameraController : MonoBehaviour
{
    Dictionary<string, List<INPUT>> Inputs = new Dictionary<string, List<INPUT>>();
    Dictionary<string, Slider> Sliders = new Dictionary<string, Slider>();
    Dictionary<string, Toggle> Toggles = new Dictionary<string, Toggle>();
    public Transform Centre;
    public Vector3 Cam_Rotation;
    public float Cam_Distance, RotateSpeed, ZoomSpeed, OrbitDampening, ZoomDampening, DragSpeed, MoveSpeed;
    public List<Transform> GridCameras;
    public Transform content;
    Draw draw;
    void Start()
    {
        draw = FindObjectOfType<Draw>();
        Inspector();
    }
    public void Inspector(){
        Sliders.Add("zoom", draw.uiobj.Slidr("Zoom", 0.1f, 20f, Cam_Distance, content));
        Sliders.Add("zoom_move", draw.uiobj.Slidr("Tốc độ zoom", 1f, 5f, ZoomSpeed, content));
        Sliders.Add("fov", draw.uiobj.Slidr("Tầm nhìn", 20f, 150f, Camera.main.fieldOfView, content));
        Sliders.Add("speed_move", draw.uiobj.Slidr("Tốc độ di chuyển", 1f, 100f, MoveSpeed, content));
        Sliders.Add("speed_drag", draw.uiobj.Slidr("Tốc độ kéo", 1f, 100f, DragSpeed, content));
        Sliders.Add("speed_rotate", draw.uiobj.Slidr("Tốc độ quay", 1f, 10f, RotateSpeed, content));
        Toggles.Add("orbit", draw.uiobj.Togle("Quay xung quanh tâm", false, content));
        Inputs.Add("pos", draw.uiobj.Vec3("Toạ độ", content));
        RealtimeInput();
        content.gameObject.SetActive(false);
    }
    public IEnumerator Okay(){
        content.gameObject.SetActive(true);
        draw.mouse.UnselectAll();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        Cancel();
    }
    public void RealtimeInput(){
        draw.listener.Add_Slider(Sliders["zoom"], () => {Cam_Distance = Sliders["zoom"].value;});
        draw.listener.Add_Slider(Sliders["zoom_move"], () => {ZoomSpeed = Sliders["zoom_move"].value;});
        draw.listener.Add_Slider(Sliders["fov"], () => {
            Camera.main.fieldOfView = Sliders["fov"].value;
            foreach (Transform cam in GridCameras){
                cam.GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
            }
        });
        draw.listener.Add_Slider(Sliders["speed_move"], () => {MoveSpeed = Sliders["speed_move"].value;});
        draw.listener.Add_Slider(Sliders["speed_drag"], () => {DragSpeed = Sliders["speed_drag"].value;});
        draw.listener.Add_Slider(Sliders["speed_rotate"], () => {RotateSpeed = Sliders["speed_rotate"].value;});
        draw.listener.Add_Inputs(Inputs["pos"], () => draw.inputhandler.Update_Position(this.gameObject, draw.inputhandler.Input2Vec(Inputs["pos"])));
    }
    public void Cancel(){
        content.gameObject.SetActive(false);
        draw.Cancel();
    }
    void Update()
    {
        Rotate();
        Move();
        Drag();
        Zoom();
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
        else{
            if (Toggles["orbit"].isOn){
                Cam_Rotation.x += RotateSpeed * 10 * Time.deltaTime;
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
            draw.inputhandler.Vec2Input(Inputs["pos"], draw.calc.swapYZ(this.transform.position));
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
        Cam_Distance = 10f;
    }
}
