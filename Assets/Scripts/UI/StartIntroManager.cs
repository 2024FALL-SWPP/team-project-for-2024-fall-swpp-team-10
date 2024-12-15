using UnityEngine;
using TMPro;
using UnityEngine.Video;
using System.Collections;

public class StartIntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public CanvasGroup titleCanvasGroup;
    public GameObject title;
    public TextMeshProUGUI pressAnyKeyText;
    public float fadeDuration = 2f;

    private bool isVideoFinished = false;
    private bool hasFadedIn = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        if (titleCanvasGroup != null)
        {
            titleCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogError("Title Canvas Group not assigned.");
        }
        if (title != null)
        {
            title.SetActive(false);
        }
        else
        {
            Debug.LogError("Title not assigned.");
        }

        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("\"Press Any Key\" text not assigned.");
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer not assigned.");
        }
    }

    void Update()
    {
        if (isVideoFinished && !hasFadedIn)
        {
            StartCoroutine(FadeInTitle());
        }

        if (hasFadedIn)
        {
            if (Input.anyKeyDown)
            {
                if (GameManager.inst != null)
                {
                    GameManager.inst.LoadMainMenu();
                }
                else
                {
                    Debug.LogError("GameManager instance not found.");
                }
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        isVideoFinished = true;
    }

    IEnumerator FadeInTitle()
    {
        hasFadedIn = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            if (titleCanvasGroup != null)
            {
                titleCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (titleCanvasGroup != null)
        {
            titleCanvasGroup.alpha = 1f;
        }

        if (title != null)
        {
            title.SetActive(true);
        }
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(true);
            blinkCoroutine = StartCoroutine(BlinkPressAnyKey());
        }
    }

    IEnumerator BlinkPressAnyKey()
    {
        while (true)
        {
            if (pressAnyKeyText != null)
            {
                pressAnyKeyText.enabled = true;
                yield return new WaitForSecondsRealtime(1f);
                pressAnyKeyText.enabled = false;
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                yield break;
            }
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
    }
}
