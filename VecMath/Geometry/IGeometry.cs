﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VecMath.Geometry
{
    public interface IGeometry
    {
        bool IsIntersect(Vector3 pos, Vector3 vec);
        bool IsIntersect(Vector3 pos);
        float CalculateTimeToIntersect(Vector3 pos, Vector3 vec);
    }

    public struct AABoundingBox : IGeometry
    {
        public static AABoundingBox Root { get; } = new AABoundingBox(new Vector3(1, 1, 1) * -1E10F, new Vector3(1, 1, 1) * 1E10F);

        public Vector3 PosMin { get; set; }
        public Vector3 PosMax { get; set; }

        public AABoundingBox(Vector3 pos1, Vector3 pos2)
        {
            PosMin = MathUtil.Min(pos1, pos2);
            PosMax = MathUtil.Max(pos1, pos2);
        }

        public bool IsIntersect(Vector3 pos)
        {
            return MathUtil.WithIn(pos, PosMin, PosMax);
        }

        public bool IsIntersect(Vector3 pos, Vector3 vec)
        {
            return CalculateTimeToIntersect(pos, vec, out float min, out float max);
        }

        public float CalculateTimeToIntersect(Vector3 pos, Vector3 vec)
        {
            CalculateTimeToIntersect(pos, vec, out float min, out float max);
            return min;
        }

        public bool CalculateTimeToIntersect(Vector3 pos, Vector3 vec, out float min, out float max)
        {
            max = float.MaxValue;
            min = float.MinValue;

            if (IsIntersect(pos))
            {
                min = 0;

                for (int i = 0; i < 3; i++)
                {
                    float v = vec[i] != 0 ? vec[i] : 1E-7F;
                    float t1 = (PosMin[i] - pos[i]) / v;
                    float t2 = (PosMax[i] - pos[i]) / v;

                    max = Math.Min(max, Math.Max(t1, t2));
                }
                return true;
            }

            for (int i = 0; i < 3; i++)
            {
                float v = vec[i] != 0 ? vec[i] : 1E-7F;
                float t1 = (PosMin[i] - pos[i]) / v;
                float t2 = (PosMax[i] - pos[i]) / v;

                float near = Math.Min(t1, t2);
                float far = Math.Max(t1, t2);

                min = Math.Max(min, near);
                max = Math.Min(max, far);

                if (min > max) return false;
            }
            return true;
        }
    }

    public struct Triangle : IGeometry
    {
        public Vector3 Pos1 { get; }
        public Vector3 Pos2 { get; }
        public Vector3 Pos3 { get; }
        public Vector3 Normal { get; }

        public Triangle(Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            Pos1 = pos1;
            Pos2 = pos2;
            Pos3 = pos3;
            Normal = (Pos3 - Pos1) ^ (Pos2 - Pos1);
        }

        public bool IsIntersect(Vector3 pos)
        {
            return Math.Abs((Pos1 - pos) * Normal) <= 1E-10;
        }

        public bool IsIntersect(Vector3 pos, Vector3 vec)
        {
            var cross1 = (Pos2 - Pos1) ^ (pos - Pos1);
            var dot1 = cross1 * vec;

            var cross2 = (Pos3 - Pos2) ^ (pos - Pos2);
            var dot2 = cross2 * vec;

            if (dot1 > 0 ^ dot2 > 0)
            {
                return false;
            }

            var cross3 = (Pos1 - Pos3) ^ (pos - Pos3);
            var dot3 = cross3 * vec;

            return !(dot2 > 0 ^ dot3 > 0);
        }

        public float CalculateTimeToIntersect(Vector3 pos, Vector3 vec)
        {
            float dot = Normal * vec;

            if (Math.Abs(dot) <= 1E-6) return 1E+6F;

            return Normal * (Pos1 - pos) / dot;
        }
    }

    public struct Plane : IGeometry
    {
        public Vector3 Pos { get; set; }
        public Vector3 Normal { get; set; }

        public Plane(Vector3 pos, Vector3 normal)
        {
            Pos = pos;
            Normal = normal;
        }

        public bool IsIntersect(Vector3 pos)
        {
            return Math.Abs((Pos - pos) * Normal) <= 1E-10;
        }

        public bool IsIntersect(Vector3 pos, Vector3 vec)
        {
            return Math.Abs(Normal * vec) > 1E-6;
        }

        public float CalculateTimeToIntersect(Vector3 pos, Vector3 vec)
        {
            float dot = Normal * vec;

            if (Math.Abs(dot) <= 1E-6) return 1E+6F;

            return Normal * (Pos - pos) / dot;
        }
    }
}
