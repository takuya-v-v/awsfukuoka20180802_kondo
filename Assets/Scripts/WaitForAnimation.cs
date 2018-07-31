using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAnimation : CustomYieldInstruction
{
    Animator animator;
    int lastStateHash = 0;
    int layerNo = 0;

    public WaitForAnimation(Animator animator, int layerNo = 0)
    {
        this.layerNo = layerNo;
        this.animator = animator;
        this.lastStateHash = animator.GetCurrentAnimatorStateInfo(layerNo).fullPathHash;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorState = animator.GetCurrentAnimatorStateInfo(layerNo);
            return currentAnimatorState.fullPathHash == lastStateHash &&
                (currentAnimatorState.normalizedTime < 1);
        }
    }
}
