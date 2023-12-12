using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath : MonoBehaviour
{
    // Clamps an int num between min and max
    public static int Clamp(int num, int min, int max)
    {
        if (max < min)
        {
            max = min;
        }
        return (num < min) ? min : ((num > max) ? max : num);
    }

    // Clamps a float num between min and max
    public static float ClampF(float num, float min, float max)
    {
        if (max < min)
        {
            max = min;
        }
        return (num < min) ? min : ((num > max) ? max : num);
    }

    // Projects the target onto the XZ plane of given origin
    public static Vector3 GetProjectedTarget(Vector3 origin, Vector3 target)
    {
        target.y = origin.y;
        return target;
    }

    // Returns a direction vector towards the target
    public static Vector3 GetDirectionTo(Vector3 origin, Vector3 target, bool ignoreY)
    {
        target = GetProjectedTarget(origin, target);
        return (target - origin).normalized;
    }

    // Clamps the direction along the 4 cardinal vectors in the XZ plane
    public static Vector3 ClampAlongCardinals(Vector3 dir)
    {
        if (!(dir.x == 0 && dir.z == 0)) // Not a ZERO Vector along XZ vector
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            {
                dir.x = dir.x / Mathf.Abs(dir.x);
                dir.z = 0.0f;
            }
            else
            {
                dir.z = dir.z / Mathf.Abs(dir.z);
                dir.x = 0.0f;
            }
        }
        return dir;
    }
}
