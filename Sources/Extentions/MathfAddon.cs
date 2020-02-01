using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfAddon
{

	public static Vector3 LinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        return p0 + t * (p1 - p0);
    }

    public static Vector3 QuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float sqrt = t * t;
        float sqru = u * u;
        Vector3 p = sqru * p0;
        p += 2 * u * t * p1;
        p += sqrt * p2;
        return p;
    }
}
