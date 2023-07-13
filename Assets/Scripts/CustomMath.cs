using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath : MonoBehaviour
{
    // Clamps the num between min and max
    public static int Clamp(int num, int min, int max)
    {
        if (max < min)
        {
            max = min;
        }
        return (num < min) ? (min) : ((num > max) ? (max) : (num));
    }

    // Projects the target onto the Y plane of origin
    public static Vector3 GetProjectedTarget(Vector3 origin, Vector3 target)
    {
        target.y = origin.y;
        return target;
    }

    // Returns a direction vector to target
    public static Vector3 GetDirectionTo(Vector3 origin, Vector3 target, bool ignoreY)
    {
        target = GetProjectedTarget(origin, target);
        return (target - origin).normalized;
    }

    // Clamps the direction along the 4 cardinal vectors in the XZ plane
    public static Vector3 ClampAlongCardinals(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction.x = direction.x / Mathf.Abs(direction.x);
            direction.z = 0.0f;
        }
        else
        {
            direction.z = direction.z / Mathf.Abs(direction.z);
            direction.x = 0.0f;
        }
        return direction;
    }
}
