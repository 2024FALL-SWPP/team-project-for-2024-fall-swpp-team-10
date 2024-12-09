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
    public int characterNum;

    private int blinkcount = 3;
    // KeyCode[] keyCodes = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.L)) //mac의 경우 shift + option + L
        {
            if (GameManager.inst.IsUnlocked(characterNum))
                PlayerPrefs.DeleteKey(GameManager.inst.PlayerPrefsCharacterUnlockKey((Character)characterNum));
        }

        if (GameManager.inst.IsUnlocked(characterNum))
        {
            if (lockImage != null)
                lockImage.SetActive(false);
        }
        else
        {
            if (lockImage != null)
                lockImage.SetActive(true);
        }
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
