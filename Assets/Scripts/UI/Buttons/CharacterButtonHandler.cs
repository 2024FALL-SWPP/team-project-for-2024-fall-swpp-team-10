using System.Collections;
using System.Collections.Generic;
using EnumManager;
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

    [Header("Lock Image")]
    public GameObject lockImage;
    public Character character;

    private int blinkcount = 3;
    private bool actionEnabled = true;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.L)) //mac의 경우 shift + option + L
        {
            if (GameManager.inst.IsUnlocked(character))
                PlayerPrefs.DeleteKey(GameManager.inst.PlayerPrefsCharacterUnlockKey(character));
        }

        if (GameManager.inst.IsUnlocked(character))
        {
            if (lockImage != null)
                lockImage.SetActive(false);
        }
        else
        {
            if (lockImage != null)
                lockImage.SetActive(true);
        }

        actionEnabled = GameManager.inst.IsUnlocked(character) || lockImage == null;
    }

    public void OnPointerClick()
    {
        if (!GameManager.inst.IsSelected() && actionEnabled)
        {
            GameManager.inst.SetSelected(true);
            if (selectSound != null)
            {
                audioSource.PlayOneShot(selectSound, 3f);
                StartCoroutine(WaitTwoSecondsAndStart());
            }
        }
    }

    public void OnPointerEnter()
    {
        if (!GameManager.inst.IsSelected() && actionEnabled)
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
        if (!GameManager.inst.IsSelected() && actionEnabled)
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
        GameManager.inst.SetSelected(false);
        GameManager.inst.LoadMainStage();
    }
}
