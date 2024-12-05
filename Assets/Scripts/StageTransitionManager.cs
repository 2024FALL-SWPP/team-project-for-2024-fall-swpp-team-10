using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageTransitionManager : MonoBehaviour
{
    [SerializeField] protected Animator transitionAnimator;  // Reference to the Animator component
    [SerializeField] public float countdownDuration = 5;

    public virtual IEnumerator StartMainStageTransition()
    {
        throw new NotImplementedException();
    }

    public virtual void SetCurrentCharacter(GameObject _character)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator Countdown()
    {
        throw new NotImplementedException();
    }

    public virtual float BossStageTransition()
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator BossStageTransitionCoroutine()
    {
        throw new NotImplementedException();
    }
}