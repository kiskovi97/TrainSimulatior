using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Main
{
    class CatmullMesh
    {
        public Mesh _mesh = new Mesh();
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<int> _triangles = new List<int>();
        public float LineWidth = 2f;
        private const float stepCount = 10f;
        private const float dirCount = 2f;

        public void UpdateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.RecalculateNormals();
        }

        public void CleareShape()
        {
            _vertices.Clear();
            _triangles.Clear();
        }

        public void AddCatmullLine(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var distance = (p1 - p2).magnitude;
            var lastTime = distance + dirCount * 2f;

            var step = lastTime / (stepCount * 2f);
            for (var time = 1f; time < lastTime - 1f; time += step * 2f)
            {
                var P1 = CatmullRomSpline(time - step * 3, p0, 0f, p1,  dirCount, p2, distance + dirCount, p3, lastTime);
                var P2 = CatmullRomSpline(time - step, p0, 0f, p1, dirCount, p2, distance + dirCount, p3, lastTime);
                var P3 = CatmullRomSpline(time + step, p0, 0f, p1, dirCount, p2, distance + dirCount, p3, lastTime);
                var P4 = CatmullRomSpline(time + step * 3, p0, 0f, p1, dirCount, p2, distance + dirCount, p3, lastTime);

                AddLine(P1, P2, P3, P4);
            }
        }

        public Vector3 GetPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            var distance = (p1 - p2).magnitude;
            var lastTime = distance + dirCount * 2f;
            return CatmullRomSpline(time, p0, 0f, p1, dirCount, p2, distance + dirCount, p3, lastTime);
        }

        private void AddLine(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var start = _vertices.Count;

            var left = (p0 - p2).normalized * LineWidth / 2f;
            left = new Vector3(left.z, left.y, -left.x);
            var left2 = (p1 - p3).normalized * LineWidth / 2f;
            left2 = new Vector3(left2.z, left2.y, -left2.x);

            _vertices.Add(p1 - left);
            _vertices.Add(p1 + left);
            _vertices.Add(p2 + left2);
            _vertices.Add(p2 - left2);

            _triangles.Add(start);
            _triangles.Add(start + 1);
            _triangles.Add(start + 2);
            _triangles.Add(start);
            _triangles.Add(start + 2);
            _triangles.Add(start + 3);
        }

        private static Vector3 CatmullRomSpline(float t, Vector3 p0, float t0, Vector3 p1, float t1, Vector3 p2, float t2, Vector3 p3, float t3)
        {
            var a1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            var a2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            var a3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

            var b1 = (t2 - t) / (t2 - t0) * a1 + (t - t0) / (t2 - t0) * a2;
            var b2 = (t3 - t) / (t3 - t1) * a2 + (t - t1) / (t3 - t1) * a3;

            return (t2 - t) / (t2 - t1) * b1 + (t - t1) / (t2 - t1) * b2;
        }
    }
}
