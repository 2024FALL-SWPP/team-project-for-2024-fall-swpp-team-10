using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : MonoBehaviour
{
    public Animator animator;
    int col;
    Color[] myColors = new Color[] { Color.white, Color.red, Color.gray };

    // Start is called before the first frame update
    void Start()
    {
        col = 0;
        ColorUtility.TryParseHtmlString("#CB5353", out myColors[1]);
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

    public void ChangeColor()
    {
        col = (col + 1) % 3;
        foreach (Transform childTransform in gameObject.transform)
        {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) smr.material.color = myColors[col];
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = myColors[col];
        }
    }
}
