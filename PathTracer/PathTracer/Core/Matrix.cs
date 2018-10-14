using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public struct Matrix
    {
        public double m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33;

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return m00;
                if (index == 1)
                    return m01;
                if (index == 2)
                    return m02;
                if (index == 3)
                    return m03;
                if (index == 4)
                    return m10;
                if (index == 5)
                    return m11;
                if (index == 6)
                    return m12;
                if (index == 7)
                    return m13;
                if (index == 8)
                    return m20;
                if (index == 9)
                    return m21;
                if (index == 10)
                    return m22;
                if (index == 11)
                    return m23;
                if (index == 12)
                    return m30;
                if (index == 13)
                    return m31;
                if (index == 14)
                    return m32;
                if (index == 15)
                    return m33;
                throw new System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    m00 = value;
                if (index == 1)
                    m01 = value;
                if (index == 2)
                    m02 = value;
                if (index == 3)
                    m03 = value;
                if (index == 4)
                    m10 = value;
                if (index == 5)
                    m11 = value;
                if (index == 6)
                    m12 = value;
                if (index == 7)
                    m13 = value;
                if (index == 8)
                    m20 = value;
                if (index == 9)
                    m21 = value;
                if (index == 10)
                    m22 = value;
                if (index == 11)
                    m23 = value;
                if (index == 12)
                    m30 = value;
                if (index == 13)
                    m31 = value;
                if (index == 14)
                    m32 = value;
                if (index == 15)
                    m33 = value;
                throw new System.IndexOutOfRangeException();
            }
        }

        public Matrix transpose
        {
            get
            {
                Matrix matrix = default(Matrix);
                matrix.m00 = m00;
                matrix.m01 = m10;
                matrix.m02 = m20;
                matrix.m03 = m30;

                matrix.m10 = m01;
                matrix.m11 = m11;
                matrix.m12 = m21;
                matrix.m13 = m31;

                matrix.m20 = m02;
                matrix.m21 = m12;
                matrix.m22 = m22;
                matrix.m23 = m32;

                matrix.m30 = m03;
                matrix.m31 = m13;
                matrix.m32 = m23;
                matrix.m33 = m33;
                return matrix;
            }
        }

        public Matrix inverse
        {
            get
            {
                Matrix mat = this;
                mat.Inverse();
                return mat;
            }
        }

        public Matrix(double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m03 = m03;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m30 = m30;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.m00 - b.m00 > double.Epsilon || b.m00 - a.m00 > double.Epsilon)
                return false;
            if (a.m01 - b.m01 > double.Epsilon || b.m01 - a.m01 > double.Epsilon)
                return false;
            if (a.m02 - b.m02 > double.Epsilon || b.m02 - a.m02 > double.Epsilon)
                return false;
            if (a.m03 - b.m03 > double.Epsilon || b.m03 - a.m03 > double.Epsilon)
                return false;
            if (a.m10 - b.m10 > double.Epsilon || b.m10 - a.m10 > double.Epsilon)
                return false;
            if (a.m11 - b.m11 > double.Epsilon || b.m11 - a.m11 > double.Epsilon)
                return false;
            if (a.m12 - b.m12 > double.Epsilon || b.m12 - a.m12 > double.Epsilon)
                return false;
            if (a.m13 - b.m13 > double.Epsilon || b.m13 - a.m13 > double.Epsilon)
                return false;
            if (a.m20 - b.m20 > double.Epsilon || b.m20 - a.m20 > double.Epsilon)
                return false;
            if (a.m21 - b.m21 > double.Epsilon || b.m21 - a.m21 > double.Epsilon)
                return false;
            if (a.m22 - b.m22 > double.Epsilon || b.m22 - a.m22 > double.Epsilon)
                return false;
            if (a.m23 - b.m23 > double.Epsilon || b.m23 - a.m23 > double.Epsilon)
                return false;
            if (a.m30 - b.m30 > double.Epsilon || b.m30 - a.m30 > double.Epsilon)
                return false;
            if (a.m31 - b.m31 > double.Epsilon || b.m31 - a.m31 > double.Epsilon)
                return false;
            if (a.m32 - b.m32 > double.Epsilon || b.m32 - a.m32 > double.Epsilon)
                return false;
            if (a.m33 - b.m33 > double.Epsilon || b.m33 - a.m33 > double.Epsilon)
                return false;
            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            if (a.m00 - b.m00 > double.Epsilon || b.m00 - a.m00 > double.Epsilon)
                return true;
            if (a.m01 - b.m01 > double.Epsilon || b.m01 - a.m01 > double.Epsilon)
                return true;
            if (a.m02 - b.m02 > double.Epsilon || b.m02 - a.m02 > double.Epsilon)
                return true;
            if (a.m03 - b.m03 > double.Epsilon || b.m03 - a.m03 > double.Epsilon)
                return true;
            if (a.m10 - b.m10 > double.Epsilon || b.m10 - a.m10 > double.Epsilon)
                return true;
            if (a.m11 - b.m11 > double.Epsilon || b.m11 - a.m11 > double.Epsilon)
                return true;
            if (a.m12 - b.m12 > double.Epsilon || b.m12 - a.m12 > double.Epsilon)
                return true;
            if (a.m13 - b.m13 > double.Epsilon || b.m13 - a.m13 > double.Epsilon)
                return true;
            if (a.m20 - b.m20 > double.Epsilon || b.m20 - a.m20 > double.Epsilon)
                return true;
            if (a.m21 - b.m21 > double.Epsilon || b.m21 - a.m21 > double.Epsilon)
                return true;
            if (a.m22 - b.m22 > double.Epsilon || b.m22 - a.m22 > double.Epsilon)
                return true;
            if (a.m23 - b.m23 > double.Epsilon || b.m23 - a.m23 > double.Epsilon)
                return true;
            if (a.m30 - b.m30 > double.Epsilon || b.m30 - a.m30 > double.Epsilon)
                return true;
            if (a.m31 - b.m31 > double.Epsilon || b.m31 - a.m31 > double.Epsilon)
                return true;
            if (a.m32 - b.m32 > double.Epsilon || b.m32 - a.m32 > double.Epsilon)
                return true;
            if (a.m33 - b.m33 > double.Epsilon || b.m33 - a.m33 > double.Epsilon)
                return true;
            return false;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            double rm00 = a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20 + a.m03 * b.m30;
            double rm01 = a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21 + a.m03 * b.m31;
            double rm02 = a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22 + a.m03 * b.m32;
            double rm03 = a.m00 * b.m03 + a.m01 * b.m13 + a.m02 * b.m23 + a.m03 * b.m33;

            double rm10 = a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20 + a.m13 * b.m30;
            double rm11 = a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
            double rm12 = a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
            double rm13 = a.m10 * b.m03 + a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;

            double rm20 = a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20 + a.m23 * b.m30;
            double rm21 = a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
            double rm22 = a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
            double rm23 = a.m20 * b.m03 + a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;

            double rm30 = a.m30 * b.m00 + a.m31 * b.m10 + a.m32 * b.m20 + a.m33 * b.m30;
            double rm31 = a.m30 * b.m01 + a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
            double rm32 = a.m30 * b.m02 + a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
            double rm33 = a.m30 * b.m03 + a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;

            return new Matrix(rm00, rm01, rm02, rm03,
                rm10, rm11, rm12, rm13,
                rm20, rm21, rm22, rm23,
                rm30, rm31, rm32, rm33);
        }

        public Vector3 TransformVector(Vector3 vector)
        {
            Vector3 result = default(Vector3);
            double x = vector.x * m00 + vector.y * m10 + vector.z * m20;
            double y = vector.x * m01 + vector.y * m11 + vector.z * m21;
            double z = vector.x * m02 + vector.y * m12 + vector.z * m22;
            
            return new Vector3(x, y, z);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            double x = point.x * m00 + point.y * m10 + point.z * m20 + m30;
            double y = point.x * m01 + point.y * m11 + point.z * m21 + m31;
            double z = point.x * m02 + point.y * m12 + point.z * m22 + m32;
            double w = point.x * m03 + point.y * m13 + point.z * m23 + m33;

            if (w >= 0.0 - double.Epsilon && w <= 0.0 + double.Epsilon )
            {
                return new Vector3(Double.NaN, Double.NaN, Double.NaN);
            }

            return new Vector3(x / w, y / w, z / w);
        }

        public void Transpose()
        {
            double t01 = m01;
            m01 = m10;
            m10 = t01;

            double t02 = m02;
            m02 = m20;
            m20 = t02;

            double t03 = m03;
            m03 = m30;
            m30 = t03;

            double t12 = m12;
            m12 = m21;
            m21 = t12;

            double t13 = m13;
            m13 = m31;
            m31 = t13;

            double t23 = m23;
            m23 = m32;
            m32 = t23;
        }

        public void Inverse()
        {
            Matrix tmp = default(Matrix);

            tmp[0] = m11 * m22 * m33 -
                m11 * m23 * m32 -
                m21 * m12 * m33 +
                m21 * m13 * m32 +
                m31 * m12 * m23 -
                m31 * m13 * m22;

            tmp[4] = -m10 * m22 * m33 +
                m10 * m23 * m32 +
                m20 * m12 * m33 -
                m20 * m13 * m32 -
                m30 * m12 * m23 +
                m30 * m13 * m22;

            tmp[8] = m10 * m21 * m33 -
                m10 * m23 * m31 -
                m20 * m11 * m33 +
                m20 * m13 * m31 +
                m30 * m11 * m23 -
                m30 * m13 * m21;

            tmp[12] = -m10 * m21 * m32 +
                m10 * m22 * m31 +
                m20 * m11 * m32 -
                m20 * m12 * m31 -
                m30 * m11 * m22 +
                m30 * m12 * m21;

            tmp[1] = -m01 * m22 * m33 +
                m01 * m23 * m32 +
                m21 * m02 * m33 -
                m21 * m03 * m32 -
                m31 * m02 * m23 +
                m31 * m03 * m22;

            tmp[5] = m00 * m22 * m33 -
                m00 * m23 * m32 -
                m20 * m02 * m33 +
                m20 * m03 * m32 +
                m30 * m02 * m23 -
                m30 * m03 * m22;

            tmp[9] = -m00 * m21 * m33 +
                m00 * m23 * m31 +
                m20 * m01 * m33 -
                m20 * m03 * m31 -
                m30 * m01 * m23 +
                m30 * m03 * m21;

            tmp[13] = m00 * m21 * m32 -
                m00 * m22 * m31 -
                m20 * m01 * m32 +
                m20 * m02 * m31 +
                m30 * m01 * m22 -
                m30 * m02 * m21;

            tmp[2] = m01 * m12 * m33 -
                m01 * m13 * m32 -
                m11 * m02 * m33 +
                m11 * m03 * m32 +
                m31 * m02 * m13 -
                m31 * m03 * m12;

            tmp[6] = -m00 * m12 * m33 +
                m00 * m13 * m32 +
                m10 * m02 * m33 -
                m10 * m03 * m32 -
                m30 * m02 * m13 +
                m30 * m03 * m12;

            tmp[10] = m00 * m11 * m33 -
                m00 * m13 * m31 -
                m10 * m01 * m33 +
                m10 * m03 * m31 +
                m30 * m01 * m13 -
                m30 * m03 * m11;

            tmp[14] = -m00 * m11 * m32 +
                m00 * m12 * m31 +
                m10 * m01 * m32 -
                m10 * m02 * m31 -
                m30 * m01 * m12 +
                m30 * m02 * m11;

            tmp[3] = -m01 * m12 * m23 +
                m01 * m13 * m22 +
                m11 * m02 * m23 -
                m11 * m03 * m22 -
                m21 * m02 * m13 +
                m21 * m03 * m12;

            tmp[7] = m00 * m12 * m23 -
                m00 * m13 * m22 -
                m10 * m02 * m23 +
                m10 * m03 * m22 +
                m20 * m02 * m13 -
                m20 * m03 * m12;

            tmp[11] = -m00 * m11 * m23 +
                m00 * m13 * m21 +
                m10 * m01 * m23 -
                m10 * m03 * m21 -
                m20 * m01 * m13 +
                m20 * m03 * m11;

            tmp[15] = m00 * m11 * m22 -
                m00 * m12 * m21 -
                m10 * m01 * m22 +
                m10 * m02 * m21 +
                m20 * m01 * m12 -
                m20 * m02 * m11;

            double det = m00 * tmp[0] + m01 * tmp[4] + m02 * tmp[8] + m03 * tmp[12];

            if (det == 0)
                throw new System.Exception("不可逆矩阵");

            det = 1.0 / det;

            for (int i = 0; i < 16; i++)
                this[i] = tmp[i] * det;
        }


        public static Matrix Identity()
        {
            Matrix matrix = default(Matrix);
            matrix.m00 = 1;
            matrix.m01 = 0;
            matrix.m02 = 0;
            matrix.m03 = 0;
            matrix.m10 = 0;
            matrix.m11 = 1;
            matrix.m12 = 0;
            matrix.m13 = 0;
            matrix.m20 = 0;
            matrix.m21 = 0;
            matrix.m22 = 1;
            matrix.m23 = 0;
            matrix.m30 = 0;
            matrix.m31 = 0;
            matrix.m32 = 0;
            matrix.m33 = 1;
            return matrix;
        }

        public static Matrix TRS(Vector3 position, Vector3 euler, Vector3 scale)
        {
            double cosx = Math.Cos(euler.x * 0.01745329252 * 0.5);
            double cosy = Math.Cos(euler.y * 0.01745329252 * 0.5);
            double cosz = Math.Cos(euler.z * 0.01745329252 * 0.5);

            double sinx = Math.Sin(euler.x * 0.01745329252 * 0.5);
            double siny = Math.Sin(euler.y * 0.01745329252 * 0.5);
            double sinz = Math.Sin(euler.z * 0.01745329252 * 0.5);

            double rx = cosy * sinx * cosz + siny * cosx * sinz;
            double ry = siny * cosx * cosz - cosy * sinx * sinz;
            double rz = cosy * cosx * sinz - siny * sinx * cosz;
            double rw = cosy * cosx * cosz + siny * sinx * sinz;

            double x2 = 2.0 * rx * rx;
            double y2 = 2.0 * ry * ry;
            double z2 = 2.0 * rz * rz;
            double xy = 2.0 * rx * ry;
            double xz = 2.0 * rx * rz;
            double xw = 2.0 * rx * rw;
            double yz = 2.0 * ry * rz;
            double yw = 2.0 * ry * rw;
            double zw = 2.0 * rz * rw;

            double ra = 1.0 - y2 - z2;
            double rb = xy + zw;
            double rc = xz - yw;
            double rd = xy - zw;
            double re = 1.0 - x2 - z2;
            double rf = yz + xw;
            double rg = xz + yw;
            double rh = yz - xw;
            double ri = 1.0 - x2 - y2;

            Matrix m = default(Matrix);
            m.m00 = scale.x * ra;
            m.m01 = scale.x * rb;
            m.m02 = scale.x * rc;
            m.m03 = 0.0;
            m.m10 = scale.y * rd;
            m.m11 = scale.y * re;
            m.m12 = scale.y * rf;
            m.m13 = 0.0;
            m.m20 = scale.z * rg;
            m.m21 = scale.z * rh;
            m.m22 = scale.z * ri;
            m.m23 = 0.0;
            m.m30 = position.x;
            m.m31 = position.y;
            m.m32 = position.z;
            m.m33 = 1.0f;

            return m;
        }

    }
}
