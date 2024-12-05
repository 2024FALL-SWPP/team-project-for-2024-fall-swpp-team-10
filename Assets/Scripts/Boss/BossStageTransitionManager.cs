using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class BossStageTransitionManager : StageTransitionManager
{
    [Header("Transition Settings")]
    [SerializeField] TextMeshProUGUI countdownText;
    public override float BossStageTransition()
    {
        StartCoroutine("BossStageTransitionCoroutine");
        AnimatorStateInfo stateInfo = transitionAnimator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length / stateInfo.speed;
        return animationDuration;
    }

    public override IEnumerator BossStageTransitionCoroutine()
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

    public override IEnumerator Countdown()
    {
        // Start countdown
        countdownText.gameObject.SetActive(true);
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = $"Boss Stage Starting in {i}...";
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);
    }
}
