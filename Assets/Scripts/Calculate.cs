using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class Calculate : MonoBehaviour
{
    void Start()
    {
        
    }
    public KeyValuePair<Vector3, Vector3> Duong_vuong_goc_chung(KeyValuePair<Vector3, Vector3> firstLine, KeyValuePair<Vector3, Vector3> secondLine){
        var u1 = firstLine.Value - firstLine.Key;
        var u2 = secondLine.Value - secondLine.Key;
        var A = DenseMatrix.OfArray(new double[,]{
            {-sqr(Vector3.Distance(Vector3.zero, u1)), Vector3.Dot(u1, u2)},
            {-Vector3.Dot(u1,u2), sqr(Vector3.Distance(Vector3.zero, u2))},
        });
        var B = DenseMatrix.OfArray(new double[,]{
            {- u1.x*(secondLine.Key.x - firstLine.Key.x) - u1.y*(secondLine.Key.y - firstLine.Key.y) - u1.z*(secondLine.Key.z - firstLine.Key.z)},
            {- u2.x*(secondLine.Key.x - firstLine.Key.x) - u2.y*(secondLine.Key.y - firstLine.Key.y) - u2.z*(secondLine.Key.z - firstLine.Key.z)},
        });
        var res = A.Inverse().Multiply(B);
        var t1 = (float)res.Row(0)[0];
        var t2 = (float)res.Row(1)[0];
        return new KeyValuePair<Vector3, Vector3>(new Vector3(firstLine.Key.x+u1.x*t1, firstLine.Key.y+u1.y*t1, firstLine.Key.z+u1.z*t1), new Vector3(secondLine.Key.x+u2.x*t2, secondLine.Key.y+u2.y*t2, secondLine.Key.z+u2.z*t2));
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
    public Vector3 KC_sang_toa_do(Vector3 center, Vector3 originPos, float dist){
        if (dist==0f) return originPos;
        if (Vector3.Distance(originPos, center)==0){
            originPos.x += 1;
            return KC_sang_toa_do(center, originPos, dist);
        }
        float k = dist/Vector3.Distance(originPos, center);
        return Dilation(center, originPos, k);
    }
    public Vector3 Translate(Vector3 center, Vector3 originPos){
        return originPos+center;
    }
    public Vector3 I_Translate(Vector3 center, Vector3 originPos){
        return originPos-center;
    }
    public Vector3 Dilation(Vector3 center, Vector3 originPos, float k){
        return new Vector3(k*(originPos.x-center.x) + center.x, k*(originPos.y-center.y) + center.y, k*(originPos.z-center.z) + center.z);
    }
    public  Vector3 I_Dilation(Vector3 center, Vector3 originPos, float k){
        return new Vector3((1f/k)*(originPos.x-center.x) + center.x, (1f/k)*(originPos.y-center.y) + center.y, (1f/k)*(originPos.z-center.z) + center.z);
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
