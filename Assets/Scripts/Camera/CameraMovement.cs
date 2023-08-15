using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform pivot, mainCam;

    public float cam_distance, max_distance_from_pivot;

    public float drag_speed, drag_dampening;

    public float move_speed, move_dampening_time;
    Vector3 move_velocity = Vector3.zero;
    bool isMoving = false;

    public float zoom_speed, zoom_dampening;

    public float orbit_speed, orbit_dampening, dragX=0, dragY=0;
    public Vector3 cam_rotation, target;

    void Start(){

    }
    void Update(){
        if (Input.GetMouseButtonUp(2)) isMoving = !isMoving;

        Move();
        Orbit();
        Zoom();
        Drag();
    }
    void Orbit(){
        if (Input.GetMouseButton(1) && !isDragging())
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseX != 0 || mouseY != 0)
            {
                if (Math.Abs(mouseX) > 1f/orbit_speed) mouseX = Mathf.Sign(mouseX) * 1f/orbit_speed;
                if (Math.Abs(mouseY) > 1f/orbit_speed) mouseY = Mathf.Sign(mouseY) * 1f/orbit_speed;
                cam_rotation.x += mouseX * orbit_speed;
                cam_rotation.y -= mouseY * orbit_speed * 0.5f;
            }
        }
        Quaternion QT = Quaternion.Euler(cam_rotation.y, cam_rotation.x, 0);
        pivot.rotation = Quaternion.Lerp(pivot.rotation, QT, Time.deltaTime * orbit_dampening);
    }
    void Move(){
        target = pivot.position;
        
        if (isMoving && !isDragging()){
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

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel");
            cam_distance *= 1 - (Mathf.Sign(ScrollAmount) * zoom_speed * 0.01f);
            cam_distance = Mathf.Clamp(cam_distance, .1f, max_distance_from_pivot);
        }
    }
    void Drag(){
        dragX = Mathf.Lerp(dragX, isDragging() ? Input.GetAxis("Mouse X") : 0, Time.deltaTime * drag_dampening);
        dragY = Mathf.Lerp(dragY, isDragging() ? Input.GetAxis("Mouse Y") : 0, Time.deltaTime * drag_dampening);
        transform.Translate(-dragX * Time.deltaTime * drag_speed, -dragY * Time.deltaTime * drag_speed, 0);
    }
    bool isDragging(){
        return Input.GetKey(KeyCode.Space) && Input.GetMouseButton(0);
    }

}