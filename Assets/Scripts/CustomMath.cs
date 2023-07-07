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
}
