using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonHandler : MonoBehaviour
{
    [Header("Sound System")]
    public AudioSource audioSource;
    [SerializeField] public AudioClip hoverSound;
    [SerializeField] public AudioClip selectSound;

    [Header("Hover")]
    public GameObject hover;

    private int blinkcount = 3;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick()
    {
        if (!GameManager.inst.selected)
        {
            GameManager.inst.selected = true;
            if (selectSound != null)
            {
                audioSource.PlayOneShot(selectSound, 3f);
                StartCoroutine(WaitTwoSecondsAndStart());
            }
        }
    }

    public void OnPointerEnter()
    {
        if (!GameManager.inst.selected)
        {
            hover.SetActive(true);
            if (hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }
    }

    public void OnPointerExit()
    {
        if (!GameManager.inst.selected)
            hover.SetActive(false);
    }

    IEnumerator WaitTwoSecondsAndStart()
    {
        hover.SetActive(true);
        for (int i = 0; i < blinkcount; i++)
        {
            yield return new WaitForSeconds(0.4f);
            hover.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            hover.SetActive(true);
        }
        GameManager.inst.selected = false;
        GameManager.inst.LoadMainStage();
    }
}
