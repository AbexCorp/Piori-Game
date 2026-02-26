using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }
    public static float DistanceTo(this Vector2 from, Vector2 to)
    {
        return (to - from).magnitude;
    }
}
