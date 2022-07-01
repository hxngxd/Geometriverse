using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
public class Calculate : MonoBehaviour
{
    public const float pi = Mathf.PI;
    void Start()
    {
        
    }
    public void PolygonVertex(LineRenderer line, Vector3 center, Vector3 vertex, int step, Matrix<double> RotationMatrix){
        line.positionCount = step;
        float radius = Vector3.Distance(center, vertex);
        for (int cur_step = 0;cur_step<step;cur_step++){
            float progress = (float)cur_step/step;
            float rad = progress * 2 * pi;
            float Xp = cos(rad), Yp = sin(rad);
            var origin = Translate(center, new Vector3(Xp, Yp, 0));
            var rotate = MatrixRotate(center, KC_sang_toa_do(center, origin, radius), RotationMatrix);
            line.SetPosition(cur_step, swapYZ(rotate));
        }
    }
    public Vector3 tam_duong_tron_ngoai_tiep(Vector3 A, Vector3 B, Vector3 C){
        var p = Vector3.Cross(B-A, C-A);
        var M = DenseMatrix.OfArray(new double[,]{
            {-2*(A.x-B.x), -2*(A.y-B.y), -2*(A.z-B.z)},
            {-2*(A.x-C.x), -2*(A.y-C.y), -2*(A.z-C.z)},
            {p.x, p.y, p.z}
        });
        var N = DenseMatrix.OfArray(new double[,]{
            {sqr(Vector3.Distance(Vector3.zero, B)) - sqr(Vector3.Distance(Vector3.zero, A))},
            {sqr(Vector3.Distance(Vector3.zero, C)) - sqr(Vector3.Distance(Vector3.zero, A))},
            {p.x*A.x + p.y*A.y + p.z*A.z}
        });
        return Mat2Vec(M.Inverse().Multiply(N).Transpose());
    } 
    public KeyValuePair<bool, Vector3> intersect_line_plane(KeyValuePair<Vector3, Vector3> Line, Dictionary<string, float> Plane){
        var M = Line.Key;
        var u = Line.Value - M;
        if (Vector3.Dot(u, new Vector3(Plane["a"], Plane["b"], Plane["c"]))==0) return new KeyValuePair<bool, Vector3>(false, Vector3.positiveInfinity);
        var N1 = Plane["a"]*M.x + Plane["b"]*M.y + Plane["c"]*M.z + Plane["d"];
        var N2 = Plane["a"]*u.x + Plane["b"]*u.y + Plane["c"]*u.z;
        var t = -(N1/N2);
        return new KeyValuePair<bool, Vector3>(true, new Vector3(M.x+u.x*t, M.y+u.y*t, M.z+u.z*t));
    }
    public Vector3 MatrixRotate(Vector3 center, Vector3 originPos, Matrix<double> RotationMatrix){
        originPos = I_Translate(center, originPos);
        var v = Vec2Mat(originPos).Transpose();
        return Translate(center, Mat2Vec(RotationMatrix.Multiply(v).Transpose()));
    }
    public Dictionary<string, float> plane_equation(Vector3 A, Vector3 B, Vector3 C){
        var equation = new Dictionary<string, float>();
        var perp = Vector3.Cross(B - A, C - A);
        equation.Add("a", perp.x);
        equation.Add("b", perp.y);
        equation.Add("c", perp.z);
        equation.Add("d", (-perp.x*A.x) + (-perp.y*A.y) + (-perp.z*A.z));
        return equation;
    }
    public Matrix<double> RM_Plane_XY(Vector3 A, Vector3 B, Vector3 C){
        var perp = Vector3.Cross(B - A, C - A).normalized;
        return RM_Vectors(Vector3.zero, new Vector3(0,0,1), perp);
    }
    public Matrix<double> RM_Vectors(Vector3 center, Vector3 A, Vector3 B){
        A = I_Translate(center, KC_sang_toa_do(center, A, 1f));
        B = I_Translate(center, KC_sang_toa_do(center, B, 1f));
        var c = Vector3.Dot(A, B);

        if (c==1) return Euler_to_RM(Vector3.zero);
        else if (c==-1) return Euler_to_RM(new Vector3(pi, 0, 0));

        var v = Vector3.Cross(A, B);
        var s = Vector3.Magnitude(v);
        var vx = DenseMatrix.OfArray(new double[,]{
            {0, -v.z, v.y},
            {v.z, 0, -v.x},
            {-v.y, v.x, 0}
        });
        var R = DenseMatrix.CreateIdentity(3) + vx + vx.Multiply(vx)*(1-c)/(sqr(s));
        return R;
    }
    public Matrix<double> RotationMatrixX(float angle){
        var Rx = DenseMatrix.OfArray(new double[,]{
            {1, 0, 0},
            {0, cos(angle), -sin(angle)},
            {0, sin(angle), cos(angle)}
        });
        return Rx;
    }
    public Matrix<double> RotationMatrixY(float angle){
        var Ry = DenseMatrix.OfArray(new double[,]{
            {cos(angle), 0, sin(angle)},
            {0, 1, 0},
            {-sin(angle), 0, cos(angle)}
        });
        return Ry;
    }
    public Matrix<double> RotationMatrixZ(float angle){
        var Rz = DenseMatrix.OfArray(new double[,]{
            {cos(angle), -sin(angle), 0},
            {sin(angle), cos(angle), 0},
            {0, 0, 1}
        });
        return Rz;
    }
    public Matrix<double> Euler_to_RM(Vector3 angle){
        var Rx = RotationMatrixX(angle.x);
        var Ry = RotationMatrixY(angle.y);
        var Rz = RotationMatrixZ(angle.z);
        var R = Rz.Multiply(Ry).Multiply(Rx);
        return R;
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
    public Vector3 roundVec3(Vector3 vec, float n){
        return new Vector3(fRound(vec.x, n), fRound(vec.y, n), fRound(vec.z, n));
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
    public Matrix<double> Vec2Mat(Vector3 vt){
        return DenseMatrix.OfArray(new double[,]{{vt.x, vt.y, vt.z}});
    }
    public Vector3 Mat2Vec(Matrix<double> M){
        return new Vector3((float)M.Row(0)[0], (float)M.Row(0)[1], (float)M.Row(0)[2]);
    }
}
