using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageTransitionManager : StageTransitionManager
{
    public override IEnumerator BossStageTransition()
    {
        if (transitionAnimator != null)
        {
            // Play the animation
            transitionAnimator.Play("BossStageStart");

            // Get animation length
            AnimatorStateInfo stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("BossStageStart"))
            {
                yield return null;
                stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
            }
        }
    }
}
