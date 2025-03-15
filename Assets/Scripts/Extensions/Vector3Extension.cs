using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 DirectionTo3D(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
    public static Vector3 DirectionTo2D(this Vector3 from, Vector3 to)
    {
        return (new Vector3(to.x, 0, to.z) - new Vector3(from.x, 0, from.z)).normalized;
    }
    public static float DistanceTo3D(this Vector3 from, Vector3 to)
    {
        return (to - from).magnitude;
    }
    public static float DistanceTo2D(this Vector3 from, Vector3 to)
    {
        return (new Vector3(to.x, 0, to.z) - new Vector3(from.x, 0, from.z)).magnitude;
    }
}
