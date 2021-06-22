using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Vector3> Points;
    public Transform Train;
    public float Speed = 3f;
    private readonly List<Vector3> _pointsList = new List<Vector3>();
    private readonly List<float> _pointsTimes = new List<float>();
    private float _time;
    private float lastTime;

    void Start()
    {
        var time = 0f;
        foreach (var point in Points)
        {
            if (_pointsList.Count > 0)
            {
                var lastPoint = _pointsList.LastOrDefault();
                time += (lastPoint - point).magnitude;
            }
            _pointsList.Add(point);
            _pointsTimes.Add(time);
        }

        var first = _pointsList.FirstOrDefault();
        var last = _pointsList.LastOrDefault();
        lastTime = time + (first - last).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime * Speed;
        if (_time > lastTime)
        {
            _time -= lastTime;
        }

        Train.rotation = Quaternion.LookRotation(GetPositionCatmull(_time + 2f) - GetPositionCatmull(_time - 2f));

        Train.position = GetPositionCatmull(_time);

        for (var time = 0f; time < lastTime; time += lastTime / 20f)
        {
            Debug.DrawLine(GetPositionCatmull(time + lastTime / 40f), GetPositionCatmull(time - lastTime / 40f), Color.green);
        }
    }

    private Vector3 GetPosition(float time)
    {
        time %= lastTime;

        var fromIndex = _pointsTimes.FindLastIndex(pointTime => pointTime <= time);
        if (fromIndex >= _pointsTimes.Count) fromIndex = 0;
        if (fromIndex < 0) fromIndex = 0;
        var toIndex = fromIndex + 1;
        if (toIndex >= _pointsTimes.Count) toIndex = 0;

        var fromTime = _pointsTimes[fromIndex];
        var toTime = _pointsTimes[toIndex];
        var fromPoint = _pointsList[fromIndex];
        var toPoint = _pointsList[toIndex];

        Debug.DrawLine(fromPoint, toPoint, Color.blue);
        var point  = Vector3.Lerp(fromPoint, toPoint, toTime > fromTime ? (time - fromTime) / (toTime - fromTime) : (time - fromTime) / (toTime + lastTime - fromTime));
        Debug.DrawLine(point, point + Vector3.up, Color.blue);
        return point;
    }

    private Vector3 GetPositionCatmull(float time)
    {
        time %= lastTime;

        var i1 = _pointsTimes.FindLastIndex(pointTime => pointTime <= time);
        if (i1 < 0) i1 = 0;
        i1 %= _pointsTimes.Count;

        var i0 = (i1 < 1) ? _pointsTimes.Count - 1 : i1 - 1;
        var i2 = (i1 + 1) % _pointsTimes.Count;
        var i3 = (i1 + 2) % _pointsTimes.Count;
        
        var point = GetCatmull(time, i0, i1, i2, i3);
        return point;
    }

    private Vector3 GetCatmull(float t, int i0, int i1, int i2, int i3)
    {
        var p0 = _pointsList[i0];
        var p1 = _pointsList[i1];
        var p2 = _pointsList[i2];
        var p3 = _pointsList[i3];
        var t0 = _pointsTimes[i0];
        var t1 = _pointsTimes[i1];
        var t2 = _pointsTimes[i2];
        var t3 = _pointsTimes[i3];
        
        if (t1 < t0) t1 += lastTime;
        if (t2 < t1) t2 += lastTime;
        if (t3 < t2) t3 += lastTime;
        if (t < t1) t += lastTime;

        return CatmullRomSpline(t, p0, t0, p1, t1, p2, t2, p3, t3);
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
