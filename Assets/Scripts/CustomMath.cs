using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath : MonoBehaviour
{
    public static int Clamp(int num, int min, int max)
    {
        if (max < min)
        {
            max = min;
        }
        if (num < min)
        {
            return min;
        }
        else if (num > max)
        {
            return max;
        }
        return num;
    }
}
