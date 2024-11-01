using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCloseMouth()
    {
        if (animator.GetBool("mouthOpen_b")) animator.SetBool("mouthOpen_b", false);
        else animator.SetBool("mouthOpen_b", true);
    }
}
