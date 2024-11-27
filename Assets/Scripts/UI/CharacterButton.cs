using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    [Header("Sound System")]
    [SerializeField] public AudioClip hoverSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;

    [Header("Hover")]
    public GameObject hover;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter()
    {
        hover.SetActive(true);
        if (hoverSound != null)
        {
            AudioSource.PlayClipAtPoint(hoverSound, new Vector3(960, 540, 0), volume);
        }
    }

    public void OnPointerExit()
    {
        hover.SetActive(false);
    }
}
