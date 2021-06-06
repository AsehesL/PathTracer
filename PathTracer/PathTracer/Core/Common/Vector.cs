using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Vector2
    {
        public double x, y;

        public double magnitude
        {
            get { return System.Math.Sqrt(x * x + y * y); }
        }

        public double sqrMagnitude
        {
            get { return x * x + y * y; }
        }

        public Vector2 normalized
        {
            get
            {
                double invm = 1.0 / this.magnitude;
                return new Vector2(x * invm, y * invm);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                throw new System.IndexOutOfRangeException();
            }
        }

        public static Vector2 one
        {
            get { return new Vector2(1, 1); }
        }

        public static Vector2 zero
        {
            get { return new Vector2(0, 0); }
        }

        public static Vector2 right
        {
            get { return new Vector2(1, 0); }
        }

        public static Vector2 left
        {
            get { return new Vector2(-1, 0); }
        }

        public static Vector2 up
        {
            get { return new Vector2(0, 1); }
        }

        public static Vector2 down
        {
            get { return new Vector2(0, -1); }
        }

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void Normalize()
        {
            double invm = 1.0 / this.magnitude;
            this.x *= invm;
            this.y *= invm;
        }

        public void Scale(Vector2 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return false;
            if (v2.x - v1.x > double.Epsilon)
                return false;
            if (v1.y - v2.y > double.Epsilon)
                return false;
            if (v2.y - v1.y > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return true;
            if (v2.x - v1.x > double.Epsilon)
                return true;
            if (v1.y - v2.y > double.Epsilon)
                return true;
            if (v2.y - v1.y > double.Epsilon)
                return true;
            return false;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2 operator *(Vector2 v, double f)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator *(double f, Vector2 v)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator /(Vector2 v, double f)
        {
            double invf = 1.0 / f;
            return new Vector2(v.x * invf, v.y * invf);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static Vector2 Reflect(Vector2 i, Vector2 n)
        {
            return i - 2.0*Vector2.Dot(n, i)*n;
        }

        public static Vector2 Refract(Vector2 i, Vector2 n, double eta)
        {
            double cosi = Vector2.Dot(-1.0 * i, n);
            double cost2 = 1.0 - eta * eta * (1.0 - cosi * cosi);
            Vector2 t = eta * i + ((eta * cosi - Math.Sqrt(Math.Abs(cost2))) * n);
            double v = cost2 > 0 ? 1.0 : 0.0;
            return new Vector2(v * t.x, v * t.y);
        }

        public static double Angle(Vector2 from, Vector2 to)
        {
            Vector2 fn = from.normalized;
            Vector2 tn = to.normalized;
            double v = Vector2.Dot(fn, tn);
            if (v < -1.0)
                v = -1.0;
            else if (v > 1.0)
                v = 1.0;
            return Math.Acos(v) * 57.29578;
        }

        public static double Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static double Distance(Vector2 lhs, Vector2 rhs)
        {
            return (lhs - rhs).magnitude;
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        public override bool Equals(object obj)
        {
            var objVector = (Vector2) obj;
            return objVector.x == this.x && objVector.y == this.y;
        }

        public override string ToString()
        {
            return $"Vector2({this.x},{this.y})";
        }

        public static Vector2 Parse(string text)
        {
            string[] split = text.Split(',');

            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;

            return new Vector2(x, y);
        }
    }

    public struct Vector3
    {
        public double x, y, z;

        public double magnitude
        {
            get { return System.Math.Sqrt(x * x + y * y + z * z); }
        }

        public double sqrMagnitude
        {
            get { return x * x + y * y + z * z; }
        }

        public Vector3 normalized
        {
            get
            {
                double invm = 1.0 / this.magnitude;
                return new Vector3(x * invm, y * invm, z * invm);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else if (index == 2)
                    return z;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else if (index == 2)
                    z = value;
                else
                    throw new System.IndexOutOfRangeException();
            }
        }

        public static Vector3 one
        {
            get { return new Vector3(1, 1, 1); }
        }

        public static Vector3 zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        public static Vector3 right
        {
            get { return new Vector3(1, 0, 0); }
        } 

        public  static Vector3 left
        {
            get { return new Vector3(-1, 0, 0); }
        }

        public  static Vector3 up
        {
            get { return new Vector3(0, 1, 0); }
        }

        public  static  Vector3 down
        {
            get { return new Vector3(0, -1, 0); }
        }

        public  static Vector3 forward
        {
            get { return new Vector3(0, 0, 1); }
        }

        public static Vector3 back
        {
            get { return new Vector3(0, 0, -1); }
        }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Normalize()
        {
            double invm = 1.0 / this.magnitude;
            this.x *= invm;
            this.y *= invm;
            this.z *= invm;
        }

        public void Scale(Vector3 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public bool IsZero()
        {
            if (x < -double.Epsilon || x > double.Epsilon) return false;
            if (y < -double.Epsilon || y > double.Epsilon) return false;
            if (z < -double.Epsilon || z > double.Epsilon) return false;
            return true;
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return false;
            if (v2.x - v1.x > double.Epsilon)
                return false;
            if (v1.y - v2.y > double.Epsilon)
                return false;
            if (v2.y - v1.y > double.Epsilon)
                return false;
            if (v1.z - v2.z > double.Epsilon)
                return false;
            if (v2.z - v1.z > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return true;
            if (v2.x - v1.x > double.Epsilon)
                return true;
            if (v1.y - v2.y > double.Epsilon)
                return true;
            if (v2.y - v1.y > double.Epsilon)
                return true;
            if (v1.z - v2.z > double.Epsilon)
                return true;
            if (v2.z - v1.z > double.Epsilon)
                return true;
            return false;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector3 operator *(Vector3 v, double f)
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }

        public static Vector3 operator *(double f, Vector3 v)
        {
            return new Vector3(v.x * f, v.y * f, v.z * f);
        }

        public static Vector3 operator /(Vector3 v, double f)
        {
            double invf = 1.0 / f;
            return new Vector3(v.x * invf, v.y * invf, v.z * invf);
        }

        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, double t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return 2.0 * Vector3.Dot(n, i) * n - i;
        }

        public static bool Refract(Vector3 i, Vector3 n, double eta, out Vector3 result)
        {
            double cosi = Vector3.Dot(-1.0*i, n);
            double cost2 = 1.0 - eta * eta * (1.0 - cosi * cosi);
            if (cost2 > 0)
            {
                result = eta * i + ((eta * cosi - Math.Sqrt(Math.Abs(cost2))) * n);
                return true;
            }

            result = default(Vector3);
            return false;
        }

        public static Vector3 ONB(Vector3 normal, Vector3 direction)
        {
            Vector3 w = normal;
            Vector3 u = Vector3.Cross(new Vector3(0.00424f, 1, 0.00764f), w);
            u.Normalize();
            Vector3 v = Vector3.Cross(u, w);
            Vector3 l = direction.x * u + direction.y * v + direction.z * w;
            //if (Vector3.Dot(l, normal) < 0.0)
            //    l = -direction.x * u - direction.y * v - direction.z * w;
            l.Normalize();
            return l;
        }

        public static double Angle(Vector3 from, Vector3 to)
        {
            Vector3 fn = from.normalized;
            Vector3 tn = to.normalized;
            double v = Vector3.Dot(fn, tn);
            if (v < -1.0)
                v = -1.0;
            else if (v > 1.0)
                v = 1.0;
            return Math.Acos(v) * 57.29578;
        }

        public static double Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public static double Distance(Vector3 lhs, Vector3 rhs)
        {
            return (lhs - rhs).magnitude;
        }

        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        public static Vector3 Project(Vector3 vector, Vector3 normal)
        {
            double n = Vector3.Dot(normal, normal);
            Vector3 result = default(Vector3);
            if (n >= double.Epsilon)
                result = normal*Vector3.Dot(vector, normal)/n;
            return result;
        }

        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            Vector3 pj = Vector3.Project(vector, planeNormal);
            return vector - pj;
        }

        public override bool Equals(object obj)
        {
            var objVector = (Vector3)obj;
            return objVector.x == this.x && objVector.y == this.y && objVector.z == this.z;
        }

        public override string ToString()
        {
            return $"Vector3({this.x},{this.y},{this.z})";
        }

        public static Vector3 Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector3.zero;
            string[] split = text.Split(',');

            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;
            double z = split.Length >= 3 ? double.Parse(split[2]) : 0.0;

            return new Vector3(x, y, z);
        }
    }

    public struct Vector4
    {
        public double x, y, z, w;

        public double magnitude
        {
            get { return System.Math.Sqrt(x * x + y * y + z * z + w * w); }
        }

        public double sqrMagnitude
        {
            get { return x * x + y * y + z * z + w * w; }
        }

        public Vector3 xyz
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        public Vector4 normalized
        {
            get
            {
                double invm = 1.0 / this.magnitude;
                return new Vector4(x * invm, y * invm, z * invm, w * invm);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else if (index == 2)
                    return z;
                else if (index == 3)
                    return w;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else if (index == 2)
                    z = value;
                else if (index == 3)
                    w = value;
                else
                    throw new System.IndexOutOfRangeException();
            }
        }

        public Vector4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Normalize()
        {
            double invm = 1.0 / this.magnitude;
            this.x *= invm;
            this.y *= invm;
            this.z *= invm;
            this.w *= invm;
        }

        public void Scale(Vector4 scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
            this.w *= scale.w;
        }

        public static bool operator ==(Vector4 v1, Vector4 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return false;
            if (v2.x - v1.x > double.Epsilon)
                return false;
            if (v1.y - v2.y > double.Epsilon)
                return false;
            if (v2.y - v1.y > double.Epsilon)
                return false;
            if (v1.z - v2.z > double.Epsilon)
                return false;
            if (v2.z - v1.z > double.Epsilon)
                return false;
            if (v1.w - v2.w > double.Epsilon)
                return false;
            if (v2.w - v1.w > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Vector4 v1, Vector4 v2)
        {
            if (v1.x - v2.x > double.Epsilon)
                return true;
            if (v2.x - v1.x > double.Epsilon)
                return true;
            if (v1.y - v2.y > double.Epsilon)
                return true;
            if (v2.y - v1.y > double.Epsilon)
                return true;
            if (v1.z - v2.z > double.Epsilon)
                return true;
            if (v2.z - v1.z > double.Epsilon)
                return true;
            if (v1.w - v2.w > double.Epsilon)
                return true;
            if (v2.w - v1.w > double.Epsilon)
                return true;
            return false;
        }

        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        public static Vector4 operator *(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }

        public static Vector4 operator *(Vector4 v, double f)
        {
            return new Vector4(v.x * f, v.y * f, v.z * f, v.w * f);
        }

        public static Vector4 operator *(double f, Vector4 v)
        {
            return new Vector4(v.x * f, v.y * f, v.z * f, v.w * f);
        }

        public static Vector4 operator /(Vector4 v, double f)
        {
            double invf = 1.0 / f;
            return new Vector4(v.x * invf, v.y * invf, v.z * invf, v.w * invf);
        }

        public static Vector4 operator /(Vector4 v1, Vector4 v2)
        {
            return new Vector4(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z, v1.w / v2.w);
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, double t)
        {
            return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
        }

        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));
        }

        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));
        }

        public override bool Equals(object obj)
        {
            var objVector = (Vector4)obj;
            return objVector.x == this.x && objVector.y == this.y && objVector.z == this.z && objVector.w == this.w;
        }

        public override string ToString()
        {
            return $"Vector4({this.x},{this.y},{this.z},{this.w})";
        }

        public static Vector4 Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Vector4();
            string[] split = text.Split(',');

            double x = split.Length >= 1 ? double.Parse(split[0]) : 0.0;
            double y = split.Length >= 2 ? double.Parse(split[1]) : 0.0;
            double z = split.Length >= 3 ? double.Parse(split[2]) : 0.0;
            double w = split.Length >= 4 ? double.Parse(split[3]) : 0.0;

            return new Vector4(x, y, z, w);
        }
    }
}
