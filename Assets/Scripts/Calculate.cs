using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate : MonoBehaviour
{
    void Start()
    {
        
    }
    //Hinh chieu
    public Vector3 HC_diem_len_duong_thang(Vector3 A, KeyValuePair<Vector3, Vector3> Line){
        var N = Line.Value;
        var M = Line.Key;
        var u = N-M;
        float t = -((M.x-A.x)*u.x + (M.y-A.y)*u.y + (M.z-A.z)*u.z)/(sqr(u.x) + sqr(u.y) + sqr(u.z));
        return new Vector3(M.x+u.x*t, M.y+u.y*t, M.z+u.z*t);
    }
    //Khoang cach
    public float KC_diem_duong_thang(Vector3 A, KeyValuePair<Vector3, Vector3> Line){
        return Vector3.Distance(A, HC_diem_len_duong_thang(A, Line));
    }
    public float sin(float rad){
        return Mathf.Sin(rad);
    }
    public float cos(float rad){
        return Mathf.Cos(rad);
    }
    public float tan(float rad){
        return Mathf.Tan(rad);
    }
    public float asin(float rad){
        return Mathf.Asin(rad);
    }
    public float acos(float rad){
        return Mathf.Acos(rad);
    }
    public float atan(float rad){
        return Mathf.Atan(rad);
    }
    public float atan2(float y, float x){
        return Mathf.Atan2(y, x);
    }
    public float fRound(float p, float n){
        return Mathf.Round(p * pow(10, n))/pow(10, n);
    }
    public float sqr(float x){
        return x*x;
    }
    public float sqrt(float x){
        return Mathf.Sqrt(x);
    }
    public float pow(float x, float n){
        return Mathf.Pow(x, n);
    }
    public float toRad(float angle){
        return angle * Mathf.Deg2Rad;
    }
    public float toDeg(float rad){
        return rad * Mathf.Rad2Deg;
    }
    public Vector3 Vec2Rad(Vector3 deg){
        return new Vector3(toRad(deg.x), toRad(deg.y), toRad(deg.z));
    }
    public Vector3 Vec2Deg(Vector3 rad){
        return new Vector3(toDeg(rad.x), toDeg(rad.y), toDeg(rad.z));
    }
    public Vector3 swapYZ(Vector3 vt){
        return new Vector3(vt.x, vt.z, vt.y);
    }
    public Vector3 swapXY(Vector3 vt){
        return new Vector3(vt.y, vt.x, vt.z);
    }
}
