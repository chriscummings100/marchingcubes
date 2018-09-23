using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//distance field functions - with some help from
//https://iquilezles.org/www/articles/distfunctions/distfunctions.htm

public static class SDFOps
{
    public static Vector3 V3Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static float Sphere(Vector3 p, Vector3 centre, float radius)
    {
        p -= centre;
        return p.magnitude - radius;
    }

    public static float Box(Vector3 p, Vector3 centre, Vector3 halfExtent)
    {
        p -= centre;
        Vector3 d = V3Abs(p) - halfExtent;
        return Mathf.Min(Mathf.Max(d.x, d.y, d.z), 0.0f) + Vector3.Magnitude(Vector3.Max(d, Vector3.zero));
    }

    public static float Union(float a, float b)
    {
        return Mathf.Min(a, b);
    }

    public static float SmoothMin(float a, float b, float k)
    {
        float h = Mathf.Max(k - Mathf.Abs(a - b), 0.0f);
        return Mathf.Min(a, b) - h * h * 0.25f / k;
    }

    public static float Blend(float a, float b, float k)
    {
        return SmoothMin(a, b, k);
    }
}
