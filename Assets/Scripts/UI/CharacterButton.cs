using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    [Header("Sound System")]
    public AudioSource audioSource;
    [SerializeField] public AudioClip hoverSound;
    [SerializeField] public AudioClip selectSound;

    [Header("Hover")]
    public GameObject hover;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectedSound()
    {
        if (!GameManager.inst.selected)
        {
            GameManager.inst.selected = true;
            if (selectSound != null)
            {
                audioSource.PlayOneShot(selectSound);
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
        yield return new WaitForSeconds(2f);
        GameManager.inst.LoadMainStage();
    }
}
