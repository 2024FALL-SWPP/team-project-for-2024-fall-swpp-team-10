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

    public virtual IEnumerator StartMainStageTransition()
    {
        throw new NotImplementedException();
    }

    public virtual void SetCurrentCharacter(GameObject _character)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator BossStageTransition()
    {
        throw new NotImplementedException();
    }
}