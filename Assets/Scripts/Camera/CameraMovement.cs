using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Video;

public class CameraMovement : MonoBehaviour
{
    public Transform pivot, mainCam;

    public float cam_distance, drag_speed;
    bool isDragging = false;

    public float move_speed, move_dampening_time;
    Vector3 move_velocity = Vector3.zero;
    bool isMoving = false;

    public float zoom_speed, zoom_dampening;

    public float orbit_speed, orbit_dampening;
    public Vector3 cam_rotation, target;

    void Start(){

    }
    void Update(){
        if (Input.GetMouseButtonDown(2)) isDragging = true;
        if (Input.GetMouseButtonUp(2)){
            isMoving = !isMoving;
            isDragging = false;
        }

        Move();
        Orbit();
        Zoom();
        Drag();
    }
    void Orbit(){
        if (Input.GetMouseButton(1) && !isDragging)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cam_rotation.x += Input.GetAxis("Mouse X") * orbit_speed;
                cam_rotation.y -= Input.GetAxis("Mouse Y") * orbit_speed;
            }
        }
        Quaternion QT = Quaternion.Euler(cam_rotation.y, cam_rotation.x, 0);
        pivot.rotation = Quaternion.Lerp(pivot.rotation, QT, Time.deltaTime * orbit_dampening);
    }
    void Move(){
        target = pivot.position;
        
        if (isMoving && !isDragging){
            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move += transform.forward * move_speed;
            if (Input.GetKey(KeyCode.S)) move -= transform.forward * move_speed;
            if (Input.GetKey(KeyCode.D)) move += transform.right * move_speed;
            if (Input.GetKey(KeyCode.A)) move -= transform.right * move_speed;
            if (Input.GetKey(KeyCode.E)) move += transform.up * move_speed;
            if (Input.GetKey(KeyCode.Q)) move -= transform.up * move_speed;

            target = pivot.position + move;
        }

        pivot.position = Vector3.SmoothDamp(pivot.position, target, ref move_velocity, move_dampening_time);
    }
    void Zoom(){
        if (mainCam.localPosition.z != cam_distance * -1f){
            mainCam.localPosition = new Vector3(0f, 0f, Mathf.Lerp(mainCam.localPosition.z, cam_distance * -1f, Time.deltaTime * zoom_dampening));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f && !isDragging)
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel");
            cam_distance *= 1 - (Mathf.Sign(ScrollAmount) * zoom_speed * 0.01f);
            cam_distance = Mathf.Clamp(cam_distance, .1f, 20f);
        }
    }
    void Drag(){
        if (Input.GetMouseButton(2))
        {
            float xdrag = Input.GetAxisRaw("Mouse X");
            float ydrag = Input.GetAxisRaw("Mouse Y");

            transform.Translate(-xdrag * Time.deltaTime * drag_speed, -ydrag * Time.deltaTime * drag_speed, 0);
        }
    }

}