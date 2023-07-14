using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Standard : MonoBehaviour
{
    // Checks if the given animator is playing the gicen animation
    public static bool IsAnimationPlaying(Animator anim, string animName)
    {
        return ((anim.GetCurrentAnimatorStateInfo(0)).IsName(animName)
            && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f));
    }
}
